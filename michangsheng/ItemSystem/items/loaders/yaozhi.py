import json
from pathlib import Path
from typing import Dict
from pydantic import BaseModel, Field


class YaoZhi(BaseModel):
    id: int = Field(description="药质id")
    name: str = Field(description="药质名称")
    desc: str = Field(description="药质描述")


class YaoZhis:
    _filename = "d_LianDan.py.yaocaizhonglei.json"

    def __init__(self):
        self._initialed = False
        self._yaozhis: Dict[int, YaoZhi] = {}

    def get_by_item_id(self, item_id: int) -> YaoZhi:
        self._load()
        return self._yaozhis[item_id]

    def _load(self):
        if self._initialed:
            return

        path = Path(__file__).parent.parent / "assets" / self._filename
        data = json.loads(path.read_text())
        for _, raw in data.items():
            item = YaoZhi(**raw)
            self._yaozhis[item.id] = item
        self._initialed = True


yaozhis = YaoZhis()
