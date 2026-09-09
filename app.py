import json
import math
import re
import time
from collections import Counter, defaultdict

import faiss
import streamlit as st
from google import genai
from sentence_transformers import SentenceTransformer


st.set_page_config(
    page_title="Shipra AI Assistant",
    page_icon="🤖",
    layout="centered",
)

st.title("🤖 Shipra Full-Stack AI Assistant")
st.write("Ask naturally about the Shipra frontend or backend project.")


GEMINI_API_KEY = st.secrets["GEMINI_API_KEY"]
client = genai.Client(api_key=GEMINI_API_KEY)


STOP_WORDS = {
    "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
    "how", "i", "in", "is", "it", "main", "mai", "me", "my", "of",
    "on", "or", "the", "this", "to", "was", "what", "when", "where",
    "which", "who", "why", "with", "you", "your", "yar", "yaar", "hai",
    "hain", "hy", "hyn", "kya", "kia", "ka", "ki", "ke", "ko", "k",
    "se", "sy", "aur", "kon", "rha", "raha", "rhy", "rahi", "ho",
    "hota", "hoti", "hotay", "hote", "time", "wala", "wali", "bhi",
    "batao", "btao", "bata", "bta", "mujhe", "mjhy", "kr", "kro",
    "kar", "kare", "karen", "karte", "step", "steps", "proper",
}


def normalize_token(word):
    word = word.lower()

    aliases = {
        "frontend": "frontend",
        "front": "frontend",
        "backend": "backend",
        "back": "backend",
        "sale": "sale",
        "sales": "sale",
        "channel": "channel",
        "channels": "channel",
        "error": "error",
        "errors": "error",
        "fail": "error",
        "failed": "error",
        "failure": "error",
        "success": "success",
        "successful": "success",
    }

    if word in aliases:
        return aliases[word]
    if word.startswith(("activat", "connect")):
        return "connect"
    if word.startswith("validat"):
        return "validate"
    if word.startswith("configur") or word == "config":
        return "config"
    if word.startswith("creat"):
        return "create"
    if word in {"add", "adding", "added"}:
        return "create"
    if word.startswith("updat"):
        return "update"
    if word.startswith("delet") or word.startswith("remov"):
        return "delete"
    if word.startswith("retriev") or word.startswith("fetch") or word == "get":
        return "fetch"

    return word


def tokenize(text):
    text = re.sub(r"([a-z0-9])([A-Z])", r"\1 \2", str(text))
    words = re.findall(r"[a-zA-Z0-9]+", text.lower())

    return {
        normalize_token(word)
        for word in words
        if len(word) >= 2 and word not in STOP_WORDS
    }


@st.cache_resource(show_spinner="Loading Shipra knowledge base...")
def load_data():
    with open("chunks.json", "r", encoding="utf-8") as file:
        chunks = json.load(file)

    with open("metadata.json", "r", encoding="utf-8") as file:
        metadata = json.load(file)

    index = faiss.read_index("faiss.index")
    embedding_model = SentenceTransformer("all-MiniLM-L6-v2")

    if len(chunks) != len(metadata):
        raise ValueError(
            "chunks.json and metadata.json contain different item counts."
        )

    if index.ntotal != len(chunks):
        raise ValueError(
            "faiss.index does not match chunks.json. Rebuild the index."
        )

    document_tokens = []
    path_tokens = []
    token_document_frequency = Counter()
    file_chunk_lookup = defaultdict(dict)

    for idx, (chunk, item) in enumerate(zip(chunks, metadata)):
        file_path = item.get("file_path", "")
        section = item.get("section_title", "")
        tokens = tokenize(f"{file_path} {section} {chunk}")
        current_path_tokens = tokenize(file_path)

        document_tokens.append(tokens)
        path_tokens.append(current_path_tokens)
        token_document_frequency.update(tokens)

        chunk_number = item.get("chunk_index")
        if isinstance(chunk_number, int):
            file_chunk_lookup[file_path][chunk_number] = idx

    return (
        chunks,
        metadata,
        index,
        embedding_model,
        document_tokens,
        path_tokens,
        token_document_frequency,
        file_chunk_lookup,
    )


