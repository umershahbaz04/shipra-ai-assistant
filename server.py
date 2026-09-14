from datetime import date
import os
import hmac
import re

from mcp.server.transport_security import TransportSecuritySettings
from starlette.middleware.base import BaseHTTPMiddleware
from starlette.requests import Request
from starlette.responses import JSONResponse
import uvicorn
from pathlib import Path

from mcp.server.mcpserver import MCPServer


PROJECT_ROOT = Path(__file__).resolve().parent / "project-source"

mcp = MCPServer("Shipra Code Assistant")
MOCK_ORDER_RECORDS = [
    {
        "order_no": "ORD-1001",
        "status": "Delivered",
        "created_on": "2026-09-10",
        "store_id": 1,
        "carrier_id": 10,
    },
    {
        "order_no": "ORD-1002",
        "status": "On The Way",
        "created_on": "2026-09-10",
        "store_id": 1,
        "carrier_id": 10,
    },
    {
        "order_no": "ORD-1003",
        "status": "Queued",
        "created_on": "2026-09-10",
        "store_id": 1,
        "carrier_id": 11,
    },
    {
        "order_no": "ORD-1004",
        "status": "Pending",
        "created_on": "2026-09-11",
        "store_id": 2,
        "carrier_id": 10,
    },
    {
        "order_no": "ORD-1005",
        "status": "Delivered",
        "created_on": "2026-09-11",
        "store_id": 2,
        "carrier_id": 11,
    },
]


def load_order_records():
    """
    Temporary mock-data source.

    Later, replace only this function with a secure Shipra Backend API call
    or a read-only database query. Keep the MCP tools below unchanged.
    """
    return MOCK_ORDER_RECORDS


@mcp.tool()
def get_order_status_summary(
    date_from: str | None = None,
    date_to: str | None = None,
    store_id: int | None = None,
    carrier_id: int | None = None,
) -> dict:
    """
    Return mock order counts for Delivered, On The Way, Queued, and Pending.
    Dates must use YYYY-MM-DD format.
    """
    try:
        if date_from:
            date.fromisoformat(date_from)

        if date_to:
            date.fromisoformat(date_to)
    except ValueError:
        return {
            "status": "error",
            "message": "Use dates in YYYY-MM-DD format.",
        }

    records = []

    for order in load_order_records():
        if date_from and order["created_on"] < date_from:
            continue

        if date_to and order["created_on"] > date_to:
            continue

        if store_id is not None and order["store_id"] != store_id:
            continue

        if carrier_id is not None and order["carrier_id"] != carrier_id:
            continue

        records.append(order)

    counts = {
        "delivered": 0,
        "on_the_way": 0,
        "queued": 0,
        "pending": 0,
    }

    for order in records:
        status = order["status"].casefold()

        if status == "delivered":
            counts["delivered"] += 1
        elif status == "on the way":
            counts["on_the_way"] += 1
        elif status == "queued":
            counts["queued"] += 1
        elif status == "pending":
            counts["pending"] += 1

    return {
        "status": "ok",
        "data_source": "mock",
        "filters": {
            "date_from": date_from,
            "date_to": date_to,
            "store_id": store_id,
            "carrier_id": carrier_id,
        },
        "total_orders": len(records),
        "counts": counts,
    }


@mcp.tool()
def get_project_structure() -> dict:
    """List the available Shipra source folders."""
    if not PROJECT_ROOT.is_dir():
        return {
            "status": "error",
            "message": "project-source folder was not found beside server.py",
        }

    folders = sorted(
        folder.name
        for folder in PROJECT_ROOT.iterdir()
        if folder.is_dir()
        and not folder.is_symlink()
        and not folder.name.startswith(".")
        and folder.name.lower() not in {"node_modules", "bin", "obj"}
    )

    return {
        "status": "ok",
        "folders": folders,
        "folder_count": len(folders),
    }

