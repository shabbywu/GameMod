using BepInEx;
using HarmonyLib;

namespace InstantlyForgeAndRefine
{
    [BepInPlugin("cn.shabywu.michangsheng.InstantlyForgeAndRefine", "瞬间炼器和炼丹", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {

        public delegate void Log(object data);
        public static Log LogDebug;
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(InstantlyRefine));
            Harmony.CreateAndPatchAll(typeof(InstanlyForge));

            LogDebug = Logger.LogDebug;
        }

        public class InstanlyForge {
            [HarmonyPatch(typeof(LianQiResultManager), "getCostTime")]
            [HarmonyPrefix]
            static bool patchGetCostTime(ref int __result){
                LogDebug("calling patched LianQiResultManager::getCostTime");
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
                LogDebug("before lianDanJieSuan");
                if (singleton == null) {
                    lock (_object) {
                        LogDebug("patch Avatar::AddTime");
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
                            LogDebug("unpatch Avatar::AddTime");
                        }
                    } 
                }
                LogDebug("after lianDanJieSuan");
            }
        }

        public class AvatarPatcher
        {
            [HarmonyPatch(typeof(KBEngine.Avatar), "AddTime")] // Specify target method with HarmonyPatch attribute
            [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
            static bool patchAvatarAddTime(){
                LogDebug("Avatar::AddTime, skip add any time.");
                return false;
            }
        }

    }
}
