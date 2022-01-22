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
            LogDebug = Logger.LogDebug;
        }
        private static ItemDataBaseList itemDatabase;

        public delegate void Log(object data);
        public static Log LogDebug;

        [HarmonyPatch(typeof(DanFangParentCell), "init")]
        [HarmonyPostfix]
        static void PatchDanFangParentCellInit(ref DanFangParentCell __instance)
        {
            // TODO: 重构成 UIIconShow
            DanFangParentCell instance = __instance;
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

        [HarmonyPatch(typeof(DanFangParentCell), "clickDanFang")]
        [HarmonyPrefix]
        static void PatchDanFangParentCellClickDanFang(ref DanFangParentCell __instance, ref GameObject ___DanFangChildCell)
        {
            if (__instance.DanFangID == 0)
            {
                LogDebug("未知丹方, 忽略");
            }
            var item = Items.getByItemID(__instance.DanFangID);
            LogDebug($"点击了 {item}");
            var danyao = new DanYao(item.id);

            foreach (var danfang in new Emulator(danyao, maxNum: 12).Generator())
            {
                var builtinDanFang = danfang.toJSONObject();
                if (!hasSameDanFang(builtinDanFang))
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
        }

        public static bool hasSameDanFang(JSONObject obj)
        {
            foreach (var other in Tools.instance.getPlayer().DanFang.list)
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
