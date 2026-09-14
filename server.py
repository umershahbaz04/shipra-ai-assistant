import json
import os
import re
from pathlib import Path
from typing import Iterable

from mcp.server.mcpserver import MCPServer


APP_ROOT = Path(__file__).resolve().parent
SOURCE_EXTENSIONS = {".cs", ".js", ".jsx", ".ts", ".tsx"}
EXCLUDED_DIRECTORIES = {
    ".git",
    ".venv",
    "venv",
    "node_modules",
    "bin",
    "obj",
    "dist",
    "build",
    "__pycache__",
}
EXCLUDED_SUFFIXES = (".min.js", ".generated.cs", ".designer.cs")
KNOWN_PROJECT_FOLDERS = {
    "Shipra.Frontend",
    "Shipra.Backend.API.Web",
    "Shipra.Backend.API.Application",
    "Shipra.Backend.API.Infrastructure",
    "Shipra.Backend.API.Core",
}
MAX_SOURCE_FILE_BYTES = 2_000_000


def _root_score(path: Path) -> int:
    if not path.is_dir():
        return -1

    score = sum(
        10
        for folder_name in KNOWN_PROJECT_FOLDERS
        if (path / folder_name).is_dir()
    )

    if (path / "mock-data" / "orders.json").is_file():
        score += 2

    return score


def _discover_project_root() -> Path:
    """Resolve the directory that directly contains the Shipra source folders."""
    configured_root = os.getenv("SHIPRA_PROJECT_ROOT", "").strip()

    if configured_root:
        configured = Path(configured_root).expanduser()
        if not configured.is_absolute():
            configured = APP_ROOT / configured
        return configured.resolve()

    candidates: list[Path] = [
        APP_ROOT / "project-source",
        APP_ROOT / "Project-Source",
        APP_ROOT,
    ]

    # Also detect a nested checkout without walking outside the deployed app.
    try:
        for path in APP_ROOT.rglob("*"):
            if not path.is_dir():
                continue

            try:
                relative = path.relative_to(APP_ROOT)
            except ValueError:
                continue

            if len(relative.parts) > 4:
                continue

            if any(part.lower() in EXCLUDED_DIRECTORIES for part in relative.parts):
                continue

            if path.name in KNOWN_PROJECT_FOLDERS:
                candidates.append(path.parent)
    except OSError:
        pass

    unique_candidates: list[Path] = []
    seen: set[str] = set()

    for candidate in candidates:
        try:
            resolved = candidate.resolve()
        except OSError:
            continue

        key = str(resolved)
        if key not in seen:
            seen.add(key)
            unique_candidates.append(resolved)

    ranked = sorted(
        unique_candidates,
        key=lambda path: (_root_score(path), -len(path.parts)),
        reverse=True,
    )

    if ranked and _root_score(ranked[0]) > 0:
        return ranked[0]

    # Preserve the conventional path so diagnostics remain understandable.
    return (APP_ROOT / "project-source").resolve()


PROJECT_ROOT = _discover_project_root()
mcp = MCPServer("Shipra Code Assistant")


def _is_allowed_source(path: Path) -> bool:
    if path.suffix.lower() not in SOURCE_EXTENSIONS:
        return False
    if path.name.lower().endswith(EXCLUDED_SUFFIXES):
        return False
    return True


def _iter_source_files() -> Iterable[Path]:
    if not PROJECT_ROOT.is_dir():
        return

    for current_dir, directories, filenames in os.walk(
        PROJECT_ROOT,
        followlinks=False,
    ):
        current = Path(current_dir)
        directories[:] = sorted(
            name
            for name in directories
            if name.lower() not in EXCLUDED_DIRECTORIES
            and not (current / name).is_symlink()
            and not (current / name).is_junction()
        )

        for filename in sorted(filenames):
            path = current / filename
            if not _is_allowed_source(path):
                continue
            if path.is_symlink():
                continue
            yield path


def _safe_relative(path: Path) -> str | None:
    try:
        return path.resolve().relative_to(PROJECT_ROOT.resolve()).as_posix()
    except (OSError, ValueError):
        return None


def _read_source_text(path: Path) -> str | None:
    try:
        resolved = path.resolve()
        resolved.relative_to(PROJECT_ROOT.resolve())
        if not resolved.is_file():
            return None
        if resolved.stat().st_size > MAX_SOURCE_FILE_BYTES:
            return None
        return resolved.read_text(encoding="utf-8-sig")
    except (OSError, UnicodeError, ValueError):
        return None


