using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using TheLastStand.Controller.Unit;
using TheLastStand.Controller.Panic;
using TheLastStand.Controller.Building;
using TheLastStand.Database;
using TheLastStand.Database.Building;
using TheLastStand.Model.Building;

namespace UnlimitedReroll
{
    [BepInPlugin("cn.shabywu.the_last_stand.unlimited_reroll", "无限重投", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {

        static ConfigEntry<bool> UnlimitedMainStatsReroll;
        static ConfigEntry<bool> UnlimitedSecondaryStatsReroll;
        static ConfigEntry<int> ExtraPanicRewardRerollNumber;
        static ConfigEntry<bool> UnlimitedShopReroll;
        static ConfigEntry<bool> OnlyDisableShopRerollPriceIncrement;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            UnlimitedMainStatsReroll = Config.Bind("TheLastStand.UnlimitedReroll",  "UnlimitedMainStatsReroll", true, "主要属性无限刷新");
            UnlimitedSecondaryStatsReroll = Config.Bind("TheLastStand.UnlimitedReroll",  "UnlimitedSecondaryStatsReroll", true, "次要属性无限刷新");

            ExtraPanicRewardRerollNumber = Config.Bind("TheLastStand.UnlimitedReroll",  "ExtraPanicRewardRerollNumber", 10, "每晚奖励额外重摇次数");

            OnlyDisableShopRerollPriceIncrement = Config.Bind("TheLastStand.UnlimitedReroll",  "OnlyDisableShopRerollPriceIncrement", true, "仅禁用商店刷新价格递增");
            UnlimitedShopReroll = Config.Bind("TheLastStand.UnlimitedReroll",  "UnlimitedShopReroll", false, "商店刷新无消耗(优先级高于`禁用商店价格刷新`)");

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        // 主要属性刷新
        [HarmonyPatch(typeof(UnitLevelUpController), "DrawAvailableMainStats")]
        [HarmonyPrefix]
        static bool DrawAvailableMainStats(ref UnitLevelUpController __instance)
        {
            if (UnlimitedMainStatsReroll.Value) {
                __instance.DrawAvailableStats(true);
                return false;
            }
            return true;
        }

        // 次要属性刷新
        [HarmonyPatch(typeof(UnitLevelUpController), "DrawAvailableSecondaryStats")]
        [HarmonyPrefix]
        static bool DrawAvailableSecondaryStats(ref UnitLevelUpController __instance)
        {
            if (UnlimitedSecondaryStatsReroll.Value) {
                __instance.DrawAvailableStats(false);
                return false;
            }
            return true;
        }

        // 每晚奖励额外重摇次数
        [HarmonyPatch(typeof(PanicRewardController), "ReloadBaseNbRerollReward")]
        [HarmonyPostfix]
        static void PatchReloadBaseNbRerollReward(ref PanicRewardController __instance, ref int __result)
        {
            __result += ExtraPanicRewardRerollNumber.Value;
            __instance.PanicReward.BaseNbRerollReward = __result;
        }

        // 商品刷新价格不变化
        [HarmonyPatch(typeof(Shop), "ShopRerollPrice", MethodType.Getter)]
        [HarmonyPrefix]
        static bool PatchShopRerollPrice(ref Shop __instance, ref int __result)
        {
            if (UnlimitedShopReroll.Value) {
                __result = 0;
                return false;
            }
            if (OnlyDisableShopRerollPriceIncrement.Value) {
                __result = BuildingDatabase.ShopDefinition.RerollPrices[0];
                return false;
            }
            return true;
        }
    }
}
