import io
import json
import os
import shutil
import tempfile
import urllib.request
import zipfile
from pathlib import Path

import faiss
import numpy as np
from sentence_transformers import SentenceTransformer


FRONTEND_ZIP_URL = (
    "https://github.com/umershahbaz04/"
    "shipra-frontend/archive/refs/heads/main.zip"
)

SUPPORTED_EXTENSIONS = {
    ".js", ".jsx", ".ts", ".tsx",
    ".json", ".css", ".md", ".html"
}

IGNORED_FOLDERS = {
    "node_modules",
    "build",
    "dist",
    ".git",
    "coverage"
}

IGNORED_FILES = {
    "package-lock.json"
}

MAX_FILE_SIZE = 300_000
CHUNK_SIZE = 1800
CHUNK_OVERLAP = 200


def download_frontend():
    print("Downloading frontend from GitHub...")

    request = urllib.request.Request(
        FRONTEND_ZIP_URL,
        headers={"User-Agent": "Shipra-AI-Indexer"}
    )

    with urllib.request.urlopen(request) as response:
        return response.read()


def split_text(text):
    chunks = []
    start = 0

    while start < len(text):
        end = start + CHUNK_SIZE
        chunk = text[start:end].strip()

        if chunk:
            chunks.append(chunk)

        start = end - CHUNK_OVERLAP

    return chunks


def collect_frontend_chunks(frontend_root):
    frontend_chunks = []
    frontend_metadata = []

    for file_path in frontend_root.rglob("*"):
        if not file_path.is_file():
            continue

        relative_path = file_path.relative_to(frontend_root)

        if any(folder in relative_path.parts for folder in IGNORED_FOLDERS):
            continue

        if file_path.name in IGNORED_FILES:
            continue

        if file_path.suffix.lower() not in SUPPORTED_EXTENSIONS:
            continue

        if file_path.stat().st_size > MAX_FILE_SIZE:
            continue

        try:
            content = file_path.read_text(
                encoding="utf-8",
                errors="ignore"
            )
        except Exception:
            continue

        for chunk_number, chunk in enumerate(split_text(content)):
            frontend_chunks.append(chunk)

            frontend_metadata.append({
                "project": "frontend",
                "file_path": str(relative_path).replace("\\", "/"),
                "section_title": f"Frontend: {relative_path}",
                "chunk_index": chunk_number
            })

    return frontend_chunks, frontend_metadata


def main():
    project_root = Path(__file__).parent

    chunks_path = project_root / "chunks.json"
    metadata_path = project_root / "metadata.json"

    with chunks_path.open("r", encoding="utf-8") as file:
        backend_chunks = json.load(file)

    with metadata_path.open("r", encoding="utf-8") as file:
        backend_metadata = json.load(file)

    for item in backend_metadata:
        item.setdefault("project", "backend")
        item.setdefault("file_path", "Shipra.Backend.API documentation")

    zip_data = download_frontend()
    temporary_directory = Path(tempfile.mkdtemp())

    try:
        with zipfile.ZipFile(io.BytesIO(zip_data)) as archive:
            archive.extractall(temporary_directory)

        extracted_folders = [
            path for path in temporary_directory.iterdir()
            if path.is_dir()
        ]

        frontend_root = extracted_folders[0]

        frontend_chunks, frontend_metadata = collect_frontend_chunks(
            frontend_root
        )

        all_chunks = backend_chunks + frontend_chunks
        all_metadata = backend_metadata + frontend_metadata

        print(f"Backend chunks: {len(backend_chunks)}")
        print(f"Frontend chunks: {len(frontend_chunks)}")
        print(f"Total chunks: {len(all_chunks)}")

        model = SentenceTransformer("all-MiniLM-L6-v2")

        embeddings = model.encode(
            all_chunks,
            convert_to_numpy=True,
            show_progress_bar=True
        ).astype("float32")

        index = faiss.IndexFlatL2(embeddings.shape[1])
        index.add(embeddings)

        with chunks_path.open("w", encoding="utf-8") as file:
            json.dump(all_chunks, file, ensure_ascii=False, indent=2)

        with metadata_path.open("w", encoding="utf-8") as file:
            json.dump(all_metadata, file, ensure_ascii=False, indent=2)

        faiss.write_index(
            index,
            str(project_root / "faiss.index")
        )

        print("Knowledge index created successfully.")

    finally:
        shutil.rmtree(temporary_directory, ignore_errors=True)


if __name__ == "__main__":
    main()