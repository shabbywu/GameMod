using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ItemSystem;

namespace ItemSystem.Loaders
{
    [Serializable]
    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }
        public int ItemIcon { get; set; }
        public int maxNum { get; set; }
        public WuPingType type { get; set; }
        public int quality { get; set; }
        public int price { get; set; }
        public string desc { get; set; }
        public string desc2 { get; set; }
        public int CanSale { get; set; }
        public int CanUse { get; set; }
        public int NPCCanUse { get; set; }
        public List<int> seid { get; set; }
        public List<int> ItemFlag { get; set; }
        public List<int> Affix { get; set; }
        public TuJianType TuJianType { get; set; }
        public int ShopType { get; set; }
        public string FaBaoType { get; set; }
        public string WuWeiType { get; set; }
        public string ShuXingType { get; set; }
        public string typePinJie { get; set; }
        public string StuTime { get; set; }
        public string vagueType { get; set; }
        public string DanDu { get; set; }
        public int yaoZhi1 { get; set; }
        public int yaoZhi2 { get; set; }
        public int yaoZhi3 { get; set; }
        public List<string> wuDao { get; set; }

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(WuPingType), type)}<{id}: {name}>";
        }
    }

    public class Items
    {
        private static Items instance = null;
        private static readonly object _lock = new object();
        private static string path = "Effect/json/d_items.py.datas";

        public IDictionary<int, Item> items;

        public static Item getByItemID(int itemID)
        {
            return Instance.items[itemID];
        }

        public static IEnumerable<Item> filterByType(WuPingType type)
        {
            foreach(var kv in Instance.items) {
                if (kv.Value.type == type) {
                    yield return kv.Value;
                }
            }
        }

        private Items()
        {
            items = new Dictionary<int, Item> { };
            string data = ModResources.LoadText(path);
            Dictionary<string, Item> loaded = JsonConvert.DeserializeObject<Dictionary<string, Item>>(data);
            foreach (KeyValuePair<string, Item> kv in loaded)
            {
                items[kv.Value.id] = kv.Value;
            }
        }

        public static Items Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = new Items();
                        }
                    }
                }
                return instance;
            }
        }
    }
}
