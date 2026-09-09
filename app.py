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
        file_path = item.get(
            "file_path",
            "Shipra.Backend.API documentation",
        )
        display_symbol = item.get("symbol")

        # A 120-line chunk may contain many methods, so the indexer's first
        # detected symbol is not always the method relevant to the matched
        # call. Correct deterministic, path-confirmed entry points before they
        # are shown to the model.
        if file_path.endswith(
            "/CreateSaleChannelConfig/CreateSaleChannelConfigCommandHandler.cs"
        ):
            display_symbol = (
                "CreateSaleChannelConfigCommandHandler.HandleRequest"
            )
        elif file_path.endswith("/Api/SaleChannelController.cs") and (
            "CreateSaleChannelConfig" in chunks[idx]
        ):
            display_symbol = "SaleChannelController.CreateSaleChannelConfig"
        elif file_path.endswith("/api/AxiosInterceptors.js") and (
            "CreateSaleChannelConfig" in chunks[idx]
        ):
            display_symbol = "CreateSaleChannelConfig"
        elif file_path.endswith("/saleChannelConnectModal.js") and (
            "handleConnect" in chunks[idx]
        ):
            display_symbol = "SaleChannelConnectModal.handleConnect"
        elif file_path.endswith("/SaleChannelConfigRepository.cs") and (
            "CreateSaleChannelConfig" in chunks[idx]
        ):
            display_symbol = "SaleChannelConfigRepository.CreateSaleChannelConfig"

        results.append(
            {
                "chunk_id": idx,
                "distance": distance_map.get(idx),
                "project": item.get("project", "backend"),
                "source_type": item.get("source_type", "documentation"),
                "file_path": file_path,
                "section": item.get("section_title", "Untitled section"),
                "symbol": display_symbol,
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
Exact-code placeholder: [[CODE_SOURCE_{number}]]

Content:
{result['text']}
"""
        )

    return "\n".join(context_parts)


def code_language(file_path):
    extension = file_path.rsplit(".", 1)[-1].lower()
    return {
        "cs": "csharp",
        "cshtml": "csharp",
        "js": "javascript",
        "jsx": "jsx",
        "ts": "typescript",
        "tsx": "tsx",
        "json": "json",
        "sql": "sql",
        "html": "html",
        "css": "css",
        "scss": "scss",
        "yml": "yaml",
        "yaml": "yaml",
    }.get(extension, "text")


def snippet_anchor_candidates(result):
    """Return path-confirmed anchors ordered by usefulness."""
    path = result["file_path"].lower()
    start_line = result.get("start_line") or 1

    if path.endswith("/salechannelconnectmodal.js"):
        if start_line <= 50:
            return ["const handleConnect", "let body = {", "CreateSaleChannelConfig"]
        return ["let body = {", "CreateSaleChannelConfig", "const handleConnect"]
    if path.endswith("/api/axiosinterceptors.js"):
        return ["export const CreateSaleChannelConfig", "CreateSaleChannelConfig"]
    if path.endswith("/api/salechannelcontroller.cs"):
        return ['[HttpPost("CreateSaleChannelConfig")]', "CreateSaleChannelConfig"]
    if path.endswith(
        "/createsalechannelconfig/createsalechannelconfigcommandhandler.cs"
    ):
        return [
            "var oSaleChannelConfig = await",
            "protected override async Task<ServiceResultDTO> HandleRequest",
            "UpdateSaleChannelConfigWhileActivate",
        ]
    if path.endswith("/salechannelconfigrepository.cs"):
        return [
            "public async Task<SaleChannelConfig> CreateSaleChannelConfig",
            "CreateSaleChannelConfig(",
        ]
    if path.endswith("/salechannelconfig.cs"):
        return [
            "public void UpdateSaleChannelConfigWhileActivate",
            "public static SaleChannelConfig CreateSaleChannelConfig",
        ]

    symbol = result.get("symbol") or ""
    candidates = [symbol.split(".")[-1]] if symbol else []
    return [candidate for candidate in candidates if candidate]


def extract_exact_snippet(result, question, maximum_lines=24):
    """Select a useful contiguous excerpt without asking the model to copy it."""
    lines = result["text"].splitlines()
    if not lines:
        return ""

    anchor_index = None
    used_path_confirmed_anchor = False
    for candidate in snippet_anchor_candidates(result):
        for line_index, line in enumerate(lines):
            if candidate in line:
                anchor_index = line_index
                used_path_confirmed_anchor = True
                break
        if anchor_index is not None:
            break

    if anchor_index is None:
        query_tokens = tokenize(question)
        symbol_tokens = tokenize(result.get("symbol") or "")
        best_score = -1
        anchor_index = 0

        for line_index, line in enumerate(lines):
            line_tokens = tokenize(line)
            score = (
                len(query_tokens.intersection(line_tokens)) * 4
                + len(symbol_tokens.intersection(line_tokens)) * 3
            )
            if any(
                marker in line
                for marker in ("public ", "private ", "const ", "await ", "return ")
            ):
                score += 1
            if score > best_score:
                best_score = score
                anchor_index = line_index

    # Known entry-point anchors start exactly on the matched source line. The
    # generic fallback includes two setup lines for local context.
    excerpt_start = (
        anchor_index
        if used_path_confirmed_anchor
        else max(0, anchor_index - 2)
    )
    excerpt_end = min(len(lines), excerpt_start + maximum_lines)
    selected = lines[excerpt_start:excerpt_end]

    # Remove only blank edges. Interior lines and every code character remain
    # byte-for-byte identical to the indexed source chunk.
    while selected and not selected[0].strip():
        selected.pop(0)
    while selected and not selected[-1].strip():
        selected.pop()

    return "\n".join(selected)


def build_code_cards(results, question):
    cards = {}

    for source_number, result in enumerate(results, start=1):
        if result.get("source_type") != "actual_code":
            continue

        snippet = extract_exact_snippet(result, question)
        if not snippet:
            continue

        language = code_language(result["file_path"])
        symbol = result.get("symbol") or "Not detected"
        cards[source_number] = (
            f"\n**File:** `{result['file_path']}`  \n"
            f"**Function/Class:** `{symbol}`  \n"
            f"```{language}\n{snippet}\n```\n"
        )

    return cards


def inject_verified_code(answer, code_cards, minimum_cards=4):
    """Remove model-written code and inject only exact source excerpts."""
    # The model is never trusted to reproduce source code or labels.
    answer = re.sub(r"```[A-Za-z0-9_+-]*\s*\n.*?```", "", answer, flags=re.DOTALL)
    answer = re.sub(
        r"(?mi)^\s*(?:\*{0,2})?(?:File|Function/Class|Function|Symbol):.*$",
        "",
        answer,
    )
    answer = re.sub(r"\n{3,}", "\n\n", answer).strip()

    used_sources = []

    def replace_placeholder(match):
        source_number = int(match.group(1))
        card = code_cards.get(source_number)
        if not card:
            return ""
        if source_number not in used_sources:
            used_sources.append(source_number)
        return card

    answer = re.sub(
        r"\[\[CODE_SOURCE_(\d+)\]\]",
        replace_placeholder,
        answer,
    )

    # If the model omitted too many markers, append deterministic cards from
    # the strongest retrieval anchors instead of generating or guessing code.
    if len(used_sources) < minimum_cards:
        missing = [
            source_number
            for source_number in code_cards
            if source_number not in used_sources
        ][: minimum_cards - len(used_sources)]

        if missing:
            answer = answer.rstrip() + "\n\n### Verified Project Code\n"
            for source_number in missing:
                answer += (
                    f"\n[Source {source_number}]"
                    f"{code_cards[source_number]}"
                )

    return answer.strip()


def ask_shipra_ai(question):
    results = search_documentation(question, top_k=15)
    context = build_context(results)
    code_cards = build_code_cards(results, question)
    question_tokens = tokenize(question)
    flow_requested = bool(
        question_tokens.intersection(
            {"connect", "create", "update", "delete", "validate", "flow"}
        )
    )
    sale_channel_create_flow = {
        "sale",
        "channel",
        "connect",
        "create",
    }.issubset(question_tokens)
    desired_cards = 6 if sale_channel_create_flow else 4
    minimum_code_cards = (
        min(desired_cards, len(code_cards)) if flow_requested else 0
    )

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
13. Never write, quote, recreate, or fence source code yourself. Exact code is
    inserted later by the application. To place code after a step, output only
    the supplied marker for that source, for example [[CODE_SOURCE_2]]. Put the
    marker on its own line and never alter its spelling or number.
14. Use 4-6 code markers for a cross-layer flow when matching actual-code
    sources exist. Place each marker immediately after the step it supports.
    Do not write File, Function, Class, or Symbol labels; the application adds
    verified labels with the exact snippet.
15. Do not use "...", invented sample values, or an "Example Code" block when
    explaining current behavior. If evidence is missing, say so plainly.
16. Do not expose credential values or claim that a credential is valid unless
    the retrieved execution path visibly performs that validation.
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
19. Never output Markdown code fences. Use only exact-code placeholders. The
    application—not the model—owns all code, path, and function rendering.
20. Explain conditional branches independently. A method call inside an `else`
    block proves behavior only for that branch. Do not claim the `if` branch
    performs the same activation unless its visible lines also call the
    activation method.
21. The matching backend class/function label for this route is
    `CreateSaleChannelConfigCommandHandler.HandleRequest`. Never label it from
    incidental metadata such as `StatusCode`. Preserve the exact method name
    `UpdateSaleChannelConfigWhileActivate`; do not alter it with spaces or
    underscores.
22. For a Sale Channel create/activate explanation, cover the retrieved chain
    in this order: `handleConnect` validation and body/response handling;
    Axios helper; controller action; general command handler; repository save;
    and domain activation flag. Do not omit repository/domain behavior when
    those sources are present.
23. In the shown Shopify branch, an existing Shopify config is updated in the
    `if` branch. The visible activation call occurs in the `else` branch after
    creating a new Shopify config. State this distinction exactly and do not
    summarize both branches as automatically activating the channel.

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
  2. Supporting source number inline.
  3. The matching [[CODE_SOURCE_N]] marker on its own line.
  4. One or two plain-language sentences explaining what that source proves.
- Prefer 4-6 decisive excerpts that show the cross-layer execution chain. Omit
  repetitive imports, styling, localization, and unrelated boilerplate.
- Never type a code fence or manually type a file/function label.
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
                verified_answer = inject_verified_code(
                    response.text,
                    code_cards,
                    minimum_cards=minimum_code_cards,
                )
                return verified_answer, results

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
