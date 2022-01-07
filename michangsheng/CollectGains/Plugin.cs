using BepInEx;
using KBEngine;
using CaiJi;
using HarmonyLib;
using System;

namespace CollectGains
{
    [BepInPlugin("cn.shabywu.michangsheng.CollectGains", "采集耗时调整", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Harmony.CreateAndPatchAll(typeof(CollectGains));
        }
    }

    [HarmonyPatch(typeof(CaiJiUIMag), "StartCaiJi")]
    public class CollectGains {
        static readonly object _object = new object(); 

        static Harmony singleton;

        static void Prefix()
        {
            Console.WriteLine("before StartCaiJi");
            if (singleton == null) {
                lock (_object) {
                    Console.WriteLine("patch PatchContext");
                    singleton = Harmony.CreateAndPatchAll(typeof(PatchContext));
                }
            }
        }

        static void Postfix()
        {
            if (singleton != null) {
                lock (_object) {
                    if (singleton != null) {
                        singleton.UnpatchSelf();
                        singleton = null;
                        Console.WriteLine("unpatch PatchContext");
                    }
                } 
            }
            Console.WriteLine("after StartCaiJi");
        }
    }

    public class PatchContext
    {
        [HarmonyPatch(typeof(Avatar), "AddTime")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool patchAvatarAddTime(ref int addday, ref int addMonth, ref int Addyear){
            // 减少采集的耗时, 将月修改成日, 年修改成月
            addday = addMonth;
            addMonth = Addyear;
            Addyear = 0;
            return true;
        }
    }
}
