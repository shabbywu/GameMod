using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using TheLastStand.Controller.Panic;
using TheLastStand.Model.Unit;
using TheLastStand.Manager;

namespace MultipleGain
{
    [BepInPlugin("cn.shabywu.the_last_stand.multiple_gain", "多倍收益", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<float> ExperiencePercentage;
        static ConfigEntry<float> DamnedSoulsPercentage;
        static ConfigEntry<float> PanicRewardGoldPercentage;
        static ConfigEntry<float> PanicRewardMaterialsPercentage;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            ExperiencePercentage = Config.Bind("TheLastStand.MultipleGain",  "ExperiencePercentage", 1.5f, "经验倍率");
            DamnedSoulsPercentage = Config.Bind("TheLastStand.MultipleGain",  "DamnedSoulsPercentage", 10f, "污秽精华倍率");
            PanicRewardGoldPercentage = Config.Bind("TheLastStand.MultipleGain",  "PanicRewardGoldPercentage", 1.0f, "金币倍率");
            PanicRewardMaterialsPercentage = Config.Bind("TheLastStand.MultipleGain",  "PanicRewardMaterialsPercentage", 1.0f, "材料倍率");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(Unit), "ExperienceGain", MethodType.Getter)]
        [HarmonyPostfix]
        static void patchUnitExperienceGain(ref float __result)
        {
            __result *= ExperiencePercentage.Value;
            Console.WriteLine($"patch ExperienceGain: {__result}");
        }

        [HarmonyPatch(typeof(TrophyManager), "AddEnemyKill")]
        [HarmonyPrefix]
        static bool patchTrophyManagerAddEnemyKill(ref int damnedSoulsEarned)
        {
            damnedSoulsEarned = Convert.ToInt32(damnedSoulsEarned * DamnedSoulsPercentage.Value);
            Console.WriteLine($"patch AddEnemyKill: {damnedSoulsEarned}");
            return true;
        }

        [HarmonyPatch(typeof(PanicRewardController), "GetReward")]
        [HarmonyPostfix]
        static void patchGetReward(ref PanicRewardController __instance)
        {
            __instance.PanicReward.Gold = Convert.ToInt32(__instance.PanicReward.Gold * PanicRewardGoldPercentage.Value);
            __instance.PanicReward.Materials = Convert.ToInt32(__instance.PanicReward.Materials * PanicRewardMaterialsPercentage.Value);
        }
    }

}
