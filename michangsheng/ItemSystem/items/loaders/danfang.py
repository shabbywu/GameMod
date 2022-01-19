import json
from pathlib import Path
from typing import Dict
from pydantic import BaseModel, Field


class DangFang(BaseModel):
    id: int = Field(description="丹方ID")
    ItemID: int = Field(description="产物ID")
    name: str = Field(description="产物名称")
    castTime: int = Field(description="炼制耗时")

    value1: int = Field(description="药引")
    value2: int = Field(description="主药1")
    value3: int = Field(description="主药2")
    value4: int = Field(description="副药1")
    value5: int = Field(description="副药2")

    num1: int = Field(description="药引数量")
    num2: int = Field(description="主药1数量")
    num3: int = Field(description="主药2数量")
    num4: int = Field(description="副药1数量")
    num5: int = Field(description="副药2数量")


class DangFangs:
    _filename = "d_LianDan.py.DanFangBiao.json"

    def __init__(self):
        self._initialed = False
        self._dangfangs: Dict[int, DangFang] = {}

    def get_by_item_id(self, item_id: int) -> DangFang:
        self._load()
        return self._dangfangs[item_id]

    def _load(self):
        if self._initialed:
            return

        path = Path(__file__).parent.parent / "assets" / self._filename
        data = json.loads(path.read_text())
        for _, raw in data.items():
            item = DangFang(**raw)
            self._dangfangs[item.ItemID] = item
        self._initialed = True


danfangs = DangFangs()
