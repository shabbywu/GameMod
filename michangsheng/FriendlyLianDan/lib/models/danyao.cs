namespace ItemSystem.Models
{
    public class DanYao : BaseModel
    {
        static public WuPingType TYPE = WuPingType.丹药;

        public ItemSystem.Loaders.DanFang danfang;
        public int costTime;
        public YaoXing intro;
        public YaoXing main1;
        public YaoXing main2;
        public YaoXing deputy1;
        public YaoXing deputy2;

        public DanYao(int itemID) : base(itemID)
        {
            if (type != TYPE)
            {
                throw new TypeException($"物品类型不一致, 期望类型: {TYPE}, 得到 {type}");
            }

            try
            {
                danfang = ItemSystem.Loaders.DanFangs.getByDanYaoID(itemID);
                intro = YaoXing.GetHerbsYaoXing(danfang.value1, danfang.num1, 1);
                Validate(YaoXing.GetHerbsYaoXing(danfang.value2, danfang.num2, 2), YaoXing.GetHerbsYaoXing(danfang.value3, danfang.num3, 2), out main1, out main2);
                Validate(YaoXing.GetHerbsYaoXing(danfang.value4, danfang.num4, 3), YaoXing.GetHerbsYaoXing(danfang.value5, danfang.num5, 3), out deputy1, out deputy2);
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {

            }
        }

        private static void Validate(YaoXing input1, YaoXing input2, out YaoXing out1, out YaoXing out2)
        {
            if (input1 == null)
            {
                input1 = input2;
                input2 = null;
            }

            if (input2 != null && input1.kind == input2.kind)
            {
                input1 = new YaoXing(
                    kind: input1.kind,
                    intensity: input1.intensity + input2.intensity
                );
                input2 = null;
            }

            if (input1 != null && input1 != null)
            {
                if (input1.kind > input1.kind)
                {
                    var tmp = input1;
                    input1 = input2;
                    input2 = tmp;
                }
            }
            out1 = input1;
            out2 = input2;
        }
    }
}
