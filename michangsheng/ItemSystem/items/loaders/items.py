import json
from pathlib import Path
from typing import Dict, List, Iterator
from pydantic import BaseModel, Field
from items.constants import TuJianType, WuPingType


class Item(BaseModel):
    """物品定义"""

    # 基础属性
    id: int = Field(description="ID")
    name: str = Field(description="物品名称(中文)")
    ItemIcon: int = Field(description="物品 Icon 序号")
    maxNum: int = Field(description="可持有的最大个数")
    type: WuPingType = Field(description="物品类型")
    quality: int = Field(description="品质")
    price: int = Field(description="基础售价")
    desc: str = Field(description="短描述")
    desc2: str = Field(description="长描述")
    CanSale: int = Field(description="是否能售卖, 0 表示可以")
    CanUse: int = Field(description="玩家是否能使用")
    NPCCanUse: int = Field(description="NPC 是否能使用")
    seid: List[int] = Field(description="作用效果列表")
    ItemFlag: List[int]
    Affix: List[int] = Field(description="词缀(id列表)")
    tu_jian_type: TuJianType = Field(description="图鉴类型, 用于解锁图鉴", alias="TuJianType")
    ShopType: int = Field(description="商品类型")

    FaBaoType: str = Field(description="未知用途")
    WuWeiType: int = Field(description="五维类型(炼器相关)")
    ShuXingType: int = Field(description="属性类型(炼器相关)")
    typePinJie: int = Field(description="")
    StuTime: int = Field(description="学习消耗时间")

    vagueType: int = Field(description="未知")
    DanDu: int = Field(description="丹毒")
    yaoZhi1: int = Field(description="作为药引的药质")
    yaoZhi2: int = Field(description="作为主药的药质")
    yaoZhi3: int = Field(description="作为副药的药质")
    wuDao: List = Field(description="等价的悟道经验")


class Items:
    _filename = "d_items.py.datas.json"

    def __init__(self):
        self._initialed = False
        self._items: Dict[int, Item] = {}

    def get_by_id(self, item_id: int) -> Item:
        self._load()
        return self._items[item_id]

    def filter_by_type(self, type: WuPingType) -> Iterator[Item]:
        self._load()
        for _, item in self._items.items():
            if item.type == type:
                yield item

    def _load(self):
        if self._initialed:
            return

        path = Path(__file__).parent.parent / "assets" / self._filename
        data = json.loads(path.read_text())
        for _, raw in data.items():
            item = Item(**raw)
            self._items[item.id] = item
        self._initialed = True


items = Items()
