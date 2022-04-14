using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Fight;
using JSONClass;

namespace BattleGains
{
    [BepInPlugin("cn.shabywu.michangsheng.BattleGains", "战斗收益调整", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {

        public delegate void Log(object data);
        public static Log LogDebug;

        static ConfigEntry<int> EquipmentDropMultiplier;
        static ConfigEntry<int> ItemDropMultiplier;

        static ConfigEntry<float> MoneyDropMultipiler;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            EquipmentDropMultiplier = Config.Bind("BattleGains",  "EquipmentDropMultiplier", 1, "装备掉落倍率");
            ItemDropMultiplier = Config.Bind("BattleGains",  "ItemDropMultiplier", 2, "物品掉落倍率");

            MoneyDropMultipiler = Config.Bind("BattleGains",  "MoneyDropMultipiler", 2f, "金钱掉落倍率");
            Harmony.CreateAndPatchAll(typeof(Plugin));

            LogDebug = Logger.LogDebug;
        }

        // 掉落金钱
        [HarmonyPatch(typeof(FightVictory), "addMoney")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool patchAddMoney(ref NpcDrop __instance, ref float percent){
            LogDebug("calling patched FightVictory::addMoney");
            percent = MoneyDropMultipiler.Value;
            return true;
        }


        // 掉落战利品主入口, 设置掉落率 100%
        [HarmonyPatch(typeof(NpcDrop), "dropReward")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool dropReward(ref NpcDrop __instance, ref float equipLv, ref float packLv, int NPCID){
            LogDebug("calling patched NpcDrop:dropReward");
            equipLv = 1;
            packLv = 1;
            return true;
        }

        // 掉落武器
        [HarmonyPatch(typeof(NpcDrop), "dropEquip")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool dropEquip(ref NpcDrop __instance, ref JSONObject ___npcDate, ref JSONObject addItemList, int NPCID)
        {
            LogDebug("calling patched NpcDrop:dropEquip  " + NPCID);
            if (NPCID >= 20000) {
                JSONObject jsonobject = ___npcDate[NPCID.ToString()]["equipList"];
                foreach(string key in jsonobject.keys) {
                    if (jsonobject[key].HasField("NomalID")) {
                        buildTempItem(ref addItemList, jsonobject[key]["NomalID"].I, 1 * EquipmentDropMultiplier.Value, null);
                    } else {
                        buildTempItem(ref addItemList, jsonobject[key]["ItemID"].I, 1 * EquipmentDropMultiplier.Value, jsonobject[key]);
                    }
                }
            } else {
                List<int> list = new List<int>();
                if (___npcDate[NPCID.ToString()]["equipWeapon"].I > 0)
                {
                    list.Add(___npcDate[NPCID.ToString()]["equipWeapon"].I);
                }
                if (___npcDate[NPCID.ToString()]["equipRing"].I > 0)
                {
                    list.Add(___npcDate[NPCID.ToString()]["equipRing"].I);
                }
                if (___npcDate[NPCID.ToString()]["equipClothing"].I > 0)
                {
                    list.Add(___npcDate[NPCID.ToString()]["equipClothing"].I);
                }
                foreach(int itemId in list) {
                    buildTempItem(ref addItemList, itemId, 1 * EquipmentDropMultiplier.Value, null);
                }
            }
            return false; // Returning false in prefix patches skips running the original code
        }

        static void buildTempItem(ref JSONObject addItemList, int ItemID, int ItemNum, JSONObject seid = null) {
            JSONObject jsonobject = new JSONObject();
            jsonobject.AddField("UUID", Tools.getUUID());
            jsonobject.AddField("ID", ItemID);
            jsonobject.AddField("Num", ItemNum);
            if (seid != null) {
                jsonobject.AddField("seid", seid);
            } else {
                jsonobject.AddField("seid", Tools.CreateItemSeid(ItemID));
            }
            addItemList.Add(jsonobject);
        }

        [HarmonyPatch(typeof(NpcDrop), "buidTempItem")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]
        static bool multipleDroppedItem(ref int ItemID, ref int ItemNum){
            var multiple = ItemDropMultiplier.Value;
            try
            {
			    _ItemJsonData itemJsonData = _ItemJsonData.DataDict[ItemID];
                var type = (ItemTypes)itemJsonData.type;

                switch (type) {
                    case ItemTypes.武器:
                    case ItemTypes.衣服:
                    case ItemTypes.饰品:
                        multiple = EquipmentDropMultiplier.Value;
                        break;
                    case ItemTypes.技能书:
                    case ItemTypes.功法:
                    case ItemTypes.任务:
                    case ItemTypes.丹方:
                    case ItemTypes.书籍:
                    case ItemTypes.秘籍:
                        multiple = 1;
                        break;
                }
            }
            catch (System.Exception)
            {
                
                throw;
            }
            
            ItemNum *= multiple;
            return true;
        }
    }
}
