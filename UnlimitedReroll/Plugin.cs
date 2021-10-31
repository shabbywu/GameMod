using BepInEx;
using HarmonyLib;
using TheLastStand.Controller.Unit;

namespace UnlimitedReroll
{
    [BepInPlugin("cn.shabywu.the_last_stand.unlimited_reroll", "Unlimited Reroll", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(UnitLevelUpController), "DrawAvailableMainStats")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool DrawAvailableMainStats(ref UnitLevelUpController __instance)
        {
            __instance.DrawAvailableStats(true);
            return false; // Returning false in prefix patches skips running the original code
        }

        [HarmonyPatch(typeof(UnitLevelUpController), "DrawAvailableSecondaryStats")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool DrawAvailableSecondaryStats(ref UnitLevelUpController __instance)
        {
            __instance.DrawAvailableStats(false);
            return false; // Returning false in prefix patches skips running the original code
        }
    }
}
