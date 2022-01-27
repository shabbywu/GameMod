using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;


namespace BetterTooltips
{
    public class CaoYaoHandler
    {
        // 在词条框中展示当前草药可能的丹方
        static public void ShowPossibleDanFang(ref ToolTipsMag __instance, ref ToolTipsMag.Direction ____direction, ref Dictionary<int, string> ____qualityNameColordit)
        {
            Bag.BaseItem item = getItem(ref __instance);
            if (item == null)
            {
                return;
            }

            var parent = (____direction == ToolTipsMag.Direction.右) ? __instance.RightPanel.transform : __instance.LeftPanel.transform;
            List<int> main;
            List<int> deputy;
            List<int> intro;

            CaoYaoHandler.BuildAllUsedDanFang(item.Id, out main, out deputy, out intro);

            if (main.Count > 0)
            {

                __instance.CiTiaoParent.gameObject.SetActive(true);
                var gameObject = __instance.CiTiao.Inst(parent);
                gameObject.SetActive(true);
                var oldText = gameObject.GetComponentInChildren<WXB.SymbolText>();

                var box = oldText.gameObject;
                var font = oldText.font;
                var fontSize = oldText.fontSize;

                UnityEngine.Object.DestroyImmediate(oldText);

                var oldfitter = box.GetComponent<ContentSizeFitter>();
                var text = box.AddComponent<Text>();
                text.font = font;
                text.fontSize = fontSize;

                text.SetText("【主药】\n", "#dd61ff");
                for (var i = 0; i < main.Count; i++)
                {
                    JSONClass._ItemJsonData itemJsonData = JSONClass._ItemJsonData.DataDict[main[i]];
                    var name = itemJsonData.name;
                    var color = ____qualityNameColordit[itemJsonData.quality];
                    text.AddText(name, color);
                    if (i < main.Count - 1)
                    {
                        text.AddText("、");
                    }
                }
            }

            if (deputy.Count > 0)
            {
                __instance.CiTiaoParent.gameObject.SetActive(true);
                var gameObject = __instance.CiTiao.Inst(parent);
                gameObject.SetActive(true);
                var oldText = gameObject.GetComponentInChildren<WXB.SymbolText>();
                
                var box = oldText.gameObject;
                var font = oldText.font;
                var fontSize = oldText.fontSize;

                UnityEngine.Object.DestroyImmediate(oldText);

                var oldfitter = box.GetComponent<ContentSizeFitter>();
                var text = box.AddComponent<Text>();
                text.font = font;
                text.fontSize = fontSize;

                text.SetText("【辅药】\n", "#dd61ff");
                for (var i = 0; i < deputy.Count; i++)
                {
                    JSONClass._ItemJsonData itemJsonData = JSONClass._ItemJsonData.DataDict[deputy[i]];
                    var name = itemJsonData.name;
                    var color = ____qualityNameColordit[itemJsonData.quality];
                    text.AddText(name, color);
                    if (i < deputy.Count - 1)
                    {
                        text.AddText("、");
                    }
                }
            }

            if (Plugin.ShowYaoYin.Value && intro.Count > 0)
            {
                __instance.CiTiaoParent.gameObject.SetActive(true);
                var gameObject = __instance.CiTiao.Inst(parent);
                gameObject.SetActive(true);
                var oldText = gameObject.GetComponentInChildren<WXB.SymbolText>();
                
                var box = oldText.gameObject;
                var font = oldText.font;
                var fontSize = oldText.fontSize;

                UnityEngine.Object.DestroyImmediate(oldText);

                var oldfitter = box.GetComponent<ContentSizeFitter>();
                var text = box.AddComponent<Text>();
                text.font = font;
                text.fontSize = fontSize;

                text.SetText("【药引】\n", "#dd61ff");
                for (var i = 0; i < intro.Count; i++)
                {
                    JSONClass._ItemJsonData itemJsonData = JSONClass._ItemJsonData.DataDict[intro[i]];
                    var name = itemJsonData.name;
                    var color = ____qualityNameColordit[itemJsonData.quality];
                    text.AddText(name, color);
                    if (i < intro.Count - 1)
                    {
                        text.AddText("、");
                    }
                }
            }
        }

