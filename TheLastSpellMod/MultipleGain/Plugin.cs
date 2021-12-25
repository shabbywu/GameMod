using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using TheLastStand.Definition.Unit.Enemy;

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

        [HarmonyPatch(typeof(EnemyUnitTemplateDefinition), "ExperienceGain", MethodType.Setter)] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool patchExperienceGain(ref float value)
        {
            value *= ExperiencePercentage.Value;
            Console.WriteLine($"patchExperienceGain: {value}");
            return true;
        }

        [HarmonyPatch(typeof(EnemyUnitTemplateDefinition), "DamnedSoulsEarned", MethodType.Setter)] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool patchDamnedSoulsEarned(ref int value)
        {
            value = Convert.ToInt32(value * DamnedSoulsPercentage.Value);
            Console.WriteLine($"patchDamnedSoulsEarned: {value}");
            return true;
        }
    }

}