(
    chunks,
    metadata,
    index,
    embedding_model,
    document_tokens,
    path_tokens,
    token_document_frequency,
    file_chunk_lookup,
) = load_data()


def search_documentation(question, top_k=15):
    query_tokens = tokenize(question)
    query_code_names = set(
        re.findall(
            r"\b(?:[a-z_]+[A-Z][A-Za-z0-9_]*|"
            r"[A-Z][a-z0-9]+(?:[A-Z][A-Za-z0-9_]*)+)\b",
            question,
        )
    )

    question_embedding = embedding_model.encode(
        [question],
        convert_to_numpy=True,
        normalize_embeddings=True,
    ).astype("float32")

    semantic_count = min(max(top_k * 10, 100), index.ntotal)
    distances, indices = index.search(question_embedding, semantic_count)

    semantic_rank = {
        int(idx): rank
        for rank, idx in enumerate(indices[0])
        if int(idx) >= 0
    }
    distance_map = {
        int(idx): float(distance)
        for distance, idx in zip(distances[0], indices[0])
        if int(idx) >= 0
    }

    asks_for_frontend = "frontend" in query_tokens
    asks_for_backend = "backend" in query_tokens
    asks_for_flow = bool(
        query_tokens.intersection(
            {"connect", "create", "update", "delete", "validate", "flow"}
        )
    )
    asks_for_sale_channel_create = (
        {"sale", "channel", "connect", "create"}.issubset(query_tokens)
        and "sync" not in query_tokens
    )

    scored = []
    total_documents = len(chunks)

    for idx, tokens in enumerate(document_tokens):
        overlap = query_tokens.intersection(tokens)
        lexical_score = 0.0

        for token in overlap:
            frequency = token_document_frequency.get(token, 0)
            lexical_score += 1.0 + math.log(
                (total_documents + 1) / (frequency + 1)
            )

        path_overlap = query_tokens.intersection(path_tokens[idx])
        path_score = len(path_overlap) * 14.0

        rank = semantic_rank.get(idx)
        semantic_score = 0.0 if rank is None else 8.0 / (rank + 1)

        project = metadata[idx].get("project", "backend")
        file_path = metadata[idx].get("file_path", "")
        is_actual_code = (
            metadata[idx].get("source_type") == "actual_code"
            or file_path != "Shipra.Backend.API documentation"
        )

        project_score = 0.0
        if asks_for_frontend and project == "frontend":
            project_score += 10.0
        if asks_for_backend and project == "backend":
            project_score += 10.0
        if asks_for_flow and is_actual_code:
            project_score += 20.0

        exact_query_score = sum(
            500.0
            for name in query_code_names
            if name in chunks[idx] or name.lower() in file_path.lower()
        )

        intent_score = 0.0
        lowered_path = file_path.lower()
        if asks_for_sale_channel_create:
            if "salechannelconnectmodal" in lowered_path:
                intent_score += 1200.0
            if "createsalechannelconfig" in (
                lowered_path + "\n" + chunks[idx].lower()
            ):
                intent_score += 500.0
            if "/orders/" in lowered_path or "orderModal".lower() in lowered_path:
                intent_score -= 1000.0
            if "syncpolic" in lowered_path:
                intent_score -= 1000.0

        score = (
            (lexical_score * 3.0)
            + path_score
            + semantic_score
            + project_score
            + exact_query_score
            + intent_score
        )

        if score > 0:
            scored.append((score, idx))

    # Discover domain call names from the strongest matching frontend code.
    # Example: CreateSaleChannelConfig is then matched exactly across the
    # Axios helper, controller, command handler, repository, and entity.
    scored.sort(key=lambda item: item[0], reverse=True)
    link_identifiers = set()

    frontend_seeds = [
        idx
        for _, idx in scored
        if metadata[idx].get("project") == "frontend"
        and metadata[idx].get("source_type") == "actual_code"
    ][:8]

    for seed_idx in frontend_seeds:
        identifiers = re.findall(
            r"\b[A-Z][A-Za-z0-9]{5,}\b",
            chunks[seed_idx],
        )
        for identifier in identifiers:
            identifier_tokens = tokenize(identifier)
            if len(identifier_tokens.intersection(query_tokens)) >= 2:
                link_identifiers.add(identifier)

    if asks_for_sale_channel_create:
        # The create/connect modal has one canonical cross-layer call. Keeping
        # this identifier exclusive prevents order-assignment helpers from
        # splicing a different workflow into the answer.
        link_identifiers = {"CreateSaleChannelConfig"}

    exact_link_counts = {}

    if link_identifiers:
        linked_scores = []
        for score, idx in scored:
            item = metadata[idx]
            file_path = item.get("file_path", "")
            symbol = item.get("symbol") or ""
            searchable = "\n".join(
                [
                    file_path,
                    symbol,
                    chunks[idx],
                ]
            )
            exact_links = sum(
                identifier in searchable
                for identifier in link_identifiers
            )
            canonical_links = sum(
                (
                    f"/{identifier.lower()}/" in file_path.lower()
                    or symbol.lower() in {
                        identifier.lower(),
                        f"{identifier.lower()}command",
                        f"{identifier.lower()}commandhandler",
                    }
                )
                for identifier in link_identifiers
            )
            exact_link_counts[idx] = exact_links
            linked_scores.append(
                (
                    score
                    + (exact_links * 150.0)
                    + (canonical_links * 600.0),
                    idx,
                )
            )

        scored = sorted(
            linked_scores,
            key=lambda item: (
                exact_link_counts.get(item[1], 0) > 0,
                item[0],
            ),
            reverse=True,
        )

    # For flow questions, select evidence across distinct project layers. This
    # prevents a similarly named update/sync feature from crowding out the real
    # frontend -> controller -> handler -> repository -> entity chain.
    anchor_indices = []
    anchors_per_file = Counter()
    used_categories = set()

    # Reserve the first anchors for the real cross-layer call chain. Without
    # this, an unrelated page/repository can occupy a category merely because
    # it is semantically similar to the user's wording.
    if asks_for_flow and link_identifiers:
        stage_checks = [
            lambda project, path: (
                project == "frontend" and "/src/components/" in path
            ),
            lambda project, path: (
                project == "frontend"
                and ("/src/api/" in path or "/src/services/" in path)
            ),
            lambda project, path: (
                project == "backend"
                and (".Web/" in path or "/Api/" in path)
            ),
            lambda project, path: (
                project == "backend"
                and (".Application/" in path or "/Features/" in path)
            ),
            lambda project, path: (
                project == "backend"
                and (".Infrastructure/" in path or "/Repository/" in path)
            ),
            lambda project, path: (
                project == "backend" and ".Core/" in path
            ),
        ]

        for stage_check in stage_checks:
            for _, idx in scored:
                file_path = metadata[idx].get("file_path", "")
                project = metadata[idx].get("project", "backend")

                if exact_link_counts.get(idx, 0) <= 0:
                    continue
                if file_path in anchors_per_file:
                    continue
                if not stage_check(project, file_path):
                    continue

                anchor_indices.append(idx)
                anchors_per_file[file_path] += 1
                used_categories.add(
                    (project, metadata[idx].get("layer", "documentation"))
                )
                break

    for _, idx in scored:
        file_path = metadata[idx].get("file_path", "")
        project = metadata[idx].get("project", "backend")
        layer = metadata[idx].get("layer", "documentation")
        category = (project, layer)

        if anchors_per_file[file_path] >= 1:
            continue
        if asks_for_flow and category in used_categories:
            continue

        anchor_indices.append(idx)
        anchors_per_file[file_path] += 1
        used_categories.add(category)

        if len(anchor_indices) >= 6:
            break

    # Include every layer anchor first, then add neighboring chunks in rounds.
    # This preserves complete functions without letting the first file consume
    # the whole context budget.
    selected_indices = list(anchor_indices)

    for offset in (-1, 1, -2, 2):
        for anchor_idx in anchor_indices:
            item = metadata[anchor_idx]
            file_path = item.get("file_path", "")
            chunk_number = item.get("chunk_index")
            if not isinstance(chunk_number, int):
                continue

            neighbor_idx = file_chunk_lookup.get(file_path, {}).get(
                chunk_number + offset
            )
            if neighbor_idx is not None and neighbor_idx not in selected_indices:
                selected_indices.append(neighbor_idx)

            if len(selected_indices) >= top_k:
                break

        if len(selected_indices) >= top_k:
            break

    for idx in indices[0]:
        idx = int(idx)
        if idx >= 0 and idx not in selected_indices:
            selected_indices.append(idx)
        if len(selected_indices) >= top_k:
            break

    results = []
    for idx in selected_indices[:top_k]:
        item = metadata[idx]
        results.append(
            {
                "chunk_id": idx,
                "distance": distance_map.get(idx),
                "project": item.get("project", "backend"),
                "source_type": item.get("source_type", "documentation"),
                "file_path": item.get(
                    "file_path",
                    "Shipra.Backend.API documentation",
                ),
                "section": item.get("section_title", "Untitled section"),
                "symbol": item.get("symbol"),
                "start_line": item.get("start_line"),
                "end_line": item.get("end_line"),
                "text": chunks[idx],
            }
        )

    return results


