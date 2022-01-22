using BepInEx;
using UnityEngine;
using HarmonyLib;
using Tab;
using System.Collections.Generic;
using ItemSystem.Loaders;
using ItemSystem.Models;

namespace FriendlyLianDan
{
    [BepInPlugin("cn.shabywu.michangsheng.FriendlyLianDan", "更友好的炼丹", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            itemDatabase = (ItemDataBaseList)Resources.Load("ItemDatabase");
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Harmony.CreateAndPatchAll(typeof(ShowAllDanFangList));
            LogDebug = Logger.LogDebug;
        }
        private static ItemDataBaseList itemDatabase;

        public delegate void Log(object data);
        public static Log LogDebug;

        public void Update() {
            if (Input.GetKeyDown(KeyCode.Tab)) {
                UToolTip.Close();
            }
        }

        [HarmonyPatch(typeof(DanFangParentCell), "init")]
        [HarmonyPostfix]
        static void PatchDanFangParentCellInit(ref DanFangParentCell __instance)
        {
            DanFangParentCell instance = __instance;
            if (__instance.DanFangID != -1) {
                TabListener listener = __instance.gameObject.GetComponent<TabListener>();
                if (listener == null)
                {
                    listener = __instance.gameObject.AddComponent<TabListener>();
                }
                listener.mouseEnterEvent.AddListener(() =>
                {
                    UToolTip.OpenItemTooltip(new GUIPackage.item(instance.DanFangID));
                });
                listener.mouseOutEvent.AddListener(() =>
                {
                    UToolTip.Close();
                });
            }
        }


        public class ShowAllDanFangList {
            public static Dictionary<int, List<JSONObject>> cache = null;
            public static Dictionary<int, bool> initial = new Dictionary<int, bool>();

            [HarmonyPatch(typeof(DanFangPageManager), "getNoSameDanFangList")]
            [HarmonyPrefix]
            static bool PatchGetNoSameDanFangList(ref Dictionary<int, List<JSONObject>> __result, ref DanFangPageManager.DanFangPingJie ___danFangPingJie) {
                if (cache == null) {
                    cache = new Dictionary<int, List<JSONObject>>();
                    foreach (var danfang in ItemSystem.Loaders.DanFangs.List()) {
                        cache.Add(danfang.ItemID, new List<JSONObject>(){danfang.ToJSONObject()});
                    }
                }
                if (___danFangPingJie == DanFangPageManager.DanFangPingJie.所有) {
                    __result = cache;
                } else {
                    __result = new Dictionary<int, List<JSONObject>>();
                    foreach (var kv in cache) {
                        var item = ItemSystem.Loaders.Items.GetByItemID(kv.Key);
                        if ((int)___danFangPingJie == item.quality) {
                            __result.Add(kv.Key, kv.Value);
                        }
                    } 
                }
                return false;
            }

            [HarmonyPatch(typeof(DanFangParentCell), "clickDanFang")]
            [HarmonyPrefix]
            static bool PatchDanFangParentCellClickDanFang(ref DanFangParentCell __instance, ref GameObject ___DanFangChildCell)
            {
                if (__instance.DanFangID == 0)
                {
                    LogDebug("未知丹方, 忽略");
                }
                var item = Items.GetByItemID(__instance.DanFangID);
                if (initial.ContainsKey(item.id)) {
                    return true;
                }

                var danyao = new DanYao(item.id);
                LogDebug($"正在生成 {danyao} 的所有丹方");

                foreach (var danfang in new Emulator(danyao, maxNum: 14).Generator())
                {
                    var builtinDanFang = danfang.ToJSONObject();
                    if (!hasSameDanFang(builtinDanFang, __instance.childs))
                    {
                        LogDebug($"尝试添加丹方 {danfang}");
                        DanFangChildCell component = Tools.InstantiateGameObject(___DanFangChildCell, ___DanFangChildCell.transform.parent).GetComponent<DanFangChildCell>();
                        __instance.childs.Add(builtinDanFang);
                        component.danFang = __instance.childs[__instance.childs.Count - 1];
                        component.init();
                        __instance.childDanFangChildCellList.Add(component);
                        __instance.childDanFangChildCellList[__instance.finallyIndex].showLine();
                        __instance.finallyIndex++;
                        __instance.childDanFangChildCellList[__instance.finallyIndex].hideLine();
                        __instance.updateState();
                    }
                    else
                    {
                        LogDebug("当前丹方已存在, 跳过.");
                    }
                }

                initial.Add(item.id, true);
                return true;
            }
        }



        public static bool hasSameDanFang(JSONObject obj, List<JSONObject> others)
        {
            foreach (var other in others)
            {
                if (other["ID"].I == obj["ID"].I)
                {
                    int num = 0;
                    for (int j = 0; j < obj.Count; j++)
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
    }
}
