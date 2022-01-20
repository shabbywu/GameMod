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
            Harmony.CreateAndPatchAll(typeof(PatchDanFangParentCellInit));
            Harmony.CreateAndPatchAll(typeof(PatchDanYaoItemUse));
            LogDebug = Logger.LogDebug;
        }
        private static ItemDataBaseList itemDatabase;

        public delegate void Log(object data);
        public static Log LogDebug;

        [HarmonyPatch(typeof(DanFangParentCell), "init")]
        public class PatchDanFangParentCellInit
        {
            static void Postfix(ref DanFangParentCell __instance)
            {
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
        }

        [HarmonyPatch(typeof(Bag.DanYaoItem), "Use")]
        public class PatchDanYaoItemUse
        {
            static void Postfix(ref int ___Id)
            {
                var item = Items.getByItemID(___Id);

                if (item.type == ItemSystem.WuPingType.丹药)
                {
                    LogDebug($"使用了 {item}");
                    var danyao = new DanYao(item.id);
                    LogDebug($"模板丹方 {danyao.danfang}");
                    foreach (var danfang in new Emulator(danyao, maxNum: 12).Generator())
                    {
                        LogDebug($"尝试添加丹方 {danfang}");
                        Tools.instance.getPlayer().addDanFang(danfang.ItemID,
                        new List<int> { danfang.value1, danfang.value2, danfang.value3, danfang.value4, danfang.value5 },
                        new List<int> { danfang.num1, danfang.num2, danfang.num3, danfang.num4, danfang.num5 });
                    }
                }
            }
        }

        // 推演所有丹方
        public void CalculateAllDanFang(int danyaoID)
        {
            if (!jsonData.instance.LianDanDanFangBiao.HasField(danyaoID.ToString()))
            {
                LogDebug($"丹方出错丹方表ID {danyaoID} 不存在");
                return;
            }


        }

    }
}
