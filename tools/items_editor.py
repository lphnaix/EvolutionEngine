"""
Items editor/validator.

功能：
- 读取 JSON/YAML（扩展名 .json/.yaml/.yml）
- 校验必填字段、枚举、数值范围、ID 唯一性
- 可选导出为 JSON

用法示例：
  python items_editor.py --input ../Data/items.json --validate
  python items_editor.py --input ../Data/items.yaml --output ../Data/items.json --validate
"""

from __future__ import annotations

import argparse
import json
from pathlib import Path
from typing import Any, Dict, List

import yaml

VALID_TYPES = {"Weapon", "Armor", "Consumable"}


def load_items(path: Path) -> List[Dict[str, Any]]:
    if not path.exists():
        raise FileNotFoundError(f"Input file not found: {path}")
    text = path.read_text(encoding="utf-8")
    if path.suffix.lower() == ".json":
        return json.loads(text)
    if path.suffix.lower() in {".yaml", ".yml"}:
        return yaml.safe_load(text)
    raise ValueError(f"Unsupported file extension: {path.suffix}")


def save_json(items: List[Dict[str, Any]], path: Path) -> None:
    path.write_text(json.dumps(items, ensure_ascii=False, indent=2), encoding="utf-8")


def validate_items(items: List[Dict[str, Any]]) -> List[str]:
    errors: List[str] = []
    seen_ids = set()

    for idx, it in enumerate(items):
        prefix = f"item[{idx}]"
        item_id = it.get("id")
        name = it.get("name")
        itype = it.get("type")
        bonuses = it.get("bonuses", {})
        value = it.get("value", 0)

        if not item_id or not isinstance(item_id, str):
            errors.append(f"{prefix}: id missing or not a string")
        elif item_id in seen_ids:
            errors.append(f"{prefix}: duplicate id '{item_id}'")
        else:
            seen_ids.add(item_id)

        if not name or not isinstance(name, str):
            errors.append(f"{prefix}: name missing or not a string")

        if itype not in VALID_TYPES:
            errors.append(f"{prefix}: type must be one of {VALID_TYPES}, got '{itype}'")

        if not isinstance(bonuses, dict):
            errors.append(f"{prefix}: bonuses must be an object/dict")
        else:
            for k, v in bonuses.items():
                if not isinstance(k, str):
                    errors.append(f"{prefix}: bonus key must be string, got {type(k)}")
                if not isinstance(v, (int, float)):
                    errors.append(f"{prefix}: bonus value for {k} must be number, got {type(v)}")

        if not isinstance(value, (int, float)) or value < 0:
            errors.append(f"{prefix}: value must be >= 0, got {value}")

    return errors


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Items editor/validator.")
    parser.add_argument(
        "--input",
        "-i",
        type=Path,
        default=Path(__file__).resolve().parent.parent / "Data" / "items.json",
        help="Input JSON/YAML file path.",
    )
    parser.add_argument(
        "--output",
        "-o",
        type=Path,
        help="Optional output JSON path (will write validated items as JSON).",
    )
    parser.add_argument("--validate", action="store_true", help="Validate items and print errors.")
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    items = load_items(args.input)
    print(f"Loaded {len(items)} items from {args.input}")

    errors = validate_items(items) if args.validate else []
    if errors:
        print("Validation failed:")
        for e in errors:
            print(f" - {e}")
    else:
        if args.validate:
            print("Validation passed.")

    if args.output:
        save_json(items, args.output)
        print(f"Wrote {len(items)} items to {args.output}")


if __name__ == "__main__":
    main()