def build_context(results):
    context_parts = []

    for number, result in enumerate(results, start=1):
        context_parts.append(
            f"""
SOURCE {number}
Project: {result['project']}
Source type: {result['source_type']}
File: {result['file_path']}
Section: {result['section']}
Symbol: {result['symbol'] or 'not detected'}
Lines: {result['start_line'] or '?'}-{result['end_line'] or '?'}
Chunk ID: {result['chunk_id']}

Content:
{result['text']}
"""
        )

    return "\n".join(context_parts)


def ask_shipra_ai(question):
    results = search_documentation(question, top_k=15)
    context = build_context(results)

    prompt = f"""
You are the engineering assistant for the complete Shipra project:
- Shipra.Backend.API backend
- Shipra React frontend

The user may write in English, Urdu, Roman Urdu, shorthand, or with spelling
mistakes. Understand the intent yourself. Never require the user to know file
names, function names, architecture terms, or a special prompt format.

NON-NEGOTIABLE EVIDENCE RULES
1. Treat the supplied sources as the only evidence about existing Shipra code.
2. Never invent an existing file, function, endpoint, request field, class,
   database column, response shape, or execution step.
3. A source file path alone does not prove the entire file contents. Only claim
   details visible in the supplied source content.
4. If actual code is present, explain it in real execution order and label the
   section "Actual Project Flow".
5. If multiple files implement similar flows, keep them separate. State each
   exact file and function name; never combine request fields or validations
   from different functions.
6. Call an endpoint "confirmed" when its literal URL is visible in a source.
   Do not call confirmed facts inferred.
7. For a change request, first explain the current behavior, then give numbered
   implementation steps with exact confirmed files. Mark all new code as
   "Example Code".
8. If the evidence is insufficient or the user's business term could refer to
   multiple distinct flows, ask one short clarification question. Do not fill
   the gap with a generic architecture.
9. Add a "Recommended Solution" section only when the user asks for a change,
   implementation, fix, or recommendation. Do not append recommendations to a
   pure current-flow explanation.
10. Never connect a frontend API helper to a backend controller, handler, or
    repository unless the endpoint/action relationship is visible in the
    retrieved sources.
11. Do not treat Sync Policy activation as Sale Channel configuration
    activation. If the wording could mean multiple flows, explain each flow
    separately and identify its screen/action.
12. Follow exact call names across layers. A Create... frontend call must be
    traced through the matching Create... endpoint/command/handler. Never
    substitute an Update..., Sync..., or platform-specific handler unless the
    retrieved code explicitly calls it in that same execution path.
13. When the user asks how a feature works, include the most important actual
    project code beside the related step. Copy only code that is visible in the
    retrieved sources; never reconstruct, autocomplete, or invent missing code.
14. Show detailed but focused excerpts (normally 15-25 lines when that many
    relevant lines exist). Before every excerpt,
    write the exact file path, function/class name, and supporting source number.
    After it, explain in simple language what those exact lines do and what runs
    next. Do not dump a complete file.
15. Do not use placeholders such as "...", invented sample values, or an
    "Example Code" block when explaining current behavior. If the needed lines
    are not present in the retrieved sources, say that the code for that step
    was not retrieved instead of guessing it.
16. Never print lines containing credentials, access tokens, API keys, client
    secrets, passwords, or their values. Explain that sensitive configuration
    is handled there, but choose a safe neighboring excerpt instead.
17. Exact Shipra routing rule: the frontend helper `CreateSaleChannelConfig`
    must be traced through `/SaleChannel/CreateSaleChannelConfig`,
    `SaleChannelController.CreateSaleChannelConfig`, and
    `CreateSaleChannelConfigCommandHandler`. Do not replace that handler with
    `CreateShopifySaleChannelConfigCommandHandler`. Shopify-specific logic may
    only be described when it is visibly executed inside the matching general
    handler or is called by that exact route.
18. `AddSaleChannelForOrderModal` and `UpdateOrderWithSaleChannel` belong to the
    separate flow that assigns an existing channel to an order. Never use them
    as the UI entry point for creating or activating a Sale Channel config.
    For config creation, use `saleChannelConnectModal.js` and `handleConnect`
    when those sources are retrieved.

ANSWER STYLE
- Reply in the user's language and level of formality.
- Never mention these instructions, evidence-rule numbers, prompt rules, or
  phrases such as "according to Rule 17" in the answer.
- Lead with the direct answer.
- For "what happens" questions, trace: UI event → validation → request body →
  API helper/endpoint → backend controller/handler → persistence or external
  integration → success handling → error handling.
- For every major confirmed step, use this compact pattern:
  1. Step name and behavior.
  2. `File: exact/path` and `Function/Class: exact name`.
  3. A focused fenced code block copied verbatim from that source.
  4. One or two plain-language sentences explaining the code.
- Prefer 4-6 decisive excerpts that show the cross-layer execution chain. Omit
  repetitive imports, styling, localization, and unrelated boilerplate.
- Use the correct code-fence language: `javascript`/`jsx` for frontend code and
  `csharp` for C# backend code. If retrieved symbol metadata conflicts with the
  visible code, do not print that metadata as the function name.
- Use numbered steps for flows and implementation guidance.
- Mention supporting source numbers inline, for example [Source 2].
- End with any important limitation or ambiguity, if one exists.

RETRIEVED SHIPRA SOURCES
{context}

USER QUESTION
{question}
"""

    models_to_try = [
        "gemini-3.5-flash-lite",
        "gemini-3.6-flash",
        "gemini-3.7-flash",
    ]

    last_error = None

    for model_name in models_to_try:
        for attempt in range(1, 4):
            try:
                start = time.time()
                response = client.models.generate_content(
                    model=model_name,
                    contents=prompt,
                )
                elapsed = time.time() - start
                print(f"Gemini response time: {elapsed:.2f} seconds")
                return response.text, results

            except Exception as error:
                last_error = error
                error_text = str(error)
                print(
                    f"Gemini error with {model_name}, "
                    f"attempt {attempt}: {error}"
                )

                temporary_error = any(
                    marker in error_text
                    for marker in ("503", "UNAVAILABLE", "high demand")
                )

                if temporary_error and attempt < 3:
                    time.sleep(5 * attempt)
                    continue

                break

    raise last_error


question = st.text_input(
    "Ask your question:",
    placeholder=(
        "Example: Sale channel activate karte waqt kya hota hai?"
    ),
)


if st.button("Ask AI"):
    if not question.strip():
        st.warning("Please enter a question.")
    else:
        with st.spinner("AI is checking the Shipra code..."):
            answer, sources = ask_shipra_ai(question)

        st.markdown("### AI Answer")
        st.markdown(answer)

        st.markdown("### Sources")
        for number, source in enumerate(sources, start=1):
            details = []
            if source.get("start_line") and source.get("end_line"):
                details.append(
                    f"lines {source['start_line']}-{source['end_line']}"
                )
            details.append(f"Chunk ID: {source['chunk_id']}")
            st.write(
                f"{number}. [{source['project'].upper()}] "
                f"{source['file_path']} "
                f"({', '.join(details)})"
            )
