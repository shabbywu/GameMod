using System;
using System.Collections.Generic;
using ItemSystem;

namespace ItemSystem.Models
{

    public class YaoXing
    {
        public int kind { get; set; }
        public int intensity { get; set; }

        public YaoXing(int kind, int intensity)
        {
            this.kind = kind;
            this.intensity = intensity;
        }

        // 判断另一个药性是否满足当前药性的需求
        // param YaoXing other: 另一个药性
        public bool Match(YaoXing other)
        {
            if (other.kind != kind)
            {
                return false;
            }
            return other.intensity >= intensity;
        }

        public static YaoXing operator *(YaoXing one, int other)
        {
            return new YaoXing(one.kind, one.intensity * other);
        }

        public override int GetHashCode()
        {
            return $"({kind},{intensity})".GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == GetType()) {
                var i = (YaoXing)obj;
                return i.kind == kind && i.intensity == intensity;
            }
            return false;
        }

        public static Dictionary<int, int> QualityFactor = new Dictionary<int, int>{
          {1, 1},
          {2, 3},
          {3, 9},
          {4, 36},
          {5, 180},
          {6, 1080},
      };

        public static YaoXing GetHerbsYaoXing(int itemID, int num, int kind)
        {
            if (itemID == 0)
            {
                return null;
            }
            ItemSystem.Loaders.Item herb = ItemSystem.Loaders.Items.getByItemID(itemID);

            return new YaoXing(
                kind: (int)herb.GetType().GetProperty($"yaoZhi{kind}").GetValue(herb),
                intensity: QualityFactor[herb.quality] * num
            );
        }
    }

    public class Herbs : BaseModel
    {

        static public WuPingType TYPE = WuPingType.草药;

        public YaoXing intro;
        public YaoXing main;
        public YaoXing deputy;

        public Herbs(int itemID) : base(itemID)
        {

            
            intro = YaoXing.GetHerbsYaoXing(itemID, 1, 1);
            main = YaoXing.GetHerbsYaoXing(itemID, 1, 2);
            deputy = YaoXing.GetHerbsYaoXing(itemID, 1, 3);
        }
    }
}