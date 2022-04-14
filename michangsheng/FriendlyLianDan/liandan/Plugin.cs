using BepInEx;
using BepInEx.Configuration;
using Bag;
using HarmonyLib;
using JSONClass;
using System.Collections.Generic;
using ItemSystem.Models;
using System.Diagnostics;
using script.NewLianDan;
using script.NewLianDan.DanFang;

namespace FriendlyLianDan
{
    [BepInPlugin("cn.shabywu.michangsheng.FriendlyLianDan", "更友好的炼丹", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public delegate void Log(object data);
        public static Log LogDebug;
        static ConfigEntry<bool> OnlyShowProductable;
        static ConfigEntry<bool> GenerateAllDanFang;
        static ConfigEntry<bool> OrderByHerbsCost;
        static ConfigEntry<int> MaxHerbsNum;
        static List<Harmony> _harmonys = new List<Harmony>();

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            GenerateAllDanFang = Config.Bind("DanFangGenerator", "GenerateAllDanFang", true, "生成所有药性符合的丹方");
            MaxHerbsNum = Config.Bind("DanFangGenerator", "MaxHerbsNum", 12, "丹方生成器最大的药草数量");

            OnlyShowProductable = Config.Bind("DanFangHelper", "OnlyShowProductable", true, "仅展示可炼制的丹方");
            OrderByHerbsCost = Config.Bind("DanFangHelper", "OrderByHerbsCost", true, "丹方根据药草的成本排序");

            _harmonys.Add(Harmony.CreateAndPatchAll(typeof(Plugin)));
            LogDebug = Logger.LogDebug;
        }

        private void OnDestroy()
        {
            foreach (var harmony in _harmonys)
            {
                try
                {
                    harmony.UnpatchSelf();
                }
                catch (System.Exception e)
                {
                    LogDebug($"Failto Unpatch Harmony for reason: {e.Message}");
                }
            }
        }

        [HarmonyPatch(typeof(DanFangPanel), "UpdateDanFangList")]
        [HarmonyPrefix]
        public static bool Prefix(ref DanFangPanel __instance)
        {

            Stopwatch sw = new Stopwatch();
            Dictionary<int, List<JSONObject>> danfangs = null;

            if (GenerateAllDanFang.Value)
            {
                sw.Start();
                danfangs = AllDanFangs.Instance.danfangs;
                sw.Stop();
                LogDebug($"[UpdateDanFangList] Generate AllDanFangs cost: {sw.ElapsedMilliseconds}ms");
            }
            else
            {
                sw.Start();
                danfangs = new Dictionary<int, List<JSONObject>>();
                foreach (var jsonobject in Tools.instance.getPlayer().DanFang.list)
                {
                    int id = jsonobject["ID"].I;
                    if (danfangs.ContainsKey(id))
                    {
                        danfangs[id].AddItem(jsonobject);
                    }
                    else
                    {
                        danfangs.Add(id, new List<JSONObject>() { jsonobject });
                    }
                }
                sw.Stop();
                LogDebug($"[UpdateDanFangList] Transfer User's Danfangs cost: {sw.ElapsedMilliseconds}ms");
            }

            sw.Restart();
            __instance.Clear();
            __instance.CurBigDanFang = null;
            __instance.CurSmallDanFang = null;
            __instance.DanFangList = new List<BigDanFang>();
            Dictionary<int, DanFangData> dictionary = new Dictionary<int, DanFangData>();

            foreach (var kv in danfangs)
            {
                int id = kv.Key;
                foreach (var jsonobject in kv.Value)
                {
                    // 根据品质过滤丹方
                    if (__instance.CurQuality != 0 && _ItemJsonData.DataDict[id].quality != __instance.CurQuality) goto skip;
                    // 过滤无法炼制的丹方
                    if (OnlyShowProductable.Value && !LianDanUIMag.Instance.CheckCanLianZhi(jsonobject)) continue;

                    DanFangBase danFangBase = jsonobject.ToDanFangBase();
                    if (!dictionary.ContainsKey(id))
                    {
                        dictionary.Add(id, new DanFangData
                        {
                            Id = id,
                            Name = _ItemJsonData.DataDict[id].name,
                            DanFangBases = new List<DanFangBase>()
                        });
                    }
                    dictionary[id].DanFangBases.Add(danFangBase);
                }

                // 无法炼制, 展示默认丹方
                if (OnlyShowProductable.Value && !dictionary.ContainsKey(id))
                {
                    dictionary.Add(id, new DanFangData
                    {
                        Id = id,
                        Name = _ItemJsonData.DataDict[id].name,
                        DanFangBases = new List<DanFangBase>() { ItemSystem.Loaders.DanFangs.GetByDanYaoID(id).ToJSONObject().ToDanFangBase() }
                    });
                }

                skip:;
            }

            LogDebug($"[UpdateDanFangList] List AllDanFangs cost: {sw.ElapsedMilliseconds}ms");

            // 按照材料的价格排序
            if (OrderByHerbsCost.Value)
            {
                sw.Restart();
                foreach (var kv in dictionary)
                {
                    kv.Value.DanFangBases.Sort(delegate (DanFangBase a, DanFangBase b)
                    {
                        return CalculateDanFangCost(a.Json).CompareTo(CalculateDanFangCost(b.Json));
                    });
                }
                sw.Stop();
                LogDebug($"[UpdateDanFangList] Order DanFang By HerbsCost cost: {sw.ElapsedMilliseconds}ms");
            }

            sw.Restart();
            foreach (int id in dictionary.Keys)
            {
                __instance.DanFangList.Add(new BigDanFang(__instance.DanFangTemp.Inst(__instance.DanFangTemp.transform.parent), dictionary[id], BaseItem.Create(id, 1, Tools.getUUID(), Tools.CreateItemSeid(id))));
            }
            __instance.UpdatePosition();
            sw.Stop();

            LogDebug($"[UpdateDanFangList] Update UI cost: {sw.ElapsedMilliseconds}ms");
            return false;
        }

        [HarmonyPatch(typeof(DanFangPanel), "AddDanFang")]
        [HarmonyPrefix]
        public static bool patchAddDanFang(ref JSONObject json)
        {
            // 如果自动生成丹方, 那么新的单方仅加入丹方清单, 不在 UI 展示
            if (GenerateAllDanFang.Value)
            {
                if (!hasSameDanFang(json, Tools.instance.getPlayer().DanFang.list))
                {
                    Tools.instance.getPlayer().DanFang.list.Add(json);
                }
                return false;
            }
            return true;
        }

        // [HarmonyPatch(typeof(SmallDanFang), "updateState")]
        // [HarmonyPostfix]
        // static void PatchDanFangChildCell(ref SmallDanFang __instance)
        // {
        //     if (__instance.DanFangData != null)
        //     {
        //         if (!hasSameDanFang(__instance.DanFangData.Json, Tools.instance.getPlayer().DanFang.list))
        //         {
        //             // 未学习的丹方转灰色
        //             var i = __instance.SmallDanLu.transform.parent.GetComponent<UnityEngine.UI.Image>();
        //             i.color = UnityEngine.Color.cyan;
        //         }
        //     }
        // }


        public static bool hasSameDanFang(JSONObject obj, List<JSONObject> others)
        {
            foreach (var other in others)
            {
                if (other["ID"].I == obj["ID"].I)
                {
                    int num = 0;
                    for (int j = 0; j < obj["Type"].Count; j++)
                    {
                        if (other["Type"][j].I == obj["Type"][j].I && other["Num"][j].I == obj["Num"][j].I)
                        {
                            num++;
                        }
                    }
                    if (num == 5)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static int CalculateDanFangCost(JSONObject obj)
        {
            if (obj.HasField("__Cost"))
            {
                return obj["__Cost"].I;
            }

            int cost = 0;
            for (int i = 0; i < obj.Count; i++)
            {
                if (obj["Type"][i].I != 0)
                {
                    cost += ItemSystem.Loaders.Items.GetByItemID(obj["Type"][i].I).price * obj["Num"][i].I;
                }
            }
            obj.AddField("__Cost", cost);
            return cost;
        }


        public class AllDanFangs
        {
            private static AllDanFangs instance = null;
            private static readonly object _lock = new object();
            public Dictionary<int, List<JSONObject>> danfangs = null;
            public int maxNum;
            private AllDanFangs(int maxNum = 14)
            {
                this.maxNum = maxNum;
                danfangs = new Dictionary<int, List<JSONObject>>();
                foreach (var item in ItemSystem.Loaders.Items.FilterByType(ItemSystem.WuPingType.丹药))
                {
                    var danyao = new DanYao(item.id);
                    foreach (var danfang in new Emulator(danyao, maxNum: maxNum).Generator())
                    {
                        var builtinDanFang = danfang.ToJSONObject();
                        if (!danfangs.ContainsKey(danyao.id))
                        {
                            danfangs.Add(danyao.id, new List<JSONObject>());
                        }
                        danfangs[danyao.id].Add(danfang.ToJSONObject());
                    }
                }
            }

            public static AllDanFangs Instance
            {
                get
                {
                    if (instance == null || instance.maxNum != MaxHerbsNum.Value)
                    {
                        lock (_lock)
                        {
                            if (instance == null || instance.maxNum != MaxHerbsNum.Value)
                            {
                                Stopwatch sw = new Stopwatch();
                                sw.Start();
                                LogDebug($"正在初始化 AllDanFangs: {MaxHerbsNum.Value}");
                                instance = new AllDanFangs(MaxHerbsNum.Value);
                                sw.Stop();
                                LogDebug($"初始化 AllDanFangs 成功, 耗时: {sw.ElapsedMilliseconds}ms");
                            }

                        }
                    }
                    return instance;
                }
            }
        }
    }

}

static class ExtendMethods
{
    public static DanFangBase ToDanFangBase(this JSONObject jsonobject)
    {
        DanFangBase danFangBase = new DanFangBase();
        if (jsonobject["Type"][0].I > 0)
        {
            danFangBase.YaoYin.Add(jsonobject["Type"][0].I, jsonobject["Num"][0].I);
        }
        if (jsonobject["Type"][1].I > 0)
        {
            danFangBase.ZhuYao1.Add(jsonobject["Type"][1].I, jsonobject["Num"][1].I);
            danFangBase.ZhuYaoYaoXin1 = _ItemJsonData.DataDict[jsonobject["Type"][1].I].yaoZhi2;
        }
        if (jsonobject["Type"][2].I > 0)
        {
            danFangBase.ZhuYao2.Add(jsonobject["Type"][2].I, jsonobject["Num"][2].I);
            danFangBase.ZhuYaoYaoXin2 = _ItemJsonData.DataDict[jsonobject["Type"][2].I].yaoZhi2;
        }
        if (jsonobject["Type"][3].I > 0)
        {
            danFangBase.FuYao1.Add(jsonobject["Type"][3].I, jsonobject["Num"][3].I);
            danFangBase.FuYaoYaoXin1 = _ItemJsonData.DataDict[jsonobject["Type"][3].I].yaoZhi3;
        }
        if (jsonobject["Type"][4].I > 0)
        {
            danFangBase.FuYao2.Add(jsonobject["Type"][4].I, jsonobject["Num"][4].I);
            danFangBase.FuYaoYaoXin2 = _ItemJsonData.DataDict[jsonobject["Type"][4].I].yaoZhi3;
        }
        danFangBase.Json = jsonobject.Copy();
        return danFangBase;
    }
}