def _source_diagnostic() -> dict:
    sample_files: list[str] = []
    total_source_files = 0

    for path in _iter_source_files() or []:
        total_source_files += 1
        if len(sample_files) < 25:
            relative = _safe_relative(path)
            if relative:
                sample_files.append(relative)

    folders = []
    if PROJECT_ROOT.is_dir():
        try:
            folders = sorted(
                child.name
                for child in PROJECT_ROOT.iterdir()
                if child.is_dir()
                and not child.is_symlink()
                and not child.name.startswith(".")
                and child.name.lower() not in EXCLUDED_DIRECTORIES
            )
        except OSError:
            folders = []

    return {
        "project_root": str(PROJECT_ROOT),
        "project_root_exists": PROJECT_ROOT.is_dir(),
        "raw_source_ready": total_source_files > 0,
        "total_source_files": total_source_files,
        "top_level_folders": folders,
        "sample_files": sample_files,
        "frontend_found": (PROJECT_ROOT / "Shipra.Frontend").is_dir(),
        "backend_web_found": (PROJECT_ROOT / "Shipra.Backend.API.Web").is_dir(),
        "backend_application_found": (
            PROJECT_ROOT / "Shipra.Backend.API.Application"
        ).is_dir(),
        "configured_root": os.getenv("SHIPRA_PROJECT_ROOT", "").strip() or None,
    }


def _search_literal(query: str, max_results: int) -> tuple[list[dict], int, int]:
    matches: list[dict] = []
    skipped_files = 0
    searchable_files = 0
    search_text = query.casefold()

    for path in _iter_source_files() or []:
        searchable_files += 1
        relative = _safe_relative(path)
        if not relative:
            skipped_files += 1
            continue

        # Filename/path matches matter for feature pages such as returnOrders/index.js
        # even when the literal feature phrase is not repeated in the file body.
        if search_text in relative.casefold():
            matches.append({
                "file_path": relative,
                "line_number": 1,
                "match_kind": "path",
                "line_text": "",
            })
            if len(matches) >= max_results:
                return matches, skipped_files, searchable_files

        text = _read_source_text(path)
        if text is None:
            skipped_files += 1
            continue

        for line_number, line in enumerate(text.splitlines(), start=1):
            if search_text in line.casefold():
                matches.append({
                    "file_path": relative,
                    "line_number": line_number,
                    "match_kind": "content",
                    "line_text": line.strip()[:500],
                })
                if len(matches) >= max_results:
                    return matches, skipped_files, searchable_files

    return matches, skipped_files, searchable_files


def _find_orders_file() -> Path | None:
    candidates = [
        PROJECT_ROOT / "mock-data" / "orders.json",
        APP_ROOT / "project-source" / "mock-data" / "orders.json",
        APP_ROOT / "mock-data" / "orders.json",
    ]

    for candidate in candidates:
        try:
            if candidate.is_file():
                return candidate.resolve()
        except OSError:
            continue

    return None


def _load_mock_orders() -> tuple[list[dict] | None, str | None]:
    path = _find_orders_file()
    if path is None:
        return None, None

    try:
        data = json.loads(path.read_text(encoding="utf-8-sig"))
    except (OSError, UnicodeError, json.JSONDecodeError):
        return None, str(path)

    if isinstance(data, dict):
        if isinstance(data.get("orders"), list):
            data = data["orders"]
        else:
            data = [data]

    if not isinstance(data, list):
        return [], str(path)

    return [item for item in data if isinstance(item, dict)], str(path)


@mcp.tool()
def debug_source_root() -> dict:
    """Diagnose project-root discovery and raw source visibility."""
    diagnostic = _source_diagnostic()
    return {
        "status": "ok" if diagnostic["project_root_exists"] else "error",
        **diagnostic,
    }


@mcp.tool()
def get_project_structure() -> dict:
    """List Shipra source folders and report whether raw code is searchable."""
    diagnostic = _source_diagnostic()

    if not diagnostic["project_root_exists"]:
        return {
            "status": "error",
            "message": "Resolved Shipra source root does not exist.",
            **diagnostic,
        }

    return {
        "status": "ok",
        "folders": diagnostic["top_level_folders"],
        "folder_count": len(diagnostic["top_level_folders"]),
        **diagnostic,
    }


