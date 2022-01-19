from typing import Dict, List, Iterator, Tuple, Optional
from items.constants import WuPingType
from items.loaders.items import items
from items.models.danyao import Herbs, YaoXing


class Tables:
    """草药药性速查表"""

    def __init__(self):
        self._initialed = False
        self.intro: Dict[YaoXing, List] = {}
        self.main: Dict[YaoXing, List] = {}
        self.deputy: Dict[YaoXing, List] = {}

    def _load(self):
        if self._initialed:
            return

        for item in items.filter_by_type(WuPingType.草药):
            herb = Herbs(item.id)
            for kind in ["intro", "main", "deputy"]:
                base: YaoXing = getattr(herb, kind)
                container: Dict[YaoXing, List] = getattr(self, kind)
                for num in range(1, 11):
                    container.setdefault(base * num, []).append((num, herb))

        self._initialed = True

    def list_possible_intro(
        self, key: YaoXing, fuzzy: bool = False, max_num: Optional[int] = None
    ) -> Iterator[Tuple[int, Herbs]]:
        """列举所有可以达到该需求的组合"""
        self._load()
        yield from filter_by_num(self.intro.get(key, []), max_num=max_num)

        if fuzzy:
            for possible in self.intro.keys():
                if key.kind == possible.kind and key.intensity < possible.intensity:
                    yield from filter_by_num(self.intro[possible], max_num=max_num)

    def list_possible_main(
        self, key: YaoXing, fuzzy: bool = False, max_num: Optional[int] = None
    ) -> Iterator[Tuple[int, Herbs]]:
        """列举所有可以达到该需求的组合"""
        self._load()
        yield from filter_by_num(self.main.get(key, []), max_num=max_num)

        if fuzzy:
            for possible in self.main.keys():
                if key.kind == possible.kind and key.intensity < possible.intensity:
                    yield from filter_by_num(self.main[possible], max_num=max_num)

    def list_possible_deputy(
        self, key: YaoXing, fuzzy: bool = False, max_num: Optional[int] = None
    ) -> Iterator[Tuple[int, Herbs]]:
        """列举所有可以达到该需求的组合"""
        self._load()
        yield from filter_by_num(self.deputy.get(key, []), max_num=max_num)

        if fuzzy:
            for possible in self.deputy.keys():
                if key.kind == possible.kind and key.intensity < possible.intensity:
                    yield from filter_by_num(self.deputy[possible], max_num=max_num)


def filter_by_num(
    iterator: Iterator[Tuple[int, Herbs]], max_num: Optional[int]
) -> Iterator[Tuple[int, Herbs]]:
    if max_num is None:
        yield from iterator
    else:
        for combination in iterator:
            if combination[0] <= max_num:
                yield combination


tables = Tables()
