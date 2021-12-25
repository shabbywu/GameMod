using System;
using BepInEx;
using HarmonyLib;

namespace DropEveryThing
{
    [BepInPlugin("cn.shabywu.michangsheng.unlimited_reroll", "物品全掉落", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(NpcDrop), "dropEquip")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool dropEquip(ref NpcDrop __instance, ref JSONObject ___npcDate, ref JSONObject addItemList, int NPCID)
        {
            Console.WriteLine("patched dropEquip");
            if (NPCID >= 20000) {
                JSONObject jsonobject = ___npcDate[NPCID.ToString()]["equipList"];
                if (jsonobject.keys.Count == 0){
                    return false;
                }
                foreach(string key in jsonobject.keys) {
                    if (jsonobject[key].HasField("NomalID")) {
                        buildTempItem(ref addItemList, jsonobject[key]["NomalID"].I, 1, null);
                    } else {
                        buildTempItem(ref addItemList, jsonobject[key]["NomalID"].I, 1, jsonobject[key]);
                    }
                }
                return false;
            }
            return true; // Returning false in prefix patches skips running the original code
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
    }
}
