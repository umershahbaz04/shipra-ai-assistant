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

        score = (lexical_score * 3.0) + path_score + semantic_score + project_score

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

    if link_identifiers:
        linked_scores = []
        for score, idx in scored:
            item = metadata[idx]
            searchable = "\n".join(
                [
                    item.get("file_path", ""),
                    item.get("symbol") or "",
                    chunks[idx],
                ]
            )
            exact_links = sum(
                identifier in searchable
                for identifier in link_identifiers
            )
            linked_scores.append((score + (exact_links * 40.0), idx))

        scored = sorted(
            linked_scores,
            key=lambda item: item[0],
            reverse=True,
        )

    # For flow questions, select evidence across distinct project layers. This
    # prevents a similarly named update/sync feature from crowding out the real
    # frontend -> controller -> handler -> repository -> entity chain.
    anchor_indices = []
    anchors_per_file = Counter()
    used_categories = set()

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
9. Recommendations are allowed only after confirmed facts, under a separate
   "Recommended Solution" heading.
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

ANSWER STYLE
- Reply in the user's language and level of formality.
- Lead with the direct answer.
- For "what happens" questions, trace: UI event → validation → request body →
  API helper/endpoint → success handling → error handling.
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
