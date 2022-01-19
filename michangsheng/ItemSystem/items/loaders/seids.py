import json
from pathlib import Path
from typing import Dict
from pydantic import BaseModel, Extra, BaseConfig


class GoodSeid(BaseModel):
    id: int

    class Config(BaseConfig):
        extra = Extra.allow

    def get(self, i: int) -> int:
        return getattr(self, f"value{i}")


class GoodSeids:
    _filename = "d_items.py.good_seid{seid}.json"

    def __init__(self):
        self._initialed = False
        self._seids: Dict[int, Dict[int, GoodSeid]] = {}

    def get_by_id(self, seid, item_id) -> GoodSeid:
        self._load()
        return self._seids[item_id][seid]

    def _load(self):
        if self._initialed:
            return

        for i in range(1, 34):
            path = (
                Path(__file__).parent.parent / "assets" / self._filename.format(seid=i)
            )
            if not path.exists():
                continue

            data = json.loads(path.read_text())
            for _, raw in data.items():
                seid = GoodSeid(**raw)
                self._seids.setdefault(seid.id, {})[i] = seid
        self._initialed = True


good_seids = GoodSeids()
