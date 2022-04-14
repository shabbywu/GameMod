using System;
using System.Text;
using System.Collections.Generic;
using JSONClass;

namespace ItemSystem.Shims
{
    [Serializable]
    public class DanFang
    {
        public int id { get; set; }
        public string name { get; set; }
        public int ItemID { get; set; }
        public int castTime { get; set; }
        public int value1 { get; set; }
        public int value2 { get; set; }
        public int value3 { get; set; }
        public int value4 { get; set; }
        public int value5 { get; set; }
        public int num1 { get; set; }
        public int num2 { get; set; }
        public int num3 { get; set; }
        public int num4 { get; set; }
        public int num5 { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"丹方<{ItemID} {name}>{{\n", 64);
            if (value1 != 0)
            {
                sb.AppendFormat("\t药引id: {0}, 用量: {1}\n", Items.GetByItemID(value1).ToString(), num1);
            }
            if (value2 != 0)
            {
                sb.AppendFormat("\t主药1: {0}, 用量: {1}\n", Items.GetByItemID(value2).ToString(), num2);
            }
            if (value3 != 0)
            {
                sb.AppendFormat("\t主药1: {0}, 用量: {1}\n", Items.GetByItemID(value3).ToString(), num3);
            }
            if (value4 != 0)
            {
                sb.AppendFormat("\t副药1: {0}, 用量: {1}\n", Items.GetByItemID(value4).ToString(), num4);
            }
            if (value5 != 0)
            {
                sb.AppendFormat("\t副药2: {0}, 用量: {1}\n", Items.GetByItemID(value5).ToString(), num5);
            }
            sb.Append("}");
            return sb.ToString();
        }

        // 转换为游戏中 DanFang.list 中存储的 JSONObject 对象
        public JSONObject ToJSONObject()
        {
            JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject values = new JSONObject(JSONObject.Type.ARRAY);
            JSONObject nums = new JSONObject(JSONObject.Type.ARRAY);

            foreach (var v in new int[] { value1, value2, value3, value4, value5 })
            {
                values.Add(v);
            }

            foreach (var n in new int[] { num1, num2, num3, num4, num5 })
            {
                nums.Add(n);
            }

            int cost = 0;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].I != 0)
                {
                    cost += Items.GetByItemID(values[i].I).price * nums[i].I;
                }
            }

            jsonobject.AddField("ID", ItemID);
            jsonobject.AddField("Type", values);
            jsonobject.AddField("Num", nums);

            // extra fields
            jsonobject.AddField("__Cost", cost);

            return jsonobject;
        }
    }

    public class DanFangs
    {
        private static DanFangs instance = null;
        private static readonly object _lock = new object();
        public IDictionary<int, DanFang> danfangs;

        public static IEnumerable<DanFang> List()
        {
            return Instance.danfangs.Values;
        }

        public static DanFang GetByDanYaoID(int itemID)
        {
            return Instance.danfangs[itemID];
        }

        private DanFangs()
        {
            danfangs = new Dictionary<int, DanFang>();
            if (LianDanDanFangBiao.DataList.Count == 0)
            {
                LianDanDanFangBiao.InitDataDict();
            }
            foreach (var item in LianDanDanFangBiao.DataList)
            {
                var danfang = new DanFang();

                danfang.id = item.id;
                danfang.name = item.name;
                danfang.ItemID = item.ItemID;

                danfang.value1 = item.value1;
                danfang.num1 = item.num1;

                danfang.value2 = item.value2;
                danfang.num2 = item.num2;

                danfang.value3 = item.value3;
                danfang.num3 = item.num3;

                danfang.value4 = item.value4;
                danfang.num4 = item.num4;

                danfang.value5 = item.value5;
                danfang.num5 = item.num5;

                danfang.castTime = item.castTime;

                danfangs[item.ItemID] = danfang;
            }
        }

        public static DanFangs Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = new DanFangs();
                        }
                    }
                }
                return instance;
            }
        }
    }
}