        // 在词条框中展示当前草药所有产地
        static public void ShowPossibleChanDi(ref ToolTipsMag __instance, ref ToolTipsMag.Direction ____direction)
        {
            Bag.BaseItem item = getItem(ref __instance);
            if (item == null)
            {
                return;
            }
            var parent = (____direction == ToolTipsMag.Direction.右) ? __instance.RightPanel.transform : __instance.LeftPanel.transform;

            List<string> chandiNameList = new List<string>();
            foreach (JSONObject jsonobject in jsonData.instance.CaiYaoDiaoLuo.list)
            {
                for (int i = 1; i <= 8; i++)
                {
                    var fubenIndex = jsonobject["FuBen"].Str;
                    var fubenName = Tools.Code64(jsonData.instance.SceneNameJsonData[fubenIndex]["EventName"].str);

                    if (jsonobject["value" + i].I == item.Id && !chandiNameList.Contains(fubenName))
                    {
                        chandiNameList.Add(fubenName);
                    }
                }
            }

            if (chandiNameList.Count == 0)
            {
                chandiNameList.Add("无");
            }

            __instance.CiTiaoParent.gameObject.SetActive(true);
            StringBuilder builder = new StringBuilder();
            builder.Append("#c42e395【产地】#n\n");
            for (var i = 0; i < chandiNameList.Count; i++)
            {
                builder.Append(chandiNameList[i]);
                if (i < chandiNameList.Count - 1)
                {
                    builder.Append("、");
                }
            }
            var gameObject = __instance.CiTiao.Inst(parent);
            gameObject.SetActive(true);
            gameObject.GetComponentInChildren<WXB.SymbolText>().text = builder.ToString();
        }

        static Bag.BaseItem getItem(ref ToolTipsMag __instance)
        {
            Bag.BaseItem item = __instance.BaseItem;
            // 只要展示的不是草药, 就都不处理
            if (item == null)
            {
                Plugin.LogDebug("item is null");
                return null;
            }

            if (item.ItemType != Bag.ItemType.草药)
            {
                Plugin.LogDebug($"item Type dismatch: {item.ItemType}");
                return null;
            }
            return item;
        }

        // 获取当前草药可以作为药材的丹药ID
        public static void BuildAllUsedDanFang(int id, out List<int> main, out List<int> deputy, out List<int> intro)
        {
            List<JSONObject> list = CaoYaoHandler.ListAllDanFang();
            var mainSet = new HashSet<int>();
            var deputySet = new HashSet<int>();
            var introSet = new HashSet<int>();
            foreach (JSONObject jsonobject in list)
            {
                if (jsonobject["Type"][1].I == id || jsonobject["Type"][2].I == id)
                {
                    mainSet.Add(jsonobject["ID"].I);
                }

                if (jsonobject["Type"][3].I == id || jsonobject["Type"][4].I == id)
                {
                    deputySet.Add(jsonobject["ID"].I);
                }

                if (jsonobject["Type"][0].I == id)
                {
                    introSet.Add(jsonobject["ID"].I);
                }
            }

            main = mainSet.ToList();
            deputy = deputySet.ToList();
            intro = introSet.ToList();
        }

        // 列举所有丹方
        public static List<JSONObject> ListAllDanFang()
        {
            try
            {
                // 如果使用了 Mod 更友好的炼丹体验
                if (Chainloader.PluginInfos.ContainsKey("cn.shabywu.michangsheng.FriendlyLianDan"))
                {
                    BepInEx.PluginInfo extra = Chainloader.PluginInfos["cn.shabywu.michangsheng.FriendlyLianDan"];

                    ConfigEntry<bool> GenerateAllDanFang;
                    extra.Instance.Config.TryGetEntry(new ConfigDefinition("DanFangGenerator", "GenerateAllDanFang"), out GenerateAllDanFang);

                    if (GenerateAllDanFang == null)
                    {
                        Plugin.LogDebug("无法获取到是否生成所有丹方的配置, 可能 DLL 的版本不一致！");
                        return global::Tools.instance.getPlayer().DanFang.list;
                    }

                    if (!GenerateAllDanFang.Value)
                    {
                        Plugin.LogDebug("未开启生成丹方的功能！");
                        return global::Tools.instance.getPlayer().DanFang.list;
                    }

                    Type AllDanFangsClass = extra.Instance.GetType().GetNestedType("AllDanFangs");
                    var AllDanFangsInstance = AllDanFangsClass.GetProperty("Instance").GetValue(null, null);

                    if (AllDanFangsClass.GetField("danfangs").GetValue(AllDanFangsInstance) is Dictionary<int, List<JSONObject>> danfangs)
                    {
                        var result = new List<JSONObject>();
                        foreach (var kv in danfangs)
                        {
                            result.AddRange(kv.Value);
                        }
                        return result;
                    }

                }
            }
            catch (Exception e)
            {
                Plugin.LogDebug("尝试联动 Mod 更友好的炼丹体验 失败了, 原因: " + e.Message);
            }
            return global::Tools.instance.getPlayer().DanFang.list;
        }
    }
}
