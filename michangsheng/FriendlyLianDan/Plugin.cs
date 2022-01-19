using BepInEx;
using UnityEngine;
using HarmonyLib;
using Tab;
using System.Collections.Generic;

namespace FriendlyLianDan
{
    [BepInPlugin("cn.shabywu.michangsheng.FriendlyLianDan", "更友好的炼丹", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            itemDatabase = (ItemDataBaseList)Resources.Load("ItemDatabase");
            _hi = Harmony.CreateAndPatchAll(typeof(PatchDanFangParentCellInit));
            LogDebug = Logger.LogDebug;

            foreach (var item in ItemSystem.Items.Instance.items) {
                LogDebug(item.ToString());
            }
        }

        private static Harmony _hi;

        private static ItemDataBaseList itemDatabase;

        public delegate void Log(object data);
        public static Log LogDebug;

        [HarmonyPatch(typeof(DanFangParentCell), "init")]
        public class PatchDanFangParentCellInit {
            static void Postfix(ref DanFangParentCell __instance)
            {
                DanFangParentCell instance = __instance;
                TabListener listener = __instance.gameObject.GetComponent<TabListener>();
                if (listener == null) {
                    listener = __instance.gameObject.AddComponent<TabListener>();
                }
                listener.mouseEnterEvent.AddListener(() => {
                    LogDebug($"OnHover 丹方/丹药ID: {instance.DanFangID}");
                    UToolTip.OpenItemTooltip(new GUIPackage.item(instance.DanFangID));
                });
                listener.mouseOutEvent.AddListener(() => {
                    UToolTip.Close();
                });
            }
        }

        // 推演所有丹方
        public void CalculateAllDanFang(int danyaoID) {
			if (!jsonData.instance.LianDanDanFangBiao.HasField(danyaoID.ToString()))
			{
				LogDebug($"丹方出错丹方表ID {danyaoID} 不存在");
			}

            // id, value
            Dictionary<int, int> danyao = new Dictionary<int, int>();
            // 主药
        }

        public class DanFangCalculator {
            int id;
            JSONObject 基础丹方;

            KeyValuePair<int, int> 药引药性;

            KeyValuePair<int, int> 主药药性1;
            KeyValuePair<int, int> 主药药性2;

            KeyValuePair<int, int> 副药药性1;

            KeyValuePair<int, int> 副药药性2;

            public DanFangCalculator(int danyaoID) {
                if (!jsonData.instance.LianDanDanFangBiao.HasField(danyaoID.ToString()))
                {
                    LogDebug($"丹方出错丹方表ID {danyaoID} 不存在");
                }

                id = danyaoID;
                基础丹方 = jsonData.instance.LianDanDanFangBiao[danyaoID.ToString()];


                药引药性 = GetCaoYaoYaoXing((int)基础丹方["value1"].n, (int)基础丹方["num1"].n, 1);
                主药药性1 = GetCaoYaoYaoXing((int)基础丹方["value2"].n, (int)基础丹方["num2"].n, 2);
                主药药性2 = GetCaoYaoYaoXing((int)基础丹方["value3"].n, (int)基础丹方["num3"].n, 2);
                副药药性1 = GetCaoYaoYaoXing((int)基础丹方["value4"].n, (int)基础丹方["num4"].n, 3);
                副药药性2 = GetCaoYaoYaoXing((int)基础丹方["value5"].n, (int)基础丹方["num5"].n, 3);

                if (主药药性1.Key == 主药药性2.Key) {
                    主药药性1 = new KeyValuePair<int, int>(主药药性1.Key, 主药药性1.Value + 主药药性2.Value);
                    主药药性2 = new KeyValuePair<int, int>(0, 0);
                } else if (主药药性1.Key < 主药药性2.Key) {
                    var tmp = 主药药性1;
                    主药药性1 = 主药药性2;
                    主药药性2 = tmp;
                }
                
                if (副药药性1.Key == 副药药性2.Key) {
                    副药药性1 = new KeyValuePair<int, int>(副药药性1.Key, 副药药性1.Value + 副药药性2.Value);
                    副药药性2 = new KeyValuePair<int, int>(0, 0);
                } else if (副药药性1.Key < 副药药性2.Key) {
                    var tmp = 副药药性1;
                    副药药性1 = 副药药性2;
                    副药药性2 = tmp;
                }
            }

            public KeyValuePair<int, int> GetCaoYaoYaoXing(int caoyaoID, int num, int YaoZhiKind) {
                // YaoZhiKind: 1: 药引, 2: 主药, 3: 副药
                string kind = $"yaoZhi{YaoZhiKind}";
                JSONObject caoyaoData = jsonData.instance.ItemJsonData[caoyaoID.ToString()];

                List<int> qualityToNum = new List<int>{0, 1, 3, 9, 36, 180, 1080};
                
                int yaozhi = (int)caoyaoData[kind].n;
                int quality = (int)caoyaoData["quality"].n;
                num *= qualityToNum[quality];
                return new KeyValuePair<int, int>(yaozhi, num);
            }

            public void CalculateAllYaoFang(int max) {
                // 穷举法
                // 计算所有草药的药性 * 个数(10个以内) 的药性映射列表 (药质id, 药力) => [(草药id, 个数), ...]
                // 计算丹药模板的药方
                // 进行穷举
            }

            public bool TestYaoFang(KeyValuePair<int, int> 药引, KeyValuePair<int, int> 主药1, KeyValuePair<int, int> 主药2, KeyValuePair<int, int> 副药1, KeyValuePair<int, int> 副药2) {
                KeyValuePair<int, int> 主药药性1 = GetCaoYaoYaoXing(主药1.Key, 主药1.Value, 2);
                KeyValuePair<int, int> 主药药性2 = GetCaoYaoYaoXing(主药2.Key, 主药2.Value, 2);
                KeyValuePair<int, int> 副药药性1 = GetCaoYaoYaoXing(副药1.Key, 副药1.Value, 3);
                KeyValuePair<int, int> 副药药性2 = GetCaoYaoYaoXing(副药2.Key, 副药2.Value, 3);

                // 药质只计算 1 份
                List<KeyValuePair<int, int>> 药质 = new List<KeyValuePair<int, int>>{
                    GetCaoYaoYaoXing(药引.Key, 药引.Value > 0 ? 1 : 0, 1),
                    GetCaoYaoYaoXing(主药1.Key, 主药1.Value > 0 ? 1 : 0, 1),
                    GetCaoYaoYaoXing(主药2.Key, 主药2.Value > 0 ? 1 : 0, 1),
                    GetCaoYaoYaoXing(副药1.Key, 副药1.Value > 0 ? 1 : 0, 1),
                    GetCaoYaoYaoXing(副药2.Key, 副药2.Value > 0 ? 1 : 0, 1),
                };

                if (主药药性1.Key == 主药药性2.Key) {
                    主药药性1 = new KeyValuePair<int, int>(主药药性1.Key, 主药药性1.Value + 主药药性2.Value);
                    主药药性2 = new KeyValuePair<int, int>(0, 0);
                } else if (主药药性1.Key < 主药药性2.Key) {
                    var tmp = 主药药性1;
                    主药药性1 = 主药药性2;
                    主药药性2 = tmp;
                }
                
                if (副药药性1.Key == 副药药性2.Key) {
                    副药药性1 = new KeyValuePair<int, int>(副药药性1.Key, 副药药性1.Value + 副药药性2.Value);
                    副药药性2 = new KeyValuePair<int, int>(0, 0);
                } else if (副药药性1.Key < 副药药性2.Key) {
                    var tmp = 副药药性1;
                    副药药性1 = 副药药性2;
                    副药药性2 = tmp;
                }

                // 计算药性是否符合
                if (!this.主药药性1.Equals(主药药性1) || !this.主药药性2.Equals(主药药性2) || !this.副药药性1.Equals(副药药性1) || !this.副药药性2.Equals(副药药性2)) {
                    return false;
                }

                // 计算药质是否符合
                int flag = 0;
                foreach  (var item in 药质) {
                    switch (item.Value) {
                        case 1:
                            flag -=1;
                            break;
                        case 2:
                            flag +=1;
                            break;
                    }
                }

                if (flag != 0) {
                    return false;
                }

                // 计算药引药性是否符合
                return GetCaoYaoYaoXing(药引.Key, 药引.Value, 1).Equals(this.药引药性);
            }
        }
    }
}
