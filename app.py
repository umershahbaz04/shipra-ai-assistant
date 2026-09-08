import os
import json
import time
import re
import faiss
import numpy as np
import streamlit as st
from sentence_transformers import SentenceTransformer
from google import genai


# -----------------------------
# PAGE SETTINGS
# -----------------------------
st.set_page_config(
    page_title="Shipra AI Assistant",
    page_icon="🤖",
    layout="centered"
)

st.title("🤖 Shipra Full-Stack AI Assistant")
st.write("Ask questions about the Shipra frontend and backend projects.")


# -----------------------------
# LOAD API KEY
# -----------------------------
GEMINI_API_KEY = st.secrets["GEMINI_API_KEY"]

client = genai.Client(
    api_key=GEMINI_API_KEY
)


# -----------------------------
# LOAD DOCUMENTATION DATA
# -----------------------------
@st.cache_resource
def load_data():

    with open("chunks.json", "r", encoding="utf-8") as f:
        chunks = json.load(f)

    with open("metadata.json", "r", encoding="utf-8") as f:
        metadata = json.load(f)

    index = faiss.read_index("faiss.index")

    embedding_model = SentenceTransformer(
        "all-MiniLM-L6-v2"
    )

    return chunks, metadata, index, embedding_model


chunks, metadata, index, embedding_model = load_data()


# -----------------------------
# SEARCH DOCUMENTATION
# -----------------------------
def search_documentation(question, top_k=12):

    question_embedding = embedding_model.encode(
        [question],
        convert_to_numpy=True
    ).astype("float32")

    semantic_count = min(
        max(top_k * 4, 40),
        index.ntotal
    )

    distances, indices = index.search(
        question_embedding,
        semantic_count
    )

    def get_words(text):
        text = re.sub(
            r"([a-z])([A-Z])",
            r"\1 \2",
            text
        ).lower()

        words = re.findall(r"[a-z0-9]+", text)

        normalized = []

        ignored_words = {
            "yar", "hai", "hy", "kya", "kia",
            "ko", "ki", "ke", "k", "mai",
            "main", "sy", "se", "or", "aur",
            "kon", "rha", "raha", "hyn",
            "hain", "time", "wala", "wali"
        }

        for word in words:
            if word in ignored_words or len(word) < 3:
                continue

            if word.startswith("activat"):
                word = "activat"
            elif word in {"sale", "sales"}:
                word = "sale"

            normalized.append(word)

        return set(normalized)

    question_words = get_words(question)
    keyword_matches = []

    for idx, chunk in enumerate(chunks):

        searchable_text = " ".join([
            metadata[idx].get("project", ""),
            metadata[idx].get("file_path", ""),
            metadata[idx].get("section_title", ""),
            chunk
        ])

        searchable_words = get_words(searchable_text)
        keyword_score = len(
            question_words.intersection(searchable_words)
        )

        if keyword_score > 0:

            project = metadata[idx].get(
                "project",
                "backend"
            )

            file_path = metadata[idx].get(
                "file_path",
                ""
            )

            actual_code_bonus = 0

            if project == "frontend":
                actual_code_bonus += 10

            if any(
                word in file_path.lower()
                for word in [
                    "modal",
                    "api",
                    "service",
                    "controller"
                ]
            ):
                actual_code_bonus += 5

            keyword_matches.append(
                (
                    keyword_score,
                    actual_code_bonus,
                    idx
                )
            )

    keyword_matches.sort(
        key=lambda item: (
            item[1],
            item[0]
        ),
        reverse=True
    )
    selected_indices = []

    for score, bonus, idx in keyword_matches:
        if idx not in selected_indices:
            selected_indices.append(idx)

        if len(selected_indices) >= top_k:
            break

    for idx in indices[0]:
        idx = int(idx)

        if idx not in selected_indices:
            selected_indices.append(idx)

        if len(selected_indices) >= top_k:
            break

    distance_map = {
        int(idx): float(distance)
        for distance, idx in zip(
            distances[0],
            indices[0]
        )
    }

    results = []

    for idx in selected_indices:

        results.append({
            "chunk_id": idx,
            "distance": distance_map.get(idx),
            "project": metadata[idx].get(
                "project",
                "backend"
            ),
            "file_path": metadata[idx].get(
                "file_path",
                "Shipra.Backend.API documentation"
            ),
            "section": metadata[idx]["section_title"],
            "text": chunks[idx]
        })

    return results


