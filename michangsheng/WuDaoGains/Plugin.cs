﻿using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using KBEngine;

namespace WuDaoGains
{
    [BepInPlugin("cn.shabywu.michangsheng.WuDaoGains", "悟道收益调整", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<int> WuDaoZhiMultipiler;
        static ConfigEntry<int> LingGuangStudyTimeShorterMultipiler;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            WuDaoZhiMultipiler = Config.Bind("WuDaoGains",  "WuDaoZhiMultipiler", 10, "悟道值提升倍率");
            LingGuangStudyTimeShorterMultipiler = Config.Bind("WuDaoGains",  "LingGuangStudyTimeShorterMultipiler", 20, "灵感学习时间降低倍率");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }


        // 降低悟道灵感的学习时间
        [HarmonyPatch(typeof(WuDaoMag), "CalcGanWuTime")] // Specify target method with HarmonyPatch attribute
        [HarmonyPostfix]                              // There are different patch types. Prefix code runs before original code
        static void patchCalcGanWuTime(ref int __result){
            Console.WriteLine("calling patched WuDaoMag::CalcGanWuTime");
            __result /= LingGuangStudyTimeShorterMultipiler.Value;
        }

        // 提升 悟道值
        [HarmonyPatch(typeof(LunDaoManager), "AddWuDaoZhi")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool patchAddWuDaoZhi(ref LunDaoManager __instance, ref int addNum){
            Console.WriteLine("calling patched LunDaoManager::AddWuDaoZhi");
            addNum *= WuDaoZhiMultipiler.Value;
            return true;
        }
    }
}
