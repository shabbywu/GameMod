from pydantic import BaseModel, Field, BaseConfig
from typing import Tuple, Optional
from items.loaders.items import items
from items.loaders.danfang import danfangs
from items.loaders.yaozhi import yaozhis
from items.constants import WuPingType


class BaseItem:
    TYPE: WuPingType

    def __init__(self, item_id: int):
        self.base = items.get_by_id(item_id=item_id)
        self.id = self.base.id
        self.name = self.base.name
        self.quality = self.base.quality
        self.type = WuPingType(self.base.type)

        assert self.type == self.TYPE, "物品类型不一致"

    def __str__(self):
        return f"{self.base.quality}品{self.type.name}<{self.base.name}>"

    def __repr__(self):
        return f"{self.__class__.__name__}(item_id={self.base.id})"


class DanYao(BaseItem):
    """丹药"""

    TYPE = WuPingType.丹药

    def __init__(self, item_id: int):
        super().__init__(item_id)

        self.danfang = danfangs.get_by_item_id(item_id)
        self.cost_time = self.danfang.castTime

        self.intro = get_herbs_yaoxing(self.danfang.value1, self.danfang.num1, kind=1)
        self.main1 = get_herbs_yaoxing(self.danfang.value2, self.danfang.num2, kind=2)
        self.main2 = get_herbs_yaoxing(self.danfang.value3, self.danfang.num3, kind=2)
        self.deputy1 = get_herbs_yaoxing(self.danfang.value4, self.danfang.num4, kind=3)
        self.deputy2 = get_herbs_yaoxing(self.danfang.value5, self.danfang.num5, kind=3)
        self.validate()

    def validate(self):
        """校验主药, 副药的药性是否重复, 重复则将其合并"""
        if self.main1 is None:
            self.main1 = self.main2
            self.main2 = None

        if self.main2 is not None and self.main1.kind == self.main2.kind:
            self.main1 = YaoXing(
                kind=self.main1.kind,
                intensity=self.main1.intensity + self.main2.intensity,
            )
            self.main2 = None

        if self.main1 is not None and self.main2 is not None:
            if self.main1.kind > self.main2.kind:
                # 保证按药质低位排序, 方便对比
                self.main1, self.main2 = self.main2, self.main1

        if self.deputy1 is None:
            self.deputy1 = self.deputy2
            self.deputy2 = None

        if self.deputy2 is not None and self.deputy1.kind == self.deputy2.kind:
            self.deputy1 = YaoXing(
                kind=self.deputy1.kind,
                intensity=self.deputy1.intensity + self.deputy2.intensity,
            )
            self.deputy2 = None

        if self.deputy1 is not None and self.deputy2 is not None:
            if self.deputy1.kind > self.deputy2.kind:
                # 保证按药质低位排序, 方便对比
                self.deputy1, self.deputy2 = (self.deputy2, self.deputy1)


class Herbs(BaseItem):
    """草药"""

    TYPE = WuPingType.草药

    def __init__(self, item_id: int):
        super().__init__(item_id=item_id)
        self.intro = get_herbs_yaoxing(item_id, 1, 1)
        self.main = get_herbs_yaoxing(item_id, 1, 2)
        self.deputy = get_herbs_yaoxing(item_id, 1, 3)


class YaoXing(BaseModel):
    """药性"""

    kind: int = Field(
        description="药质",
    )
    intensity: int = Field(description="强度")

    class Config(BaseConfig):
        frozen = True

    def match(self, other: "YaoXing") -> Tuple[bool, float]:
        """判断另一个药性是否满足当前药性的需求

        :param other: 另一个药性
        :return: [药性是否满足需求, 缺多少药力/富裕多少药力]
        """
        if other.kind != self.kind:
            return False, float("-inf")
        if other.intensity < self.intensity:
            return False, self.intensity - other.intensity
        return True, self.intensity - other.intensity

    def __mul__(self, other: int) -> "YaoXing":
        if isinstance(other, int):
            return YaoXing(kind=self.kind, intensity=self.intensity * other)
        raise NotImplementedError

    def __str__(self):
        yaozhi = yaozhis.get_by_item_id(self.kind)
        return f"药性: {yaozhi.name}, 强度: {self.intensity}"


_quality_factor = {1: 1, 2: 3, 3: 9, 4: 36, 5: 180, 6: 1080}


def get_herbs_yaoxing(item_id: int, num: int, kind: int) -> Optional[YaoXing]:
    """计算草药的药质和药力

    :param item_id: 草药id
    :param num: 草药数量
    :param kind: 1: 药引, 2: 主药, 3: 副药
    :return:
    """
    if item_id == 0:
        return None

    kind_field = f"yaoZhi{kind}"
    herbs = items.get_by_id(item_id)
    return YaoXing(
        kind=getattr(herbs, kind_field), intensity=_quality_factor[herbs.quality] * num
    )