@mcp.tool()
def search_code(query: str, max_results: int = 20) -> dict:
    """Search literal text in Shipra source files and return file/line matches."""
    query = query.strip()

    if len(query) < 3:
        return {
            "status": "error",
            "message": "Enter at least 3 characters.",
        }

    if not PROJECT_ROOT.is_dir():
        return {
            "status": "error",
            "message": "project-source folder was not found.",
        }

    max_results = max(1, min(max_results, 50))
    allowed_extensions = {".cs", ".js", ".jsx", ".ts", ".tsx"}
    excluded_directories = {
        ".git", ".venv", "venv", "node_modules",
        "bin", "obj", "dist", "build", "__pycache__",
    }

    matches = []
    skipped_files = 0
    search_text = query.casefold()

    for current_dir, directories, filenames in os.walk(
        PROJECT_ROOT,
        followlinks=False,
    ):
        directories[:] = sorted(
            name
            for name in directories
            if name.lower() not in excluded_directories
            and not (Path(current_dir) / name).is_symlink()
            and not (Path(current_dir) / name).is_junction()
        )

        for filename in sorted(filenames):
            path = Path(current_dir) / filename

            if path.suffix.lower() not in allowed_extensions:
                continue

            if path.is_symlink():
                continue

            if filename.lower().endswith(
                (".min.js", ".generated.cs", ".designer.cs")
            ):
                continue

            try:
                resolved = path.resolve()
                relative = resolved.relative_to(PROJECT_ROOT.resolve())

                if resolved.stat().st_size > 2_000_000:
                    skipped_files += 1
                    continue

                text = resolved.read_text(encoding="utf-8-sig")
            except (OSError, UnicodeError, ValueError):
                skipped_files += 1
                continue

            if search_text in relative.as_posix().casefold():
                matches.append({
                    "file_path": relative.as_posix(),
                    "line_number": 1,
                    "line_text": "[file path match]",
                    "match_type": "path",
                })
                if len(matches) >= max_results:
                    return {
                        "status": "ok",
                        "matches": matches,
                        "limit_reached": True,
                        "skipped_files": skipped_files,
                    }

            for line_number, line in enumerate(text.splitlines(), start=1):
                if search_text in line.casefold():
                    matches.append({
                        "file_path": relative.as_posix(),
                        "line_number": line_number,
                        "line_text": line.strip()[:300],
                        "match_type": "content",
                    })

                    if len(matches) >= max_results:
                        return {
                            "status": "ok",
                            "matches": matches,
                            "limit_reached": True,
                            "skipped_files": skipped_files,
                        }

    return {
        "status": "ok",
        "matches": matches,
        "limit_reached": False,
        "skipped_files": skipped_files,
    }

@mcp.tool()
def read_file(
    file_path: str,
    start_line: int = 1,
    end_line: int = 120,
) -> dict:
    """Read a bounded line range from an allowed Shipra source file."""
    root = PROJECT_ROOT.resolve()
    relative = Path(file_path)

    if relative.is_absolute() or relative.drive or ".." in relative.parts:
        return {
            "status": "error",
            "message": "Use a relative file_path returned by search_code.",
        }

    excluded = {
        ".git", ".venv", "venv", "node_modules",
        "bin", "obj", "dist", "build", "__pycache__",
    }

    if any(part.lower() in excluded for part in relative.parts):
        return {"status": "error", "message": "This folder is excluded."}

    path = root
    for part in relative.parts:
        path = path / part
        if path.is_symlink() or path.is_junction():
            return {"status": "error", "message": "Linked paths are excluded."}

    if path.suffix.lower() not in {".cs", ".js", ".jsx", ".ts", ".tsx"}:
        return {"status": "error", "message": "Unsupported source file type."}

    if path.name.lower().endswith(
        (".min.js", ".generated.cs", ".designer.cs")
    ):
        return {"status": "error", "message": "Generated files are excluded."}

    if start_line < 1 or end_line < start_line:
        return {"status": "error", "message": "Invalid line range."}

    if end_line - start_line + 1 > 300:
        return {"status": "error", "message": "Read at most 300 lines per call."}

    try:
        path = path.resolve()
        path.relative_to(root)

        if not path.is_file():
            return {"status": "error", "message": "File not found."}

        if path.stat().st_size > 2_000_000:
            return {"status": "error", "message": "File exceeds size limit."}

        lines = path.read_text(encoding="utf-8-sig").splitlines()
    except (OSError, UnicodeError, ValueError):
        return {"status": "error", "message": "Could not read this source file."}

    if start_line > len(lines):
        return {
            "status": "error",
            "message": "start_line exceeds the file length.",
            "total_lines": len(lines),
        }

    actual_end = min(end_line, len(lines))

    return {
        "status": "ok",
        "file_path": path.relative_to(root).as_posix(),
        "start_line": start_line,
        "end_line": actual_end,
        "total_lines": len(lines),
        "has_more": actual_end < len(lines),
        "content": "\n".join(
            f"{number}: {lines[number - 1]}"
            for number in range(start_line, actual_end + 1)
        ),
    }

