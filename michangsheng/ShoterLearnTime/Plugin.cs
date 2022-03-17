using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace ShoterLearnTime
{
    [BepInPlugin("cn.shabywu.michangsheng.ShoterLearnTime", "缩短功法学习/突破时间", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        // 简介: 
        // 对于稳定版 0.9.1.130 后的版本, 领悟/突破功法的耗时的计算由 Tools.CalcLingWuOrTuPoTime 实现
        // 对于之前的版本, 领悟功法的耗时由 Tools.getStudiSkillTime 实现, 突破功法的耗时由 Tools.getStudiStaticSkillTime 实现
        static ConfigEntry<int> ShorterStudyMultipiler;
        static ConfigEntry<int> ShorterTuPoMultipiler;

        static ConfigEntry<float> CoefficientTau;
        static ConfigEntry<float> LogisticMaxPercentage;
        static ConfigEntry<bool> EnableLogisticFunction;

        public delegate void Log(object data);
        public static Log LogDebug;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            ShorterStudyMultipiler = Config.Bind("ShoterLearnTime",  "ShorterStudyMultipiler", 10, "缩短学习时间倍率");
            ShorterTuPoMultipiler = Config.Bind("ShoterLearnTime",  "ShorterTuPoMultipiler", 3, "缩短突破时间倍率");

            EnableLogisticFunction = Config.Bind("Logistic-Function", "EnableLogisticFunction", true, "使用 Logistic 方程增强【悟性】对领悟/突破的影响, 关闭则使用简单粗暴的倍率进行控制");
            CoefficientTau = Config.Bind("Logistic-Function", "CoefficientTau", 0.05f, "Logistic 方程的时间系数, 该值越大, 【悟性】对学习时间的影响越明显。不建议修改该值。");
            LogisticMaxPercentage = Config.Bind("Logistic-Function", "LogisticMaxPercentage", 80f, new ConfigDescription("使用 Logistic 方程时, 【悟性】对学习时间的影响的最大百分比。如果设置成 100, 那么悟性足够的话, 就无需学习时间(影响平衡)", new AcceptableValueRange<float>(0f, 100f)));
            Harmony.CreateAndPatchAll(typeof(Plugin));
            LogDebug = Logger.LogDebug;
        }

        // 根据悟性和悟道计算领悟和突破的耗时
        [HarmonyPatch(typeof(Tools), "CalcLingWuOrTuPoTime")]
        [HarmonyPrefix]
        static bool patchCalcLingWuOrTuPoTime(ref int __result, ref int baseTime, ref List<int> wuDao) {
            if (!EnableLogisticFunction.Value) {
                return true;
            }

            var player = PlayerEx.Player;
            float wuXinFactor = 101 - 100 / (1 + 99 * Mathf.Exp(-CoefficientTau.Value * player.wuXin));
            wuXinFactor = Mathf.Clamp(wuXinFactor, 100 - LogisticMaxPercentage.Value, 100) / 100;
            float wuDaoFactor = 0f;
            if (wuDao.Count > 0)
            {
                int num3 = wuDao.Count / 2;
                for (int i = 0; i < wuDao.Count; i += 2)
                {
                    int wuDaoType = wuDao[i];
                    int wuDaoLevelByType = player.wuDaoMag.getWuDaoLevelByType(wuDaoType);
                    float num4 = jsonData.instance.WuDaoJinJieJson[wuDaoLevelByType.ToString()]["JiaCheng"].n;
                    num4 /= (float)num3;
                    wuDaoFactor += num4;
                }
            }
            wuDaoFactor = 1f - wuDaoFactor;
            wuDaoFactor = Mathf.Clamp(wuDaoFactor, 0f, 1f);
            __result = (int)((float)baseTime * wuXinFactor * wuDaoFactor);
            LogDebug($"基础时间: {baseTime} 天, 悟性修正系数: {wuXinFactor}, 悟道修正系数: {wuDaoFactor}. 修正结果: {__result} 天");
            return false;
        }

        // 降低功法突破时间
        [HarmonyPatch(typeof(Tools), "CalcTuPoTime")] // Specify target method with HarmonyPatch attribute
        [HarmonyPostfix]                              // There are different patch types. Prefix code runs before original code
        static void patchCalcTuPoTime(ref int __result){
            if (EnableLogisticFunction.Value) {
                return;
            }
            __result /= ShorterTuPoMultipiler.Value;
            LogDebug("calling patched Tools::CalcTuPoTime result: " + __result);
        }

        // 降低功法学习时间
        [HarmonyPatch(typeof(Tools), "CalcLingWuTime")] // Specify target method with HarmonyPatch attribute
        [HarmonyPostfix]                              // There are different patch types. Prefix code runs before original code
        static void patchCalcLingWuTime(ref int __result){
            if (EnableLogisticFunction.Value) {
                return;
            }
            __result /= ShorterStudyMultipiler.Value;
            LogDebug("calling patched Tools::CalcLingWuTime result: " + __result);
        }
    }
}
