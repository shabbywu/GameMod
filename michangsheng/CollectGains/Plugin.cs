using BepInEx;
using HarmonyLib;

namespace CollectGains
{
    [BepInPlugin("cn.shabywu.michangsheng.CollectGains", "采集耗时调整", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        public delegate void Log(object data);
        public static Log LogDebug;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Harmony.CreateAndPatchAll(typeof(CollectGains));

            LogDebug = Logger.LogDebug;
        }

        [HarmonyPatch(typeof(CaiJi.CaiJiUIMag), "StartCaiJi")]
        public class CollectGains {
            static readonly object _object = new object(); 

            static Harmony singleton;

            static void Prefix()
            {
                LogDebug("before StartCaiJi");
                if (singleton == null) {
                    lock (_object) {
                        LogDebug("patch PatchContext");
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
                            LogDebug("unpatch PatchContext");
                        }
                    } 
                }
                LogDebug("after StartCaiJi");
            }
        }

        public class PatchContext
        {
            [HarmonyPatch(typeof(KBEngine.Avatar), "AddTime")] // Specify target method with HarmonyPatch attribute
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

}