SOURCE_EXTENSIONS = {".cs", ".js", ".jsx", ".ts", ".tsx"}

EXCLUDED_SOURCE_FOLDERS = {
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


def iter_shipra_source_files():
    """Yield safe Shipra source files only."""
    if not PROJECT_ROOT.is_dir():
        return

    root = PROJECT_ROOT.resolve()

    for current_dir, directories, filenames in os.walk(
        PROJECT_ROOT,
        followlinks=False,
    ):
        directories[:] = [
            folder
            for folder in directories
            if folder.lower() not in EXCLUDED_SOURCE_FOLDERS
            and not (Path(current_dir) / folder).is_symlink()
            and not (Path(current_dir) / folder).is_junction()
        ]

        for filename in sorted(filenames):
            path = Path(current_dir) / filename

            if path.suffix.lower() not in SOURCE_EXTENSIONS:
                continue

            if path.is_symlink() or path.name.lower().endswith(
                (".min.js", ".generated.cs", ".designer.cs")
            ):
                continue

            try:
                resolved = path.resolve()
                resolved.relative_to(root)

                if resolved.stat().st_size > 2_000_000:
                    continue

                yield resolved
            except (OSError, ValueError):
                continue


def read_source_lines(path: Path):
    """Read one validated source file as lines."""
    try:
        return path.read_text(encoding="utf-8-sig").splitlines()
    except (OSError, UnicodeError):
        return None


def relative_source_path(path: Path) -> str:
    return path.resolve().relative_to(PROJECT_ROOT.resolve()).as_posix()


def is_word_match(line: str, value: str) -> bool:
    return bool(
        re.search(
            rf"(?<![A-Za-z0-9_]){re.escape(value)}(?![A-Za-z0-9_])",
            line,
            re.IGNORECASE,
        )
    )

@mcp.tool()
def find_symbol(
    symbol: str,
    max_results: int = 20,
) -> dict:
    """
    Find the exact declaration of a React component, JavaScript function,
    C# class, controller, command, query, or handler.
    """
    symbol = symbol.strip()

    if not symbol:
        return {"status": "error", "message": "symbol is required."}

    max_results = max(1, min(max_results, 50))

    declaration_patterns = [
        re.compile(
            rf"\b(?:public|private|protected|internal|static|abstract|sealed|partial)?\s*"
            rf"(?:class|interface|record|enum)\s+{re.escape(symbol)}\b",
            re.IGNORECASE,
        ),
        re.compile(
            rf"\b(?:const|let|var|function|class)\s+{re.escape(symbol)}\b",
            re.IGNORECASE,
        ),
        re.compile(
            rf"\b{re.escape(symbol)}\s*=\s*(?:async\s*)?\(",
            re.IGNORECASE,
        ),
        re.compile(
            rf"\b(?:public|private|protected|internal)\s+[\w<>\[\]?,.\s]+\s+"
            rf"{re.escape(symbol)}\s*\(",
            re.IGNORECASE,
        ),
    ]

    matches = []

    for path in iter_shipra_source_files():
        lines = read_source_lines(path)

        if lines is None:
            continue

        for line_number, line in enumerate(lines, start=1):
            if any(pattern.search(line) for pattern in declaration_patterns):
                matches.append(
                    {
                        "file_path": relative_source_path(path),
                        "line_number": line_number,
                        "line_text": line.strip()[:300],
                    }
                )

                if len(matches) >= max_results:
                    return {
                        "status": "ok",
                        "symbol": symbol,
                        "matches": matches,
                        "limit_reached": True,
                    }

    return {
        "status": "ok",
        "symbol": symbol,
        "matches": matches,
        "limit_reached": False,
    }


@mcp.tool()
def find_references(
    symbol: str,
    max_results: int = 30,
    include_declarations: bool = False,
) -> dict:
    """
    Find where a symbol is used across Shipra source files.
    Set include_declarations=True to include its declaration too.
    """
    symbol = symbol.strip()

    if not symbol:
        return {"status": "error", "message": "symbol is required."}

    max_results = max(1, min(max_results, 100))
    matches = []

    declaration_pattern = re.compile(
        rf"\b(?:class|interface|record|enum|function|const|let|var)\s+"
        rf"{re.escape(symbol)}\b",
        re.IGNORECASE,
    )

    for path in iter_shipra_source_files():
        lines = read_source_lines(path)

        if lines is None:
            continue

        for line_number, line in enumerate(lines, start=1):
            if not is_word_match(line, symbol):
                continue

            if not include_declarations and declaration_pattern.search(line):
                continue

            matches.append(
                {
                    "file_path": relative_source_path(path),
                    "line_number": line_number,
                    "line_text": line.strip()[:300],
                }
            )

            if len(matches) >= max_results:
                return {
                    "status": "ok",
                    "symbol": symbol,
                    "references": matches,
                    "limit_reached": True,
                }

    return {
        "status": "ok",
        "symbol": symbol,
        "references": matches,
        "limit_reached": False,
    }


@mcp.tool()
def find_imports(
    symbol: str,
    max_results: int = 30,
) -> dict:
    """
    Find JavaScript/TypeScript imports and C# using/import references
    related to a symbol.
    """
    symbol = symbol.strip()

    if not symbol:
        return {"status": "error", "message": "symbol is required."}

    max_results = max(1, min(max_results, 100))
    matches = []

    for path in iter_shipra_source_files():
        lines = read_source_lines(path)

        if lines is None:
            continue

        extension = path.suffix.lower()

        for line_number, line in enumerate(lines, start=1):
            stripped_line = line.strip()

            is_js_import = (
                extension in {".js", ".jsx", ".ts", ".tsx"}
                and stripped_line.startswith("import ")
                and is_word_match(stripped_line, symbol)
            )

            is_csharp_import = (
                extension == ".cs"
                and (
                    stripped_line.startswith("using ")
                    or stripped_line.startswith("global using ")
                )
                and is_word_match(stripped_line, symbol)
            )

            if not (is_js_import or is_csharp_import):
                continue

            matches.append(
                {
                    "file_path": relative_source_path(path),
                    "line_number": line_number,
                    "line_text": stripped_line[:300],
                }
            )

            if len(matches) >= max_results:
                return {
                    "status": "ok",
                    "symbol": symbol,
                    "imports": matches,
                    "limit_reached": True,
                }

    return {
        "status": "ok",
        "symbol": symbol,
        "imports": matches,
        "limit_reached": False,
    }

@mcp.tool()
def find_route(
    route: str,
    max_results: int = 30,
) -> dict:
    """
    Find an API route in frontend Axios calls and backend controller actions.
    Example: CreateClientOrderLabelLookup
    """
    route = route.strip().strip("/")

    if not route:
        return {"status": "error", "message": "route is required."}

    max_results = max(1, min(max_results, 100))
    matches = []
    route_text = route.casefold()

    for path in iter_shipra_source_files():
        lines = read_source_lines(path)

        if lines is None:
            continue

        for line_number, line in enumerate(lines, start=1):
            if route_text not in line.casefold():
                continue

            file_path = relative_source_path(path)

            if "AxiosInterceptors" in file_path:
                layer = "frontend_api"
            elif "Controller" in file_path:
                layer = "backend_controller"
            else:
                layer = "related_source"

            matches.append(
                {
                    "layer": layer,
                    "file_path": file_path,
                    "line_number": line_number,
                    "line_text": line.strip()[:300],
                }
            )

            if len(matches) >= max_results:
                return {
                    "status": "ok",
                    "route": route,
                    "matches": matches,
                    "limit_reached": True,
                }

    return {
        "status": "ok",
        "route": route,
        "matches": matches,
        "limit_reached": False,
    }


@mcp.tool()
def find_controller(
    controller: str,
    max_results: int = 20,
) -> dict:
    """
    Find a .NET API controller class or controller file.
    Example: OrderController or Order
    """
    controller = controller.strip()

    if not controller:
        return {"status": "error", "message": "controller is required."}

    controller_name = (
        controller
        if controller.casefold().endswith("controller")
        else f"{controller}Controller"
    )

    max_results = max(1, min(max_results, 50))
    matches = []

    for path in iter_shipra_source_files():
        if path.suffix.lower() != ".cs":
            continue

        lines = read_source_lines(path)

        if lines is None:
            continue

        file_path = relative_source_path(path)

        for line_number, line in enumerate(lines, start=1):
            if (
                is_word_match(line, controller_name)
                or path.name.casefold() == f"{controller_name}.cs".casefold()
            ):
                matches.append(
                    {
                        "file_path": file_path,
                        "line_number": line_number,
                        "line_text": line.strip()[:300],
                    }
                )

                if len(matches) >= max_results:
                    return {
                        "status": "ok",
                        "controller": controller_name,
                        "matches": matches,
                        "limit_reached": True,
                    }

    return {
        "status": "ok",
        "controller": controller_name,
        "matches": matches,
        "limit_reached": False,
    }


@mcp.tool()
def find_handler(
    handler: str,
    max_results: int = 30,
) -> dict:
    """
    Find a MediatR command/query handler.
    Example: CreateClientOrderLabelLookupCommandHandler
    """
    handler = handler.strip()

    if not handler:
        return {"status": "error", "message": "handler is required."}

    handler_name = (
        handler
        if handler.casefold().endswith("handler")
        else f"{handler}Handler"
    )

    max_results = max(1, min(max_results, 50))
    matches = []

    for path in iter_shipra_source_files():
        if path.suffix.lower() != ".cs":
            continue

        lines = read_source_lines(path)

        if lines is None:
            continue

        for line_number, line in enumerate(lines, start=1):
            if is_word_match(line, handler_name):
                matches.append(
                    {
                        "file_path": relative_source_path(path),
                        "line_number": line_number,
                        "line_text": line.strip()[:300],
                    }
                )

                if len(matches) >= max_results:
                    return {
                        "status": "ok",
                        "handler": handler_name,
                        "matches": matches,
                        "limit_reached": True,
                    }

    return {
        "status": "ok",
        "handler": handler_name,
        "matches": matches,
        "limit_reached": False,
    }


@mcp.tool()
def trace_call_chain(
    keyword: str,
    max_results_per_layer: int = 8,
) -> dict:
    """
    Collect likely Shipra call-chain evidence by layer:
    frontend page/component, Axios API, controller, handler, repository.
    Example: CreateClientOrderLabelLookup
    """
    keyword = keyword.strip()

    if not keyword:
        return {"status": "error", "message": "keyword is required."}

    max_results_per_layer = max(1, min(max_results_per_layer, 20))

    layers = {
        "frontend_page_or_component": [],
        "frontend_api": [],
        "backend_controller": [],
        "backend_handler": [],
        "backend_repository": [],
        "other_related_code": [],
    }

    for path in iter_shipra_source_files():
        lines = read_source_lines(path)

        if lines is None:
            continue

        file_path = relative_source_path(path)

        for line_number, line in enumerate(lines, start=1):
            if not is_word_match(line, keyword):
                continue

            path_lower = file_path.casefold()
            line_lower = line.casefold()

            if "shipra.frontend/src/api/" in path_lower:
                layer_name = "frontend_api"
            elif "shipra.frontend/src/pages/" in path_lower:
                layer_name = "frontend_page_or_component"
            elif "shipra.frontend/src/components/" in path_lower:
                layer_name = "frontend_page_or_component"
            elif "/api/" in path_lower and "controller" in path_lower:
                layer_name = "backend_controller"
            elif "controller" in path_lower:
                layer_name = "backend_controller"
            elif "commandhandler" in path_lower or "queryhandler" in path_lower:
                layer_name = "backend_handler"
            elif "commandhandler" in line_lower or "queryhandler" in line_lower:
                layer_name = "backend_handler"
            elif "repository" in path_lower:
                layer_name = "backend_repository"
            else:
                layer_name = "other_related_code"

            if len(layers[layer_name]) < max_results_per_layer:
                layers[layer_name].append(
                    {
                        "file_path": file_path,
                        "line_number": line_number,
                        "line_text": line.strip()[:300],
                    }
                )

    ordered_flow = [
        {
            "stage": stage_name,
            "evidence": evidence,
        }
        for stage_name, evidence in layers.items()
        if evidence
    ]

    return {
        "status": "ok",
        "keyword": keyword,
        "call_chain_evidence": ordered_flow,
        "note": (
            "This is evidence-based tracing. Use read_exact_function on the "
            "returned files to verify the exact execution path."
        ),
    }


@mcp.tool()
def read_exact_function(
    file_path: str,
    symbol: str,
    max_lines: int = 400,
) -> dict:
    """
    Read one complete function, class, component, controller action, or handler
    by finding its declaration and matching curly braces.
    """
    relative = Path(file_path)
    root = PROJECT_ROOT.resolve()

    if not symbol.strip():
        return {"status": "error", "message": "symbol is required."}

    if relative.is_absolute() or relative.drive or ".." in relative.parts:
        return {
            "status": "error",
            "message": "Use a relative file_path returned by another MCP tool.",
        }

    if any(part.lower() in EXCLUDED_SOURCE_FOLDERS for part in relative.parts):
        return {"status": "error", "message": "This folder is excluded."}

    path = root / relative

    try:
        path = path.resolve()
        path.relative_to(root)
    except ValueError:
        return {"status": "error", "message": "Invalid file path."}

    if (
        not path.is_file()
        or path.suffix.lower() not in SOURCE_EXTENSIONS
        or path.is_symlink()
    ):
        return {"status": "error", "message": "Source file was not found."}

    lines = read_source_lines(path)

    if lines is None:
        return {"status": "error", "message": "Could not read this source file."}

    max_lines = max(20, min(max_lines, 500))
    escaped_symbol = re.escape(symbol.strip())

    declaration_patterns = [
        re.compile(rf"\bfunction\s+{escaped_symbol}\b", re.IGNORECASE),
        re.compile(rf"\b(?:const|let|var)\s+{escaped_symbol}\s*=", re.IGNORECASE),
        re.compile(rf"\b(?:class|interface|record|enum)\s+{escaped_symbol}\b", re.IGNORECASE),
        re.compile(
            rf"\b(?:public|private|protected|internal|static|async|virtual|override|\s)+"
            rf"[\w<>\[\]?,.\s]+\s+{escaped_symbol}\s*\(",
            re.IGNORECASE,
        ),
        re.compile(rf"\b{escaped_symbol}\s*\(", re.IGNORECASE),
    ]

    start_index = None

    for index, line in enumerate(lines):
        if any(pattern.search(line) for pattern in declaration_patterns):
            start_index = index
            break

    if start_index is None:
        return {
            "status": "error",
            "message": "Symbol declaration was not found in this file.",
        }

    brace_started = False
    brace_depth = 0
    end_index = None

    for index in range(start_index, len(lines)):
        line = lines[index]

        for character in line:
            if character == "{":
                brace_started = True
                brace_depth += 1
            elif character == "}" and brace_started:
                brace_depth -= 1

                if brace_depth == 0:
                    end_index = index
                    break

        if end_index is not None:
            break

    if end_index is None:
        return {
            "status": "error",
            "message": "Could not detect the end of this function or class.",
        }

    total_function_lines = end_index - start_index + 1
    actual_end_index = min(end_index, start_index + max_lines - 1)

    return {
        "status": "ok",
        "file_path": relative_source_path(path),
        "symbol": symbol,
        "start_line": start_index + 1,
        "end_line": actual_end_index + 1,
        "total_function_lines": total_function_lines,
        "is_truncated": actual_end_index < end_index,
        "content": "\n".join(
            f"{line_number}: {lines[line_number - 1]}"
            for line_number in range(start_index + 1, actual_end_index + 2)
        ),
    }


class MCPApiKeyMiddleware(BaseHTTPMiddleware):
    async def dispatch(self, request, call_next):
        if request.url.path == "/health":
            return await call_next(request)

        expected_key = os.environ.get("MCP_API_KEY", "")
        received_header = request.headers.get("authorization", "")

        if not expected_key:
            return JSONResponse(
                {"detail": "Server token is not configured."},
                status_code=503,
            )

        if not hmac.compare_digest(
            received_header,
            f"Bearer {expected_key}",
        ):
            return JSONResponse(
                {"detail": "Unauthorized"},
                status_code=401,
                headers={"WWW-Authenticate": "Bearer"},
            )

        allowed_origin = os.environ.get("MCP_ALLOWED_ORIGIN", "")
        request_origin = request.headers.get("origin", "")

        if (
            request_origin
            and allowed_origin
            and not hmac.compare_digest(request_origin, allowed_origin)
        ):
            return JSONResponse(
                {"detail": "Origin is not allowed."},
                status_code=403,
            )

        return await call_next(request)


async def health_check(request: Request):
    return JSONResponse({"status": "ok"})


transport_security = TransportSecuritySettings(
    enable_dns_rebinding_protection=False,
)

app = mcp.streamable_http_app(
    json_response=True,
    transport_security=transport_security,
)

app.add_route("/health", health_check, methods=["GET"])
app.add_middleware(MCPApiKeyMiddleware)


if __name__ == "__main__":
    transport = os.environ.get("MCP_TRANSPORT", "stdio").lower()

    if transport == "http":
        uvicorn.run(
            app,
            host="0.0.0.0",
            port=int(os.environ.get("PORT", "10000")),
            proxy_headers=True,
            forwarded_allow_ips="*",
        )
    else:
        mcp.run()