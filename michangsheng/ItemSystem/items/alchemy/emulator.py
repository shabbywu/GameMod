from typing import Iterator
from items.models.danyao import DanYao, Herbs
from items.loaders.danfang import DangFang
from items.alchemy.tables import tables


class Emulator:
    def __init__(self, target: DanYao, max_num: int = 9):
        """
        :param target: 需要模拟的对象
        :param max_num: 最大的草药限制
        """
        self.target = target
        self.max_num = max_num

    def generator(self, fuzzy: bool = False) -> Iterator:
        """生成所有丹方"""
        max_num = self.max_num
        for main1_num, main1_herb in tables.list_possible_main(
            key=self.target.main1, max_num=max_num, fuzzy=fuzzy
        ):
            possible_main2s = (
                tables.list_possible_main(
                    key=self.target.main2, max_num=max_num - main1_num, fuzzy=fuzzy
                )
                if self.target.main2
                else [(0, None)]
            )
            for main2_num, main2_herb in possible_main2s:
                possible_deputy1s = (
                    tables.list_possible_deputy(
                        self.target.deputy1,
                        max_num=max_num - main1_num - main2_num,
                        fuzzy=fuzzy,
                    )
                    if self.target.deputy1
                    else [(0, None)]
                )
                for deputy1_num, deputy1_herb in possible_deputy1s:
                    possible_deputy2s = (
                        tables.list_possible_deputy(
                            key=self.target.deputy2,
                            max_num=max_num - main1_num - main2_num - deputy1_num,
                            fuzzy=fuzzy,
                        )
                        if self.target.deputy2
                        else [(0, None)]
                    )
                    for deputy2_num, deputy2_herb in possible_deputy2s:
                        for intro_num, inteo_herb in tables.list_possible_intro(
                            key=self.target.intro,
                            max_num=max_num
                            - main1_num
                            - main2_num
                            - deputy1_num
                            - deputy2_num,
                            fuzzy=fuzzy,
                        ):
                            if self.check_neutralize(
                                inteo_herb,
                                main1_herb,
                                main2_herb,
                                deputy1_herb,
                                deputy2_herb,
                            ):
                                yield DangFang(
                                    id=0,
                                    ItemID=self.target.id,
                                    name=self.target.name,
                                    castTime=self.target.cost_time,
                                    value1=inteo_herb.id,
                                    num1=intro_num,
                                    value2=main1_herb.id,
                                    num2=main1_num,
                                    value3=main2_herb.id if main2_herb else 0,
                                    num3=main2_num,
                                    value4=deputy1_herb.id if deputy1_herb else 0,
                                    num4=deputy1_num,
                                    value5=deputy2_herb.id if deputy2_herb else 0,
                                    num5=deputy2_num,
                                )

    def check_neutralize(self, intro: Herbs, *args: Herbs) -> bool:
        """检测草药是否能中和"""
        flag = 0
        for herb in args:
            if herb is not None:
                if herb.intro.kind == 1:
                    flag -= 1
                elif herb.intro.kind == 2:
                    flag += 1

        return (flag > 0 and intro.intro.kind == 1) or (flag < 0 and intro.intro.kind == 2) or (flag == 0 and intro.intro.kind == 3)

    def __iter__(self):
        return self.generator()
