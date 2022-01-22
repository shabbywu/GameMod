using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using ItemSystem;

namespace ItemSystem.Loaders
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
        public override string ToString () {
            StringBuilder sb = new StringBuilder($"丹方<{ItemID} {name}>{{\n", 64);
            if (value1 != 0) {
                sb.AppendFormat("\t药引id: {0}, 用量: {1}\n", Items.GetByItemID(value1).ToString(), num1);
            }
            if (value2 != 0) {
                sb.AppendFormat("\t主药1: {0}, 用量: {1}\n", Items.GetByItemID(value2).ToString(), num2);
            }
            if (value3 != 0) {
                sb.AppendFormat("\t主药1: {0}, 用量: {1}\n", Items.GetByItemID(value3).ToString(), num3);
            }
            if (value4 != 0) {
                sb.AppendFormat("\t副药1: {0}, 用量: {1}\n", Items.GetByItemID(value4).ToString(), num4);
            }
            if (value5 != 0) {
                sb.AppendFormat("\t副药2: {0}, 用量: {1}\n", Items.GetByItemID(value5).ToString(), num5);
            }
            sb.Append("}");
            return sb.ToString();
        }

        // 转换为游戏中 DanFang.list 中存储的 JSONObject 对象
        public JSONObject ToJSONObject () {
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			JSONObject values = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject nums = new JSONObject(JSONObject.Type.ARRAY);

            foreach(var v in new int[]{value1, value2, value3, value4, value5}){
                values.Add(v);
            }

            foreach(var n in new int[]{num1, num2, num3, num4, num5}){
                nums.Add(n);
            }

			jsonobject.AddField("ID", ItemID);
			jsonobject.AddField("Type", values);
			jsonobject.AddField("Num", nums);
            return jsonobject;
        }
    }

    public class DanFangs
    {
        private static DanFangs instance = null;
        private static readonly object _lock = new object();
        private static string path = "Effect/json/d_LianDan.py.DanFangBiao";
        public IDictionary<int, DanFang> danfangs;

        public static IEnumerable<DanFang> List() {
            return Instance.danfangs.Values;
        }

        public static DanFang GetByDanYaoID(int itemID)
        {
            return Instance.danfangs[itemID];
        }

        private DanFangs()
        {
            danfangs = new Dictionary<int, DanFang>();
            string data = ModResources.LoadText(path);
            Dictionary<string, DanFang> loaded = JsonConvert.DeserializeObject<Dictionary<string, DanFang>>(data);
            foreach (KeyValuePair<string, DanFang> kv in loaded)
            {
                danfangs[kv.Value.ItemID] = kv.Value;
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