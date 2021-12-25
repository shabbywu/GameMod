using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace WuDaoGains
{
    [BepInPlugin("cn.shabywu.michangsheng.wu_dao_gains", "悟道收益调整", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<int> WuDaoZhiMultipiler;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            WuDaoZhiMultipiler = Config.Bind("WuDaoGains",  "WuDaoZhiMultipiler", 10, "悟道值提升倍率");

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }


        // 提升悟道值获取
        [HarmonyPatch(typeof(LunDaoSuccess), "GetWuDaoZhi")] // Specify target method with HarmonyPatch attribute
        [HarmonyPostfix]                              // There are different patch types. Prefix code runs before original code
        static void patchGetWuDaoZhi(ref int __result){
            __result *= WuDaoZhiMultipiler.Value;
            Console.WriteLine("calling patched LunDaoSuccess::GetWuDaoZhi， result: " + __result);
        }
    }
}