@mcp.tool()
def search_code(query: str, max_results: int = 20) -> dict:
    """Search both source paths and literal source text."""
    query = str(query or "").strip()

    if len(query) < 3:
        return {
            "status": "error",
            "message": "Enter at least 3 characters.",
        }

    diagnostic = _source_diagnostic()
    if not diagnostic["project_root_exists"]:
        return {
            "status": "error",
            "message": "Resolved Shipra source root does not exist.",
            **diagnostic,
        }

    max_results = max(1, min(int(max_results), 50))
    matches, skipped_files, searchable_files = _search_literal(
        query,
        max_results,
    )

    response = {
        "status": "ok",
        "query": query,
        "matches": matches,
        "limit_reached": len(matches) >= max_results,
        "skipped_files": skipped_files,
        "searchable_files": searchable_files,
        "project_root": str(PROJECT_ROOT),
        "raw_source_ready": searchable_files > 0,
    }

    if searchable_files == 0:
        response["diagnostic"] = (
            "No .cs/.js/.jsx/.ts/.tsx files are visible under PROJECT_ROOT. "
            "Deploy the raw Shipra source folders or set SHIPRA_PROJECT_ROOT."
        )
    elif not matches:
        response["diagnostic"] = (
            "Raw source is visible, but this literal query was not found in "
            "either file paths or file contents."
        )

    return response


@mcp.tool()
def read_file(
    file_path: str,
    start_line: int = 1,
    end_line: int = 120,
) -> dict:
    """Read a bounded line range from an allowed Shipra source file."""
    root = PROJECT_ROOT.resolve()
    requested = str(file_path or "").strip().replace("\\", "/")
    relative = Path(requested)

    if relative.is_absolute() or relative.drive or ".." in relative.parts:
        return {
            "status": "error",
            "message": "Use a relative file_path returned by MCP search tools.",
        }

    if any(part.lower() in EXCLUDED_DIRECTORIES for part in relative.parts):
        return {"status": "error", "message": "This folder is excluded."}

    path = root / relative
    try:
        resolved = path.resolve()
        resolved.relative_to(root)
    except (OSError, ValueError):
        return {"status": "error", "message": "Invalid source path."}

    if not _is_allowed_source(resolved):
        return {"status": "error", "message": "Unsupported source file type."}

    if not resolved.is_file():
        return {"status": "error", "message": "File not found."}

    start_line = int(start_line)
    end_line = int(end_line)
    if start_line < 1 or end_line < start_line:
        return {"status": "error", "message": "Invalid line range."}

    if end_line - start_line + 1 > 300:
        end_line = start_line + 299

    text = _read_source_text(resolved)
    if text is None:
        return {"status": "error", "message": "Could not read this source file."}

    lines = text.splitlines()
    if start_line > len(lines):
        return {
            "status": "error",
            "message": "start_line exceeds the file length.",
            "total_lines": len(lines),
        }

    actual_end = min(end_line, len(lines))
    return {
        "status": "ok",
        "file_path": resolved.relative_to(root).as_posix(),
        "start_line": start_line,
        "end_line": actual_end,
        "total_lines": len(lines),
        "has_more": actual_end < len(lines),
        "content": "\n".join(
            f"{number}: {lines[number - 1]}"
            for number in range(start_line, actual_end + 1)
        ),
    }


@mcp.tool()
def find_mock_order(order_no: str) -> dict:
    """Find one exact order in mock-data/orders.json."""
    order_no = str(order_no or "").strip()
    if not order_no:
        return {"status": "error", "message": "Order number is required."}

    orders, path = _load_mock_orders()
    if orders is None:
        return {
            "status": "error",
            "message": "mock-data/orders.json was not found or could not be read.",
            "resolved_path": path,
        }

    wanted = order_no.casefold()
    matches = [
        order
        for order in orders
        if str(
            order.get("orderNo")
            or order.get("order_no")
            or order.get("OrderNo")
            or ""
        ).casefold() == wanted
    ]

    return {
        "status": "ok",
        "file_path": "mock-data/orders.json",
        "matches": matches,
        "orders": matches,
    }


