"""
Minimal items editor placeholder.
Reads Data/items.json and prints item list.
"""

import json
from pathlib import Path

DATA_PATH = Path(__file__).resolve().parent.parent / "Data" / "items.json"


def load_items():
    with open(DATA_PATH, "r", encoding="utf-8") as f:
        return json.load(f)


def main():
    items = load_items()
    print(f"Loaded {len(items)} items from {DATA_PATH.name}:")
    for it in items:
        print(f"- {it['id']}: {it['name']} ({it['type']}) bonuses={it.get('bonuses',{})}")


if __name__ == "__main__":
    main()
