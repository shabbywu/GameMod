using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;

using GUIPackage;
using UnityEngine;


namespace BetterShoppingExperience
{
    [BepInPlugin("cn.shabywu.michangsheng.BetterShoppingExperience", "更好的交易体验", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        // 修改监听事件
        [HarmonyPatch(typeof(ExchangePlan), "Update")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]                              // There are different patch types. Prefix code runs before original code
        static bool patchExchangePlanUpdate(ref ExchangePlan __instance){
            int currentId = __instance.MonstarID;
            // 切换交易对象
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) {
                if (UINPCJiaoHu.Inst != null) {
                    List<int> o = new List<int>();

                    foreach(int id in UINPCJiaoHu.Inst.TNPCIDList){
                        o.Add(id);
                    }

                    foreach(int id in UINPCJiaoHu.Inst.NPCIDList){
                        o.Add(id);
                    }

                    foreach(int id in UINPCJiaoHu.Inst.SeaNPCIDList){
                        o.Add(id);
                    }

                    foreach(int id in UINPCJiaoHu.Inst.TNPCIDList){
                        o.Add(id);
                    }

                    foreach(int id in UINPCJiaoHu.Inst.NPCIDList){
                        o.Add(id);
                    }

                    foreach(int id in UINPCJiaoHu.Inst.SeaNPCIDList){
                        o.Add(id);
                    }

                    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ) {
                        o.Reverse();
                    }

                    int next = findNext(o, currentId);;
                    Console.WriteLine("Current" + currentId + ", next id = " + next);

                    __instance.MonstarID = next;
                    __instance.initPlan();
                    __instance.updateMoney();
                }
                return false;
            }
            // 上一页
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                __instance.inventoryMonstar.selectpage.lastPage();
            }
    
            // 上一页
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                __instance.inventoryMonstar.selectpage.nextPage();
            }

            return true;
        }

        static int findNext(List<int> list, int target) {
            int index = -1;
            try {
                index = list.FindIndex(delegate(int i){
                    return i == target;
                });
            } catch (ArgumentNullException) {
                return -1;
            }

            if (index == -1) {
                // not found.
                return -1;
            } else if (index == list.Count - 1) {
                // found, but overflow.
                return -2;
            } else {
                return list[index + 1];
            }
        }

        static int findNext(List<List<int>> llist, int target) {
            bool overflow = false;
            foreach (List<int> list in llist) {
                var next = findNext(list, target);
                if (next == -1) {
                    if (overflow && list.Count > 0) {
                        return list[0];
                    }
                    continue;
                } else if (next == -2) {
                    overflow = true;
                    continue;
                } else {
                    return next;
                }
            }
            return -2;
        }
    }
}
