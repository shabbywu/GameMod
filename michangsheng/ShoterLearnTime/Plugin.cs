using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ShoterLearnTime
{
    [BepInPlugin("cn.shabywu.michangsheng.ShoterLearnTime", "缩短功法学习/突破时间", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {

        static ConfigEntry<int> ShorterStudyMultipiler;

        static ConfigEntry<int> ShorterTuPoMultipiler;

        public delegate void Log(object data);
        public static Log LogDebug;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            ShorterStudyMultipiler = Config.Bind("ShoterLearnTime",  "ShorterStudyMultipiler", 10, "缩短学习时间倍率");
            ShorterTuPoMultipiler = Config.Bind("ShoterLearnTime",  "ShorterTuPoMultipiler", 3, "缩短突破时间倍率");
            Harmony.CreateAndPatchAll(typeof(Plugin));

            LogDebug = Logger.LogDebug;
        }

        // 降低功法突破时间
        [HarmonyPatch(typeof(Tools), "getStudiStaticSkillTime")] // Specify target method with HarmonyPatch attribute
        [HarmonyPostfix]                              // There are different patch types. Prefix code runs before original code
        static void patchGetStudiStaticSkillTime(ref int __result){
            __result /= ShorterTuPoMultipiler.Value;
            LogDebug("calling patched Tools::getStudiStaticSkillTime result: " + __result);
        }

        // 降低功法学习时间
        [HarmonyPatch(typeof(Tools), "getStudiSkillTime")] // Specify target method with HarmonyPatch attribute
        [HarmonyPostfix]                              // There are different patch types. Prefix code runs before original code
        static void patchGetStudiSkillTime(ref int __result){
            __result /= ShorterStudyMultipiler.Value;
            LogDebug("calling patched Tools::getStudiSkillTime result: " + __result);
        }
    }
}
