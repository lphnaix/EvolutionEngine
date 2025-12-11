"""
Minimal build script:
- Reads Data/config.json|yaml and Data/items.json|yaml
- Validates basic structure
- Writes Build/Config/*.json with config_version
"""

from __future__ import annotations

import json
from pathlib import Path
from typing import Any, Dict, List

import yaml

ROOT = Path(__file__).resolve().parent.parent
DATA = ROOT / "Data"
BUILD = ROOT / "Build" / "Config"
CONFIG_VERSION = "0.1"


def load_any(path: Path) -> Any:
    text = path.read_text(encoding="utf-8")
    if path.suffix.lower() == ".json":
        return json.loads(text)
    if path.suffix.lower() in {".yaml", ".yml"}:
        return yaml.safe_load(text)
    raise ValueError(f"Unsupported extension: {path}")


def write_json(obj: Any, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(obj, ensure_ascii=False, indent=2), encoding="utf-8")


def build_engine_settings() -> None:
    src = DATA / "config.json"
    settings = load_any(src)
    settings["config_version"] = CONFIG_VERSION
    write_json(settings, BUILD / "engine_settings.json")
    print(f"Wrote engine_settings.json from {src}")


def build_items() -> None:
    src = DATA / "items.json"
    items = load_any(src)
    if not isinstance(items, list):
        raise ValueError("items must be a list")
    ids = set()
    for it in items:
        if "id" not in it:
            raise ValueError("item missing id")
        if it["id"] in ids:
            raise ValueError(f"duplicate item id {it['id']}")
        ids.add(it["id"])
    out = {"config_version": CONFIG_VERSION, "items": items}
    write_json(out, BUILD / "items.json")
    print(f"Wrote items.json from {src}")


def main() -> None:
    build_engine_settings()
    build_items()


if __name__ == "__main__":
    main()
