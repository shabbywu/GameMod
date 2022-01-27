using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine.Events;
using JiaoYi;
using UnityEngine;


namespace BetterShoppingExperience
{
    [BepInPlugin("cn.shabywu.michangsheng.BetterShoppingExperience", "更好的交易体验", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            LogDebug = Logger.LogDebug;
        }

        static UnityAction closeAction;
        public delegate void Log(object data);
        public static Log LogDebug;

        private void Update()
        {
            if (JiaoYiUIMag.Inst == null)
            {
                return;
            }
            JiaoYiUIMag __instance = JiaoYiUIMag.Inst;

            int currentId = __instance.NpcId;

            // 尝试切换交易对象
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
            {
                if (UINPCJiaoHu.Inst != null)
                {
                    List<int> o = new List<int>();

                    foreach (int id in UINPCJiaoHu.Inst.TNPCIDList)
                    {
                        o.Add(id);
                    }

                    foreach (int id in UINPCJiaoHu.Inst.NPCIDList)
                    {
                        o.Add(id);
                    }

                    foreach (int id in UINPCJiaoHu.Inst.SeaNPCIDList)
                    {
                        o.Add(id);
                    }

                    foreach (int id in UINPCJiaoHu.Inst.TNPCIDList)
                    {
                        o.Add(id);
                    }

                    foreach (int id in UINPCJiaoHu.Inst.NPCIDList)
                    {
                        o.Add(id);
                    }

                    foreach (int id in UINPCJiaoHu.Inst.SeaNPCIDList)
                    {
                        o.Add(id);
                    }

                    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                    {
                        o.Reverse();
                    }

                    int next = findNext(o, currentId); ;
                    LogDebug("Current" + currentId + ", next id = " + next);

                    // stop closeAction
                    closeAction = __instance.CloseAction;

                    __instance.CloseAction = delegate () { };
                    __instance.Close();

                    ResManager.inst.LoadPrefab("JiaoYiUI").Inst(NewUICanvas.Inst.transform);
                    JiaoYiUIMag.Inst.Init(next, closeAction);
                }
            }
        }

        static int findNext(List<int> list, int target)
        {
            int index = -1;
            try
            {
                index = list.FindIndex(delegate (int i)
                {
                    return i == target;
                });
            }
            catch (Exception)
            {
                return -1;
            }

            if (index == -1)
            {
                // not found.
                return -1;
            }
            else if (index == list.Count - 1)
            {
                // found, but overflow.
                return -2;
            }
            else
            {
                return list[index + 1];
            }
        }

        static int findNext(List<List<int>> llist, int target)
        {
            bool overflow = false;
            foreach (List<int> list in llist)
            {
                var next = findNext(list, target);
                if (next == -1)
                {
                    if (overflow && list.Count > 0)
                    {
                        return list[0];
                    }
                    continue;
                }
                else if (next == -2)
                {
                    overflow = true;
                    continue;
                }
                else
                {
                    return next;
                }
            }
            return -2;
        }
    }
}
