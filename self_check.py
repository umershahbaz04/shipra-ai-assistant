from __future__ import annotations

import ast
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parent
APP = ROOT / "app.py"
SERVER = ROOT / "server.py"

def check_syntax(path: Path) -> None:
    compile(path.read_text(encoding="utf-8"), str(path), "exec")

def extract_router():
    tree = ast.parse(APP.read_text(encoding="utf-8"))
    fn = next(
        node for node in tree.body
        if isinstance(node, ast.FunctionDef)
        and node.name == "detect_request_profile"
    )
    module = ast.Module(body=[fn], type_ignores=[])
    ns = {
        "re": re,
        "GENERAL": "general",
        "PROJECT_EXISTING": "project_existing",
        "PROJECT_CHANGE": "project_change",
        "PROJECT_PROMPT": "project_prompt",
    }
    exec(compile(module, "<router-test>", "exec"), ns)
    return ns["detect_request_profile"]

def source_health():
    candidates = [
        ROOT / "project-source",
        ROOT / "Project-Source",
        ROOT,
    ]
    extensions = {".cs", ".js", ".jsx", ".ts", ".tsx"}
    excluded = {
        ".git", ".venv", "venv", "node_modules",
        "bin", "obj", "dist", "build", "__pycache__",
    }

    best = None
    best_count = -1

    for candidate in candidates:
        if not candidate.is_dir():
            continue

        count = 0
        for path in candidate.rglob("*"):
            if not path.is_file():
                continue
            if any(part.lower() in excluded for part in path.parts):
                continue
            if path.suffix.lower() in extensions:
                count += 1

        if count > best_count:
            best = candidate
            best_count = count

    return best, max(best_count, 0)

def main():
    check_syntax(APP)
    check_syntax(SERVER)

    app_text = APP.read_text(encoding="utf-8")
    server_text = SERVER.read_text(encoding="utf-8")

    assert "mcp.run()" not in app_text, "FAIL: app.py must not call mcp.run()"
    assert "mcp.run()" in server_text, "FAIL: server.py must call mcp.run()"

    router = extract_router()

    cases = [
        ("how to create dashboard?", "project_existing"),
        ("how to create dashboard in shipra?", "project_existing"),
        ("return order kesy banau shipra mai?", "project_existing"),
        ("how to create shipra stores?", "project_existing"),
        ("connect Shopify sale channel", "project_existing"),
        ("Which COD orders are not delivered yet?", "project_existing"),
        ("show Priority orders", "project_existing"),
        ("how to assign order label?", "project_existing"),
        ("create order label", "project_existing"),
        ("export order labels csv", "project_existing"),
        ("how to filter price calculator?", "project_existing"),
        ("how to upload orders?", "project_existing"),
        ("track shipment", "project_existing"),
        ("show carrier dashboard", "project_existing"),
        ("how to add product?", "project_existing"),
        ("how to update inventory?", "project_existing"),
        ("how to create customer?", "project_existing"),
        ("how to create lead?", "project_existing"),
        ("how to delete contact?", "project_existing"),
        ("change app.py to add delete chat", "project_change"),
        ("modify code for dashboard", "project_change"),
        ("implement feature for bulk export", "project_change"),
        ("create api for customer", "project_change"),
        ("generate coding prompt for return order feature", "project_prompt"),
        ("Explain Python decorators", "general"),
        ("how to build dashboard in Power BI", "general"),
        ("create dashboard in Excel", "general"),
        ("React me dashboard kaise banaye", "general"),
        ("what is inventory", "general"),
        ("what is a carrier", "general"),
    ]

    failures = []
    for question, expected in cases:
        actual = router(question)["mode"]
        if actual != expected:
            failures.append((question, actual, expected))

    if failures:
        for failure in failures:
            print("ROUTER FAIL:", failure)
        raise SystemExit(1)

    source_root, source_count = source_health()

    print("PASS: app.py syntax")
    print("PASS: server.py syntax")
    print("PASS: MCP server starts only from server.py")
    print(f"PASS: {len(cases)} routing regression scenarios")
    print(f"Source candidate: {source_root}")
    print(f"Searchable raw source files: {source_count}")

    if source_count == 0:
        print(
            "WARNING: No raw .cs/.js/.jsx/.ts/.tsx project source is deployed. "
            "The app will correctly report a source-health problem instead of "
            "inventing Shipra workflows."
        )

if __name__ == "__main__":
    main()