@mcp.tool()
def search_mock_orders(
    labels: list[str] | None = None,
    payment_method: str | None = None,
    status: str | None = None,
    exclude_status: str | None = None,
    carrier: str | None = None,
) -> dict:
    """Filter mock orders by common test fields."""
    orders, path = _load_mock_orders()
    if orders is None:
        return {
            "status": "error",
            "message": "mock-data/orders.json was not found or could not be read.",
            "resolved_path": path,
        }

    wanted_labels = {
        str(label).strip().casefold()
        for label in (labels or [])
        if str(label).strip()
    }
    wanted_payment = str(payment_method or "").strip().casefold()
    wanted_status = str(status or "").strip().casefold()
    unwanted_status = str(exclude_status or "").strip().casefold()
    wanted_carrier = str(carrier or "").strip().casefold()

    def label_names(order: dict) -> set[str]:
        values = order.get("labels") or []
        output: set[str] = set()
        for value in values:
            if isinstance(value, dict):
                name = value.get("labelName") or value.get("name") or ""
            else:
                name = value
            if str(name).strip():
                output.add(str(name).strip().casefold())
        return output

    def field(order: dict, *names: str) -> str:
        for name in names:
            value = order.get(name)
            if value is not None:
                if isinstance(value, dict):
                    value = (
                        value.get("name")
                        or value.get("carrierName")
                        or value.get("text")
                        or ""
                    )
                return str(value).strip().casefold()
        return ""

    filtered = []
    for order in orders:
        if wanted_labels and not label_names(order).intersection(wanted_labels):
            continue
        if wanted_payment and field(
            order,
            "paymentMethod",
            "payment_method",
            "PaymentMethod",
        ) != wanted_payment:
            continue
        current_status = field(order, "status", "Status", "orderStatus")
        if wanted_status and current_status != wanted_status:
            continue
        if unwanted_status and current_status == unwanted_status:
            continue
        if wanted_carrier and field(
            order,
            "carrier",
            "carrierName",
            "Carrier",
        ) != wanted_carrier:
            continue
        filtered.append(order)

    return {
        "status": "ok",
        "file_path": "mock-data/orders.json",
        "orders": filtered,
    }


def _symbol_pattern(symbol_name: str) -> re.Pattern:
    return re.compile(rf"\b{re.escape(symbol_name)}\b", re.IGNORECASE)


def _collect_line_matches(
    symbol_name: str,
    *,
    max_results: int = 30,
    predicate=None,
) -> list[dict]:
    pattern = _symbol_pattern(symbol_name)
    matches: list[dict] = []

    for path in _iter_source_files() or []:
        relative = _safe_relative(path)
        text = _read_source_text(path)
        if not relative or text is None:
            continue

        for line_number, line in enumerate(text.splitlines(), start=1):
            if not pattern.search(line):
                continue
            if predicate is not None and not predicate(line, relative):
                continue
            matches.append({
                "file_path": relative,
                "line_number": line_number,
                "line_text": line.strip()[:500],
            })
            if len(matches) >= max_results:
                return matches

    return matches


@mcp.tool()
def find_symbol(symbol_name: str) -> dict:
    """Find likely declarations/definitions for a code symbol."""
    symbol_name = str(symbol_name or "").strip()
    if not symbol_name:
        return {"status": "error", "message": "Symbol name is required."}

    declaration_words = re.compile(
        r"\b(class|interface|record|enum|def|function|const|let|var|public|private|protected|export|async)\b",
        re.IGNORECASE,
    )
    matches = _collect_line_matches(
        symbol_name,
        predicate=lambda line, _: bool(declaration_words.search(line)),
    )
    if not matches:
        matches, _, _ = _search_literal(symbol_name, 30)

    return {"status": "ok", "symbol": symbol_name, "matches": matches}


@mcp.tool()
def find_references(symbol_name: str) -> dict:
    """Find literal references to a symbol across Shipra source files."""
    symbol_name = str(symbol_name or "").strip()
    if not symbol_name:
        return {"status": "error", "message": "Symbol name is required."}

    matches = _collect_line_matches(symbol_name, max_results=50)
    return {"status": "ok", "symbol": symbol_name, "matches": matches}


@mcp.tool()
def find_imports(symbol_name: str) -> dict:
    """Find JS/TS imports, requires, and C# using lines mentioning a symbol."""
    symbol_name = str(symbol_name or "").strip()
    if not symbol_name:
        return {"status": "error", "message": "Symbol name is required."}

    def is_import(line: str, _: str) -> bool:
        lowered = line.strip().lower()
        return (
            lowered.startswith("import ")
            or lowered.startswith("using ")
            or "require(" in lowered
            or " from " in lowered
        )

    matches = _collect_line_matches(
        symbol_name,
        max_results=40,
        predicate=is_import,
    )
    return {"status": "ok", "symbol": symbol_name, "matches": matches}


