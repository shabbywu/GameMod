using System.Collections.Generic;
using ItemSystem.Models;

namespace FriendlyLianDan
{
    public class Emulator
    {
        public DanYao target;
        public int maxNum;

        /*
        :param target: 需要模拟的对象
        :param maxNum: 最大的草药限制
        */
        public Emulator(DanYao target, int maxNum = 9)
        {
            this.target = target;
            this.maxNum = maxNum;
        }

        public IEnumerable<ItemSystem.Shims.DanFang> Generator(bool fuzzy = false)
        {
            if (target.danfang == null) {
                yield break;
            }

            foreach (Combination main1 in Tables.listPossibleMain(target.main1, fuzzy: fuzzy, maxNum: maxNum))
            {
                IEnumerable<Combination> possibleMain2s = target.main2 != null ? Tables.listPossibleMain(target.main2, fuzzy: fuzzy, maxNum: maxNum - main1.num) : new List<Combination> { new Combination(num: 0, herbs: null) };
                foreach (Combination main2 in possibleMain2s)
                {
                    IEnumerable<Combination> possibleDeputy1s = target.deputy1 != null ? Tables.listPossibleDeputy(target.deputy1, fuzzy: fuzzy, maxNum: maxNum - main1.num - main2.num) : new List<Combination> { new Combination(num: 0, herbs: null) };
                    foreach (Combination deputy1 in possibleDeputy1s)
                    {
                        IEnumerable<Combination> possibleDeputy2s = target.deputy2 != null ? Tables.listPossibleDeputy(target.deputy2, fuzzy: fuzzy, maxNum: maxNum - main1.num - main2.num - deputy1.num) : new List<Combination> { new Combination(num: 0, herbs: null) };
                        foreach (Combination deputy2 in possibleDeputy2s)
                        {
                            foreach (Combination intro in Tables.listPossibleIntro(target.intro, fuzzy: fuzzy, maxNum: maxNum - main1.num - main2.num - deputy1.num - deputy2.num))
                            {
                                if (ValidateNeutralize(intro.herbs.intro, main1.herbs, main2.herbs, deputy1.herbs, deputy2.herbs))
                                {
                                    var danfang = new ItemSystem.Shims.DanFang();
                                    danfang.name = target.name;
                                    danfang.ItemID = target.id;
                                    danfang.castTime = target.costTime;

                                    danfang.value1 = intro.herbs.id;
                                    danfang.num1 = intro.num;
                                    danfang.value2 = main1.herbs.id;
                                    danfang.num2 = main1.num;

                                    if (main2.herbs?.id != null)
                                    {
                                        danfang.value3 = main2.herbs.id;
                                    }
                                    danfang.num3 = main2.num;

                                    if (deputy1.herbs?.id != null)
                                    {
                                        danfang.value4 = deputy1.herbs.id;
                                    }
                                    danfang.num4 = deputy1.num;

                                    if (deputy2.herbs?.id != null)
                                    {
                                        danfang.value5 = deputy2.herbs.id;
                                    }
                                    danfang.num5 = deputy2.num;
                                    
                                    yield return danfang;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static bool ValidateNeutralize(YaoXing intro, params Herbs[] args)
        {
            int flag = 0;
            foreach (Herbs herbs in args)
            {
                if (herbs != null)
                {
                    if (herbs.intro.kind == 1)
                    {
                        flag -= 1;
                    }
                    else if (herbs.intro.kind == 2)
                    {
                        flag += 1;
                    }
                }
            }
            return (flag > 0 && intro.kind == 1) || (flag < 0 && intro.kind == 2) || (flag == 0 && intro.kind == 3);
        }
    }
}