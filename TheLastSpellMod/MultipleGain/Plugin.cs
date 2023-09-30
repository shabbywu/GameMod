using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using TheLastStand.Model.Unit;
using TheLastStand.Manager;

namespace MultipleGain
{
    [BepInPlugin("cn.shabywu.the_last_stand.multiple_gain", "多倍收益", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<float> ExperiencePercentage;
        static ConfigEntry<float> DamnedSoulsPercentage;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            ExperiencePercentage = Config.Bind("TheLastStand.MultipleGain",  "ExperiencePercentage", 1.5f, "经验倍率");
            DamnedSoulsPercentage = Config.Bind("TheLastStand.MultipleGain",  "DamnedSoulsPercentage", 10f, "污秽精华倍率");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(Unit), "ExperienceGain", MethodType.Getter)] // Specify target method with HarmonyPatch attribute
        [HarmonyPostfix]
        static void patchUnitExperienceGain(ref float __result)
        {
            __result *= ExperiencePercentage.Value;
            Console.WriteLine($"patch ExperienceGain: {__result}");
        }

        [HarmonyPatch(typeof(TrophyManager), "AddEnemyKill")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]
        static bool patchTrophyManagerAddEnemyKill(ref int damnedSoulsEarned)
        {
            damnedSoulsEarned = Convert.ToInt32(damnedSoulsEarned * DamnedSoulsPercentage.Value);
            Console.WriteLine($"patch AddEnemyKill: {damnedSoulsEarned}");
            return true;
        }
    }

}
