import os
import json
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

st.title("🤖 Shipra Backend AI Assistant")
st.write("Ask questions about the Shipra.Backend.API project.")


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
def search_documentation(question, top_k=5):

    question_embedding = embedding_model.encode(
        [question],
        convert_to_numpy=True
    ).astype("float32")

    distances, indices = index.search(
        question_embedding,
        top_k
    )

    results = []

    for distance, idx in zip(distances[0], indices[0]):

        results.append({
            "chunk_id": int(idx),
            "distance": float(distance),
            "section": metadata[idx]["section_title"],
            "text": chunks[idx]
        })

    return results


# -----------------------------
# ASK AI
# -----------------------------
def ask_shipra_ai(question):

    results = search_documentation(
        question,
        top_k=5
    )

    context_parts = []

    for i, result in enumerate(results, start=1):

        context_parts.append(
            f"""
SOURCE {i}

Section:
{result['section']}

Chunk ID:
{result['chunk_id']}

Content:
{result['text']}
"""
        )

    context = "\n".join(context_parts)

    prompt = f"""
You are the AI assistant for the Shipra.Backend.API project.

Answer the user's question using ONLY the project documentation
provided in the CONTEXT below.

IMPORTANT RULES:

1. Do not invent project details.
2. Do not assume code that is not present in the context.
3. If the documentation does not contain enough information,
   clearly say:
   "Documentation mein is question ka complete answer nahi mila."
4. If actual project code is present, clearly identify it as
   "Actual Project Code".
5. If you create an example yourself, clearly label it as
   "Example Code".
6. Explain technical concepts in simple language.
7. Mention the relevant file/class/method when available.
8. Keep the answer focused on the user's question.

CONTEXT FROM PROJECT DOCUMENTATION:

{context}

USER QUESTION:

{question}
"""

    models_to_try = [
    "gemini-3.6-flash",
    "gemini-3.5-flash",
    "gemini-3.5-flash-lite",
    "gemini-2.5-flash-lite",
    "gemini-flash-lite-latest"
]

last_error = None

for model_name in models_to_try:
    try:
        response = client.models.generate_content(
            model=model_name,
            contents=prompt
        )

        return response.text, results

    except Exception as e:
        last_error = e
        continue

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
                f"{i}. {source['section']} "
                f"(Chunk ID: {source['chunk_id']})"
            )
