from enum import Enum


class TuJianType(int, Enum):
    """图鉴类型"""

    Normal = 0
    # 草药
    CaoYao = 1
    # 金属或矿石 材料
    WuPingCaiLiao = 2
    # 怪物掉落 材料
    SuCaiCaiLiao = 3
    # 丹药
    DanYao = 4


class WuPingType(int, Enum):
    """物品类型"""

    武器 = 0
    装甲 = 1
    饰品 = 2
    神通 = 3
    功法 = 4
    丹药 = 5
    草药 = 6
    任务物品 = 7
    炼器材料 = 8
    丹炉 = 9
    丹方 = 10
    药渣 = 11
    地图 = 12
    书籍 = 13
    灵舟 = 14
    道具 = 15
    包裹 = 16
