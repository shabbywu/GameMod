using BepInEx;
using HarmonyLib;
script.NewLianDan.LianDan;

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

        [HarmonyPatch(typeof(LianDanPanel), "StartLianDan")]
        public class InstantlyRefine {
            static readonly object _object = new object(); 

            static Harmony singleton;

            [HarmonyPrefix]
            static void Prefix()
            {
                LogDebug("before LianDanPanel::StartLianDan");
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
                LogDebug("after LianDanPanel::StartLianDan");
            }
        }

        public class AvatarPatcher
        {
            [HarmonyPatch(typeof(KBEngine.Avatar), "AddTime")]
            [HarmonyPrefix]
            static bool patchAvatarAddTime(){
                LogDebug("Avatar::AddTime, skip add any time.");
                return false;
            }
        }

    }
}