# -----------------------------
# ASK AI
# -----------------------------
def ask_shipra_ai(question):

    # Search relevant documentation
    results = search_documentation(
        question,
        top_k=12
    )

    # Prepare context
    context_parts = []

    for i, result in enumerate(results, start=1):

        context_parts.append(
    f"""
SOURCE {i}

Project:
{result['project']}

File:
{result['file_path']}

Section:
{result['section']}

Chunk ID:
{result['chunk_id']}

Content:
{result['text']}
"""
)

    context = "\n".join(context_parts)

    # -----------------------------
    # PROMPT
    # -----------------------------
    prompt = f"""
You are the AI assistant for the complete Shipra project.

The project contains:
- Shipra.Backend.API backend
- Shipra React frontend

Use the retrieved frontend and backend project information as the PRIMARY SOURCE.

When answering:
- If multiple files implement similar flows, do not merge their logic.
- Explain each implementation separately using its exact file and function name.
- For request bodies, copy only fields confirmed in the same function's retrieved code.
- State clearly when two components represent different or legacy flows.
- First inspect all retrieved sources for actual implementation code.
- If actual implementation code is available, explain that code in its real execution order.
- Do not replace existing project logic with a recommended architecture.
- For flow questions, start from the UI event or function call and follow each API call and response.
- Mention the exact function names, request-body fields, API endpoint, success action, and error handling.
- Only provide a Recommended Solution when no relevant actual code exists in any retrieved source.
- Identify whether the question concerns frontend, backend, or both.
- Mention the actual project and file paths used.
- For requested changes, provide numbered step-by-step instructions.
- Explain how frontend components connect with backend APIs when relevant.
- Clearly separate existing project code from recommended code.
- Never invent files, components, endpoints, classes, or methods.

IMPORTANT RULES:

1. If the documentation contains the answer:
   - Answer based on the project documentation.
   - Mention the relevant file, class, method, or business logic when available.
   - If actual project code is present, label it as "Actual Project Code".

2. If the documentation does NOT contain enough information:
   - Do NOT simply refuse to answer.
   - Use your general software engineering and programming knowledge to suggest the most suitable solution.
   - Clearly label this section as "Recommended Solution".
   - Any new code you create must be labeled as "Example Code".

3. Never present assumptions or recommendations as existing project facts.

4. Clearly separate confirmed information from recommendations.

5. When suggesting a solution:
   - Consider the existing Shipra.Backend.API architecture.
   - Prefer the architecture and technologies mentioned in the documentation.
   - Suggest appropriate files, classes, methods, or layers where possible.

6. When providing code:
   - Give practical and realistic code.
   - Label code not found in the documentation as "Example Code".

7. Explain technical concepts in simple language.

8. If multiple approaches are possible:
   - Recommend the most suitable approach first.

9. Keep the answer focused on the user's question.

10. Do not invent existing project classes, methods, database columns, or files.
    If something is assumed for an example, clearly say it is an assumption.

CONTEXT FROM PROJECT DOCUMENTATION:

{context}

USER QUESTION:

{question}
"""

    # -----------------------------
    # GEMINI MODEL
    # -----------------------------
    models_to_try = [
        "gemini-3.5-flash-lite",
       "gemini-3.6-flash",
      "gemini-3.7-flash"
    ]

    last_error = None

    for model_name in models_to_try:

        for attempt in range(1, 4):

            try:
                start = time.time()

                response = client.models.generate_content(
                    model=model_name,
                    contents=prompt
                )

                gemini_time = time.time() - start

                print(
                    f"Gemini response time: "
                    f"{gemini_time:.2f} seconds"
                )

                return response.text, results

            except Exception as e:
                last_error = e
                error_text = str(e)

                print(
                    f"Gemini error with {model_name}, "
                    f"attempt {attempt}: {e}"
                )

                is_temporary_error = (
                    "503" in error_text
                    or "UNAVAILABLE" in error_text
                    or "high demand" in error_text
                )

                if is_temporary_error and attempt < 3:
                    time.sleep(5 * attempt)
                    continue

                break

    raise last_error


# -----------------------------
# CHAT INPUT
# -----------------------------
question = st.text_input(
    "Ask your question:",
    placeholder="Example: Order create hone par stock kaise reserve hota hai?"
)


# -----------------------------
# ASK BUTTON
# -----------------------------
if st.button("Ask AI"):

    if not question.strip():

        st.warning("Please enter a question.")

    else:

        with st.spinner("AI is thinking..."):

            answer, sources = ask_shipra_ai(question)

        st.markdown("### AI Answer")

        st.markdown(answer)

        st.markdown("### Sources")

        for i, source in enumerate(sources, start=1):

            st.write(
    f"{i}. [{source['project'].upper()}] "
    f"{source['file_path']} "
    f"(Chunk ID: {source['chunk_id']})"
)
