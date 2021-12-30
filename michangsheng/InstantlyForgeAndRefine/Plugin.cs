using BepInEx;
using KBEngine;
using HarmonyLib;
using System;

namespace InstantlyForgeAndRefine
{
    [BepInPlugin("cn.shabywu.michangsheng.InstantlyForgeAndRefine", "瞬间炼器和炼丹", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(InstantlyRefine));
            Harmony.CreateAndPatchAll(typeof(InstanlyForge));
        }

    }

    public class InstanlyForge {
        [HarmonyPatch(typeof(LianQiResultManager), "getCostTime")]
        [HarmonyPrefix]
        static bool patchGetCostTime(ref int __result){
            Console.WriteLine("calling patched LianQiResultManager::getCostTime");
            __result = 0;
            return false;
        }
    }


    [HarmonyPatch(typeof(LianDanResultManager), "lianDanJieSuan")]
    public class InstantlyRefine {
        static readonly object _object = new object(); 

        static Harmony singleton;

        [HarmonyPrefix]
        static void Prefix()
        {
            Console.WriteLine("before lianDanJieSuan");
            if (singleton == null) {
                lock (_object) {
                    Console.WriteLine("patch Avatar::AddTime");
                    singleton = Harmony.CreateAndPatchAll(typeof(AvatarPatcher));
                }
            }
        }

        [HarmonyPostfix]
        static void Postfix()
        {
            if (singleton != null) {
                lock (_object) {
                    if (singleton != null) {
                        singleton.UnpatchSelf();
                        singleton = null;
                        Console.WriteLine("unpatch Avatar::AddTime");
                    }
                } 
            }
            Console.WriteLine("after lianDanJieSuan");
        }
    }

    public class AvatarPatcher
    {
        [HarmonyPatch(typeof(Avatar), "AddTime")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool patchAvatarAddTime(){
            Console.WriteLine("Avatar::AddTime, skip add any time.");
            return false;
        }
    }
}
