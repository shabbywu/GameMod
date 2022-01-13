using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using System.Collections.Generic;
using System;


namespace ForgetWuDaoSkill
{
    [BepInPlugin("cn.shabywu.michangsheng.ForgetWuDaoSkill", "遗忘悟道技能", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Harmony.CreateAndPatchAll(typeof(PatchWuDaoTooltipShow));
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(Tab.WuDaoTooltip), "Show")]
        public class PatchWuDaoTooltipShow {
            
            static IDictionary<int, bool> flags = new Dictionary<int, bool>();

            static void Prefix(ref int wudaoId)
            {
                Console.WriteLine("before WuDaoTooltip::Show");
                if (wudaoId > 0) {
                    flags[wudaoId] = false;
                    return;
                }
                
                // 负数表示需要遗忘
                wudaoId = -wudaoId;
                // 标记需要遗忘悟道技能
                flags[wudaoId] = true;
            }

            static void Postfix(ref int wudaoId, ref Text ____cost, ref FpBtn ____btn)
            {
                Console.WriteLine("after WuDaoTooltip::Show");
                if (flags[wudaoId]) {
                    ____cost.text = ____cost.text.Replace("【需求点数】", "【返还点数】");
                    ____btn.GetComponentInChildren<Text>().text = "遗忘";
                } else {
                    // 恢复文案
                    ____btn.GetComponentInChildren<Text>().text = "感悟";
                }
            }
        }

        [HarmonyPatch(typeof(Tab.WuDaoSlot), "SetState")]
        [HarmonyPrefix]
        static bool patchWuDaoSlotSetState(ref Tab.WuDaoSlot __instance, ref int state, ref GameObject ____go, ref Image ____icon){
            Tab.WuDaoSlot instance = __instance;
            Tab.WuDaoTooltip WuDaoTooltip = SingletonMono<Tab.TabUIMag>.Instance.WuDaoPanel.WuDaoTooltip;
            Image icon = ____icon;
            ____go.GetComponent<Tab.TabListener>().mouseUpEvent.RemoveAllListeners();
            switch (state) {
                case 1:
                    // 已学习
                    ____go.GetComponent<Tab.TabListener>().mouseUpEvent.AddListener(delegate()
                    {
                        WuDaoTooltip.Show(icon.sprite, -instance.Id, delegate(){
                            Console.WriteLine("calling Forget");
                            Forget(instance);
                        });

                    });
                    break;
                case 2:
                    // 可学习
                    ____go.GetComponent<Tab.TabListener>().mouseUpEvent.AddListener(delegate()
                    {
                        WuDaoTooltip.Show(icon.sprite, instance.Id, delegate(){
                            Console.WriteLine("calling Study");
                            Study(instance);
                        });
                    });
                    break;
                case 3:
                    // 未达到领悟条件
                    ____go.GetComponent<Tab.TabListener>().mouseUpEvent.AddListener(delegate() {
                        UIPopTip.Inst.Pop("未达到领悟条件", PopTipIconType.叹号);
                        WuDaoTooltip.Close();
                    });
                    break;
            }
            return true;
        }

        // 学习悟道技能
        static void Study(Tab.WuDaoSlot instance) {
			KBEngine.Avatar player = Tools.instance.getPlayer();
			if (instance.State == 1)
			{
				UIPopTip.Inst.Pop("已领悟过该大道", PopTipIconType.叹号);
			}
			else if (instance.State == 2)
			{
				if (player.wuDaoMag.GetNowWuDaoDian() >= instance.Cost)
				{
					if (instance.CanStudyWuDao())
					{
						foreach (int wuDaoType in JSONClass.WuDaoJson.DataDict[instance.Id].Type)
						{
							player.wuDaoMag.addWuDaoSkill(wuDaoType, instance.Id);
							SingletonMono<Tab.TabUIMag>.Instance.WuDaoPanel.UpdateWuDaoDian();
						}
						instance.SetState(1);
					}
					else if (instance.MoreCheck())
					{
						UIPopTip.Inst.Pop("未达到领悟条件", PopTipIconType.叹号);
					}
					else
					{
						UIPopTip.Inst.Pop("未领悟前置悟道", PopTipIconType.叹号);
					}
				}
				else
				{
					UIPopTip.Inst.Pop("悟道点不足", PopTipIconType.叹号);
				}
			}
			else if (instance.State == 3)
			{
				UIPopTip.Inst.Pop("未达到领悟条件", PopTipIconType.叹号);
			}
			SingletonMono<Tab.TabUIMag>.Instance.WuDaoPanel.WuDaoTooltip.Close();
        }

        static void Forget(Tab.WuDaoSlot instance) {
			KBEngine.Avatar player = Tools.instance.getPlayer();
			if (instance.State != 1)
			{
				UIPopTip.Inst.Pop("未领悟过该大道", PopTipIconType.叹号);
                SingletonMono<Tab.TabUIMag>.Instance.WuDaoPanel.WuDaoTooltip.Close();
                return;
			}

            foreach (int wuDaoType in JSONClass.WuDaoJson.DataDict[instance.Id].Type)
            {
                JSONObject wuDaoStudy = player.wuDaoMag.getWuDaoStudy(wuDaoType);
                if (wuDaoStudy.HasItem(instance.Id)) {
                    // 查找到需要遗忘的悟道技能, 并移除
                    var itemToRemove = wuDaoStudy.list.Find(r => r.n == instance.Id);
                    wuDaoStudy.list.Remove(itemToRemove);

                    GUIPackage.WuDaoStaticSkill.resetWuDaoSeid(player);

                    SingletonMono<Tab.TabUIMag>.Instance.WuDaoPanel.UpdateWuDaoDian();
                }
            }
            instance.SetState(2);
            SingletonMono<Tab.TabUIMag>.Instance.WuDaoPanel.WuDaoTooltip.Close();
        }
    }
}
