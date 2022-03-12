using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ShoterLearnTime
{
    [BepInPlugin("cn.shabywu.michangsheng.ShoterLearnTime", "缩短功法学习/突破时间", "0.2.0")]
    public class Plugin : BaseUnityPlugin
    {

        static ConfigEntry<int> ShorterBaseWuXin;

        static ConfigEntry<int> ShorterNeedWuxin;

        public delegate void Log(object data);
        public static Log LogDebug;
        public static KBEngine.Avatar player;
        public static int ratio;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            ShorterBaseWuXin = Config.Bind("ShoterLearnTime",  "ShorterBaseWuXin", 10, "基础悟性");
            ShorterNeedWuxin = Config.Bind("ShoterLearnTime",  "ShorterNeedWuxin", 10, "缩短一倍时间所需悟性");
            Harmony.CreateAndPatchAll(typeof(Plugin));

            LogDebug = Logger.LogDebug;
        }

        // 降低功法突破时间
        [HarmonyPatch(typeof(Tools), "getStudiStaticSkillTime")] // Specify target method with HarmonyPatch attribute
        [HarmonyPostfix]                              // There are different patch types. Prefix code runs before original code
        static void patchGetStudiStaticSkillTime(ref int __result){
            player = Tools.instance.getPlayer();    // 获取玩家 
            if (player.ToString() == ""){
                return; // 未进入游戏
            }
            if (player.wuXin < ShorterBaseWuXin.Value){
                return; // 小于基础悟性
            }
            // 根据悟性缩短突破时间
            ratio = 1 + ( (int)player.wuXin - ShorterBaseWuXin.Value ) / ShorterNeedWuxin.Value;
            __result /= ratio;
            // __result /= ShorterTuPoMultipiler.Value;
            LogDebug("calling patched Tools::getStudiStaticSkillTime result: " + __result);
        }

        // 降低功法学习时间
        [HarmonyPatch(typeof(Tools), "getStudiSkillTime")] // Specify target method with HarmonyPatch attribute
        [HarmonyPostfix]                              // There are different patch types. Prefix code runs before original code
        static void patchGetStudiSkillTime(ref int __result){
            player = Tools.instance.getPlayer();    // 获取玩家 
            if (player.ToString() == ""){
                return; // 未进入游戏
            }
            if (player.wuXin < ShorterBaseWuXin.Value){
                return; // 小于基础悟性
            }
            // 根据悟性缩短学习时间
            ratio = 1 + ( (int)player.wuXin - ShorterBaseWuXin.Value ) / ShorterNeedWuxin.Value;
            __result /= ratio;
            // __result /= ShorterStudyMultipiler.Value;
            LogDebug("calling patched Tools::getStudiSkillTime result: " + __result);
        }
    }
}
