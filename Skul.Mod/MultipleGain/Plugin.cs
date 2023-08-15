using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Data;

namespace MultipleGain
{
    [BepInPlugin("cn.shabywu.skul_mod.multiple_gain", "多倍收益", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<double> GoldPercentage;
        static ConfigEntry<double> DarkQuartzPercentage;
        static ConfigEntry<double> BonePercentage;
        static ConfigEntry<double> HeartQuartzPercentage;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            GoldPercentage = Config.Bind("Skul.MultipleGain",  "GoldPercentage", 1.5, "Gold(金币) 倍率");
            DarkQuartzPercentage = Config.Bind("Skul.MultipleGain",  "DarkQuartzPercentage", 1.5, "DarkQuartz(黑暗精华) 倍率");
            BonePercentage = Config.Bind("Skul.MultipleGain",  "BonePercentage", 1.5, "Bone(骨头) 倍率");
            HeartQuartzPercentage = Config.Bind("Skul.MultipleGain",  "HeartQuartzPercentage", 1.5, "HeartQuartz(生命精华) 倍率");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(GameData.Currency), "Earn",typeof(double))] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool patchCurrencyEarn(ref double amount)
        {
            GameData.Currency.gold.multiplier.AddOrUpdate(GoldPercentage, GoldPercentage.Value);
            GameData.Currency.darkQuartz.multiplier.AddOrUpdate(DarkQuartzPercentage, DarkQuartzPercentage.Value);
            GameData.Currency.bone.multiplier.AddOrUpdate(BonePercentage, BonePercentage.Value);
            GameData.Currency.heartQuartz.multiplier.AddOrUpdate(HeartQuartzPercentage, HeartQuartzPercentage.Value);
            return true;
        }
    }

}
