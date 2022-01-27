using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;

namespace StrengthenDongfu
{
    [BepInPlugin("cn.shabywu.michangsheng.StrengthenDongfu", "洞府加强", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<int> ZYXiuLianMultipiler;
        static ConfigEntry<int> ZYLingTianMultipiler;
        static ConfigEntry<int> ZYLingTianCuiShengMultipiler;

        static ConfigEntry<int> LYXiuLianMultipiler;
        static ConfigEntry<int> LYLingTianMultipiler;

        public delegate void Log(object data);
        public static Log LogDebug;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            ZYXiuLianMultipiler = Config.Bind("ZhenYan", "ZYXiuLianMultipiler", 2, "【阵眼】修炼加成倍率");
            ZYLingTianMultipiler = Config.Bind("ZhenYan", "ZYLingTianMultipiler", 3, "【阵眼】灵田培养加成倍率");
            ZYLingTianCuiShengMultipiler = Config.Bind("ZhenYan", "ZYLingTianCuiShengMultipiler", 5, "【阵眼】灵田小绿瓶催生加成倍率");

            LYXiuLianMultipiler = Config.Bind("LingYan", "LYXiuLianMultipiler", 2, "【灵眼】修炼加成倍率");
            LYLingTianMultipiler = Config.Bind("LingYan", "LYLingTianMultipiler", 3, "【灵眼】灵田培养加成倍率");

            Harmony.CreateAndPatchAll(typeof(Plugin));

            LogDebug = Logger.LogDebug;
        }

        // 提高【阵眼】相关的数据数值
        [HarmonyPatch(typeof(JSONClass.DFZhenYanLevel), "InitDataDict")]
        [HarmonyPostfix]
        public static void  PatchDFZhenYanLevelInitDataDict(ref Dictionary<int, JSONClass.DFZhenYanLevel> ___DataDict) {
            foreach (var kv in ___DataDict) {
                kv.Value.xiuliansudu *= ZYXiuLianMultipiler.Value;
                kv.Value.lingtiansudu *= ZYLingTianMultipiler.Value;
                kv.Value.lingtiancuishengsudu *= ZYLingTianCuiShengMultipiler.Value;
            }
        }

        // 提高【灵眼】相关的数据数值
        [HarmonyPatch(typeof(JSONClass.DFLingYanLevel), "InitDataDict")]
        [HarmonyPostfix]
        public static void  PatchDFLingYanLevelInitDataDict(ref Dictionary<int, JSONClass.DFLingYanLevel> ___DataDict) {
            foreach (var kv in ___DataDict) {
                kv.Value.xiuliansudu *= LYXiuLianMultipiler.Value;
                kv.Value.lingtiansudu *= LYLingTianMultipiler.Value;
            }
        }
    }
}