@mcp.tool()
def find_route(route_name: str) -> dict:
    """Find frontend or backend route declarations mentioning a route/feature."""
    route_name = str(route_name or "").strip()
    if not route_name:
        return {"status": "error", "message": "Route name is required."}

    route_markers = ("route", "path=", "httpget", "httppost", "httpput", "httpdelete", "map")

    def is_route(line: str, _: str) -> bool:
        lowered = line.casefold()
        return any(marker in lowered for marker in route_markers)

    matches = _collect_line_matches(
        route_name,
        max_results=40,
        predicate=is_route,
    )
    if not matches:
        matches, _, _ = _search_literal(route_name, 30)
    return {"status": "ok", "route": route_name, "matches": matches}


@mcp.tool()
def find_controller(identifier: str) -> dict:
    """Find controller-related source for an identifier."""
    identifier = str(identifier or "").strip()
    if not identifier:
        return {"status": "error", "message": "Controller identifier is required."}

    queries = [identifier]
    if not identifier.lower().endswith("controller"):
        queries.append(identifier + "Controller")

    matches: list[dict] = []
    seen = set()
    for query in queries:
        found, _, _ = _search_literal(query, 30)
        for item in found:
            if "controller" not in item["file_path"].casefold() and "controller" not in item.get("line_text", "").casefold():
                continue
            key = (item["file_path"], item["line_number"])
            if key not in seen:
                seen.add(key)
                matches.append(item)

    return {"status": "ok", "identifier": identifier, "matches": matches[:40]}


@mcp.tool()
def find_handler(identifier: str) -> dict:
    """Find command/query handler source for an identifier."""
    identifier = str(identifier or "").strip()
    if not identifier:
        return {"status": "error", "message": "Handler identifier is required."}

    queries = [identifier]
    if not identifier.lower().endswith("handler"):
        queries.append(identifier + "Handler")

    matches: list[dict] = []
    seen = set()
    for query in queries:
        found, _, _ = _search_literal(query, 30)
        for item in found:
            path_and_line = (
                item["file_path"] + " " + item.get("line_text", "")
            ).casefold()
            if "handler" not in path_and_line:
                continue
            key = (item["file_path"], item["line_number"])
            if key not in seen:
                seen.add(key)
                matches.append(item)

    return {"status": "ok", "identifier": identifier, "matches": matches[:40]}


@mcp.tool()
def read_exact_function(symbol_name: str) -> dict:
    """Find a symbol and return bounded source excerpts around its strongest matches."""
    symbol_name = str(symbol_name or "").strip()
    if not symbol_name:
        return {"status": "error", "message": "Symbol name is required."}

    symbol_result = find_symbol(symbol_name)
    matches = symbol_result.get("matches", []) if isinstance(symbol_result, dict) else []
    excerpts = []

    for match in matches[:5]:
        line_number = int(match.get("line_number") or 1)
        start = max(1, line_number - 12)
        read_result = read_file(
            match.get("file_path", ""),
            start,
            start + 119,
        )
        if read_result.get("status") == "ok":
            excerpts.append(read_result)

    return {
        "status": "ok",
        "symbol": symbol_name,
        "matches": matches,
        "excerpts": excerpts,
    }


@mcp.tool()
def trace_call_chain(entry_symbol: str) -> dict:
    """Return declaration/reference/import evidence that can be used to trace a call chain."""
    entry_symbol = str(entry_symbol or "").strip()
    if not entry_symbol:
        return {"status": "error", "message": "Entry symbol is required."}

    declarations = find_symbol(entry_symbol).get("matches", [])
    references = find_references(entry_symbol).get("matches", [])
    imports = find_imports(entry_symbol).get("matches", [])

    # Include compact excerpts around the strongest references so the planner
    # can identify the next exact API/function name without guessing.
    excerpts = []
    seen_paths = set()
    for match in (declarations + references)[:8]:
        file_path = match.get("file_path")
        if not file_path or file_path in seen_paths:
            continue
        seen_paths.add(file_path)
        line_number = int(match.get("line_number") or 1)
        start = max(1, line_number - 8)
        result = read_file(file_path, start, start + 79)
        if result.get("status") == "ok":
            excerpts.append(result)

    return {
        "status": "ok",
        "entry_symbol": entry_symbol,
        "declarations": declarations,
        "references": references,
        "imports": imports,
        "excerpts": excerpts,
    }


if __name__ == "__main__":
    mcp.run()
