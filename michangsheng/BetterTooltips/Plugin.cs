using System;
using System.Text;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace BetterTooltips
{
    [BepInPlugin("cn.shabywu.michangsheng.BetterTooltips", "更丰富的弹窗信息", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> ShowYaoYin;
        public static ConfigEntry<bool> ShowChandi;

        public static Harmony _harmony;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            ShowYaoYin = Config.Bind("Tooltip", "ShowYaoYin", false, "是否展示药草可炼制的丹方");
            ShowChandi = Config.Bind("Tooltip", "ShowChandi", false, "是否展示药草的产地");

            _harmony = Harmony.CreateAndPatchAll(typeof(PatchToolTipsMag));
            LogDebug = Logger.LogDebug;
        }

        private void OnDestroy() {
            _harmony.UnpatchSelf();
        }

        public delegate void Log(object data);
        public static Log LogDebug;

        class PatchToolTipsMag{
            static bool showed = false;

            [HarmonyPatch(typeof(ToolTipsMag), "UpdateSize")]
            [HarmonyPrefix]
            static public void PatchUpdateSize(ref ToolTipsMag __instance, ref ToolTipsMag.Direction ____direction, ref Dictionary<int, string> ____qualityNameColordit)
            {
                Bag.BaseItem item = __instance.BaseItem;
                // 只要展示的不是草药, 就都不处理
                if (item == null)
                {
                    LogDebug("item is null");
                    return;
                }

                if (item.ItemType == Bag.ItemType.草药 && !showed)
                {
                    CaoYaoHandler.ShowPossibleDanFang(ref __instance, ref ____direction, ref ____qualityNameColordit);
                    if (ShowChandi.Value) {
                        CaoYaoHandler.ShowPossibleChanDi(ref __instance, ref ____direction);
                    }
                    showed = true;
                }
            }

            [HarmonyPatch(typeof(ToolTipsMag), "UpdateSize")]
            [HarmonyPrefix]
            static public bool PatchClose() {
                showed = false;
                return true;
            }
        }
    }
}
