using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Awaken.TG.Main.Heroes;
using Awaken.TG.Main.Heroes.Stats;
using Awaken.TG.Main.Fights.Rewards;
using Awaken.TG.Main.Realms;

namespace MultipleGain
{
    [BepInPlugin("cn.shabbywu.tainted_grai.multiple_gain", "多倍收益", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<float> WealthMultiplier;
        static ConfigEntry<float> WyrdShardsMultiplier;
        static ConfigEntry<float> ExpMultiplier;
        static ConfigEntry<float> ResourcesMultiplier;
        static ConfigEntry<float> BloodMultiplier;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            WealthMultiplier = Config.Bind("TaintedGrail.MultipleGain",  "WealthMultiplier", 1.2f, "金钱倍率");
            WyrdShardsMultiplier = Config.Bind("TaintedGrail.MultipleGain",  "WyrdShardsMultiplier", 2f, "石头倍率");
            ExpMultiplier = Config.Bind("TaintedGrail.MultipleGain",  "ExpMultiplier", 1.2f, "经验倍率");
            ResourcesMultiplier = Config.Bind("TaintedGrail.MultipleGain",  "ResourcesMultiplier", 1f, "资源倍率");
            BloodMultiplier = Config.Bind("TaintedGrail.MultipleGain",  "BloodMultiplier", 5f, "血液倍率");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(HeroGainStats), "WealthMultiplier", MethodType.Setter)] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool patchWealthMultiplier(ref Stat value)
        {
            float BaseValue = value.BaseValue;
            float changedValue = BaseValue * WealthMultiplier.Value;
            Console.WriteLine($"patchWealthMultiplier: {BaseValue} -> {changedValue}");
            value = new Stat(value.Owner, value.Type, changedValue);
            return true;
        }

        [HarmonyPatch(typeof(HeroGainStats), "WyrdShardsMultiplier", MethodType.Setter)] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool patchWyrdShardsMultiplier(ref Stat value)
        {
            float BaseValue = value.BaseValue;
            float changedValue = BaseValue * WyrdShardsMultiplier.Value;
            Console.WriteLine($"patchWyrdShardsMultiplier: {BaseValue} -> {changedValue}");
            value = new Stat(value.Owner, value.Type, changedValue);
            return true;
        }

        [HarmonyPatch(typeof(HeroGainStats), "ExpMultiplier", MethodType.Setter)] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool patchExpMultiplier(ref Stat value)
        {
            float BaseValue = value.BaseValue;
            float changedValue = BaseValue * ExpMultiplier.Value;
            Console.WriteLine($"patchExpMultiplier: {BaseValue} -> {changedValue}");
            value = new Stat(value.Owner, value.Type, changedValue);
            return true;
        }

        [HarmonyPatch(typeof(HeroGainStats), "ResourcesMultiplier", MethodType.Setter)] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool patchResourcesMultiplier(ref Stat value)
        {
            float BaseValue = value.BaseValue;
            float changedValue = BaseValue * ResourcesMultiplier.Value;
            Console.WriteLine($"patchResourcesMultiplier: {BaseValue} -> {changedValue}");
            value = new Stat(value.Owner, value.Type, changedValue);
            return true;
        }

        [HarmonyPatch(typeof(FightRewards), "Add")]
        [HarmonyPatch(new Type[] { typeof(StatType), typeof(int), typeof(bool)})]
        [HarmonyPrefix]
        static bool patchAdd(ref StatType statType, ref int quantity, ref bool grantable)
        {
            if (statType == Realm.Stats.Blood) {
                quantity = (int)(quantity * BloodMultiplier.Value);
            }
            return true;
        }
    }

}
