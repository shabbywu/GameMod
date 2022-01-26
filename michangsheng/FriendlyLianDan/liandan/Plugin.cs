using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using Tab;
using System.Collections.Generic;
using ItemSystem.Models;

namespace FriendlyLianDan
{
    [BepInPlugin("cn.shabywu.michangsheng.FriendlyLianDan", "更友好的炼丹", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        public delegate void Log(object data);
        public static Log LogDebug;
        static ConfigEntry<bool> OnlyShowProductable;
        static ConfigEntry<bool> GenerateAllDanFang;
        static ConfigEntry<bool> OrderByHerbsCost;
        static ConfigEntry<int> MaxHerbsNum;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            GenerateAllDanFang = Config.Bind("DanFangGenerator", "GenerateAllDanFang", true, "生成所有药性符合的丹方");
            MaxHerbsNum = Config.Bind("DanFangGenerator", "MaxHerbsNum", 12, "丹方生成器最大的药草数量");

            OnlyShowProductable = Config.Bind("DanFangHelper", "OnlyShowProductable", true, "仅展示可炼制的丹方");
            OrderByHerbsCost = Config.Bind("DanFangHelper", "OrderByHerbsCost", true, "丹方根据药草的成本排序");

            Harmony.CreateAndPatchAll(typeof(Plugin));
            Harmony.CreateAndPatchAll(typeof(PatchDanFangList.PatchGetNoSameDanFangList));
            Harmony.CreateAndPatchAll(typeof(PatchDanFangList));
            LogDebug = Logger.LogDebug;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                UToolTip.Close();
            }
        }

        [HarmonyPatch(typeof(DanFangParentCell), "init")]
        [HarmonyPostfix]
        static void PatchDanFangParentCellInit(ref DanFangParentCell __instance)
        {
            DanFangParentCell instance = __instance;
            if (__instance.DanFangID != -1)
            {
                TabListener listener = __instance.gameObject.GetComponent<TabListener>();
                if (listener == null)
                {
                    listener = __instance.gameObject.AddComponent<TabListener>();
                }
                listener.mouseEnterEvent.AddListener(() =>
                {
                    var item = new GUIPackage.item(instance.DanFangID);
                    UToolTip.OpenItemTooltip(item, item.GetItemPrice());
                });
                listener.mouseOutEvent.AddListener(() =>
                {
                    UToolTip.Close();
                });
            }
        }

        [HarmonyPatch(typeof(GUIPackage.Inventory2), "Show_Tooltip")]
        [HarmonyPostfix]
        public static void PatchInventory2Show_Tooltip(GUIPackage.Inventory2 __instance, ref GUIPackage.item Item)
        {
            TooltipItem component = __instance.Tooltip.GetComponent<TooltipItem>();

            var itemNumText = string.Format("已有:{0}\n", Tools.instance.getPlayer().getItemNum(Item.itemID));
            if (!component.Label2.text.StartsWith("已有"))
            {
                component.Label2.text = itemNumText + component.Label2.text;
            }
        }

        public class PatchDanFangList
        {
            [HarmonyPatch(typeof(DanFangPageManager), "getNoSameDanFangList")]
            public class PatchGetNoSameDanFangList
            {
                public static bool Prefix(ref DanFangPageManager __instance, ref Dictionary<int, List<JSONObject>> __result, ref DanFangPageManager.DanFangPingJie ___danFangPingJie)
                {
                    LogDebug($"Calling getNoSameDanFangList: {GenerateAllDanFang.Value}");

                    if (!GenerateAllDanFang.Value)
                    {
                        return true;
                    }

                    __result = new Dictionary<int, List<JSONObject>>();

                    if (___danFangPingJie == DanFangPageManager.DanFangPingJie.所有)
                    {
                        __result = AllDanFangs.Instance.danfangs;
                    }
                    else
                    {
                        __result = new Dictionary<int, List<JSONObject>>();
                        foreach (var kv in AllDanFangs.Instance.danfangs)
                        {
                            var item = ItemSystem.Loaders.Items.GetByItemID(kv.Key);
                            if ((int)___danFangPingJie == item.quality)
                            {
                                __result.Add(kv.Key, kv.Value);
                            }
                        }
                    }
                    return false;
                }

                public static void Postfix(ref DanFangPageManager __instance, ref Dictionary<int, List<JSONObject>> __result)
                {
                    // 过滤所有可生产的丹方;
                    if (OnlyShowProductable.Value)
                    {
                        var result = new Dictionary<int, List<JSONObject>>();
                        var instance = __instance;
                        foreach (var kv in __result)
                        {
                            var value = kv.Value.FindAll(item => instance.checkCanLianZhi(item));
                            if (value.Count > 0)
                            {
                                result[kv.Key] = value;
                            }
                            else
                            {
                                // 无法炼制, 展示默认丹方
                                result[kv.Key] = new List<JSONObject>() { ItemSystem.Loaders.DanFangs.GetByDanYaoID(kv.Key).ToJSONObject() };
                            }
                        }
                        __result = result;
                    }

                    // 按照材料的价格排序
                    if (OrderByHerbsCost.Value)
                    {
                        foreach (var kv in __result)
                        {
                            kv.Value.Sort(delegate (JSONObject a, JSONObject b)
                            {
                                return CalculateDanFangCost(a).CompareTo(CalculateDanFangCost(b));
                            });
                        }
                    }
                }

                [HarmonyPatch(typeof(DanFangPageManager), "addDanFang")]
                [HarmonyPrefix]
                public static bool patchAddDanFang(ref JSONObject obj)
                {
                    // 如果自动生成丹方, 那么新的单方仅加入丹方清单, 不在 UI 展示
                    if (GenerateAllDanFang.Value)
                    {
                        if (!hasSameDanFang(obj, Tools.instance.getPlayer().DanFang.list))
                        {
                            Tools.instance.getPlayer().DanFang.list.Add(obj);
                        }
                        return false;
                    }
                    return true;
                }
            }

            [HarmonyPatch(typeof(DanFangChildCell), "updateState")]
            [HarmonyPostfix]
            static void PatchDanFangChildCell(ref DanFangChildCell __instance, ref GameObject ___CanLianZhiImage)
            {
                if (__instance.danFang != null)
                {
                    if (!hasSameDanFang(__instance.danFang, Tools.instance.getPlayer().DanFang.list))
                    {
                        // 未学习的丹方转灰色
                        var i = ___CanLianZhiImage.transform.parent.GetComponent<UnityEngine.UI.Image>();
                        i.color = UnityEngine.Color.cyan;
                    }
                }
            }
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
                                LogDebug($"正在初始化 AllDanFangs: {MaxHerbsNum.Value}");
                                instance = new AllDanFangs(MaxHerbsNum.Value);
                            }
                        }
                    }
                    return instance;
                }
            }
        }

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
    }
}
