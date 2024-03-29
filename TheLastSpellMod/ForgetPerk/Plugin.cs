﻿using System;
using System.Reflection;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

using TMPro;
using TPLib;
using TPLib.Localization;
using TheLastStand.Framework;
using TheLastStand.Framework.UI;

using TheLastStand.Manager;
using TheLastStand.Manager.Unit;
using TheLastStand.Model;
using TheLastStand.Model.Unit.Perk;
using TheLastStand.Controller.Unit.Perk;
using TheLastStand.View;
using TheLastStand.View.CharacterSheet;
using TheLastStand.View.Generic;
using TheLastStand.View.HUD;
using TheLastStand.View.Skill.SkillAction.UI;
using TheLastStand.View.Unit.Perk;

using UnityEngine;
using UnityEngine.UI;

namespace ForgetPerk
{
    [BepInPlugin("cn.shabywu.the_last_stand.forget_perk", "遗忘天赋", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<uint> DamnedSoulsCost;
        public delegate void Log(object data);
        public static Log LogDebug;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            LogDebug = Logger.LogDebug;

            DamnedSoulsCost = Config.Bind("TheLastStand.ForgetPerk",  "DamnedSoulsCost", (uint)500, "污秽精华消耗");
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Harmony.CreateAndPatchAll(typeof(OnPerkButtonClick));
            setupText();
        }
        
        private void setupText() {
            Localizer.Set("简体中文", "CharacterSheet_ForgetPerkButton", "遗忘 {0}<sprite=23>");
            Localizer.Set("English", "CharacterSheet_ForgetPerkButton", "Forget {0}<sprite=23>");

            Localizer.Set("简体中文", "CharacterSheet_ForgetPerkConsentText", "遗忘天赋需要 {0}<sprite=23>, 遗忘后将返还天赋点。");
            Localizer.Set("English", "CharacterSheet_ForgetPerkConsentText", "Forgetting the perk requires {0}<sprite=23>, and the perk point will be returned after forgetting.");
        }

        static void refreshTrainButton(ref UnitPerkDisplay selectedPerk) {
            BetterButton trainButton = (BetterButton)typeof(UnitPerkTreeView).GetField("trainButton", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(TPSingleton<CharacterSheetPanel>.Instance.UnitPerkTreeView);
            TextMeshProUGUI text = (TextMeshProUGUI)typeof(BetterButton).GetField("buttonText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(trainButton);
            text.richText = true;
            if (selectedPerk.Perk != null && selectedPerk.Perk.Unlocked) {
                LogDebug("set trainButton text to CharacterSheet_ForgetPerkButton");
                trainButton.ChangeText(Localizer.Format("CharacterSheet_ForgetPerkButton", new object[]{DamnedSoulsCost.Value}));
                trainButton.Interactable = UnitPerkTreeControllerExt.CanForgetPerk();
            } else {
                LogDebug("set trainButton text to CharacterSheet_ValidateButton");
                trainButton.ChangeText(Localizer.Get("CharacterSheet_ValidateButton"));
            }
        }

        [HarmonyPatch(typeof(UnitPerkDisplay), "OnPerkButtonClick")]
        public class OnPerkButtonClick {
            static void Prefix(ref UnitPerkDisplay __instance, out bool __state) {
                __state = false;
                if ((UnityEngine.Object)__instance == (UnityEngine.Object)null) {
                    LogDebug("OnPerkButtonClick but no perk is selected");
                    return;
                }
                UnitPerkTreeView UnitPerkTreeView = TPSingleton<CharacterSheetPanel>.Instance.UnitPerkTreeView;
                if ((UnityEngine.Object)UnitPerkTreeView.SelectedPerk == (UnityEngine.Object)__instance) {
                    LogDebug("OnPerkButtonClick but same perk is selected");
                    return;
                }
                __state = true;
            }

            static void Postfix(ref UnitPerkDisplay __instance, bool __state)
            {
                if (__state) {
                    refreshTrainButton(ref __instance);
                }
            }
        }


        [HarmonyPatch(typeof(UnitPerkTreeView), "OnTrainButtonClick")]
        [HarmonyPrefix]
        static bool OnTrainButtonClick(ref UnitPerkTreeView __instance) {
            UnitPerkTree UnitPerkTree = __instance.UnitPerkTree;
            UnitPerkDisplay SelectedPerk = __instance.SelectedPerk;
            if (SelectedPerk.Perk.Unlocked) {
                GenericConsent.Open(Localizer.Format(
                    "CharacterSheet_ForgetPerkConsentText", 
                    new object[] { DamnedSoulsCost.Value }), 
                    delegate {(new UnitPerkTreeControllerExt(UnitPerkTree)).ForgetPerk();}, 
                    delegate {LogDebug("取消遗忘操作");}
                );
                return false;
            }
            return true;
        }

        public class UnitPerkTreeControllerExt {
            public UnitPerkTree UnitPerkTree;

            public UnitPerkTreeControllerExt(UnitPerkTree UnitPerkTree) {
                this.UnitPerkTree = UnitPerkTree;
            }

            public static bool CanForgetPerk() {
                if (ApplicationManager.Application.DamnedSouls >= DamnedSoulsCost.Value) {
                    return TPSingleton<GameManager>.Instance.Game.State != Game.E_State.GameOver;
                }
                LogDebug("Not enough DamnedSouls");
                return false;
            }

            public void ForgetPerk() {
                if (!CanForgetPerk()) {
                    return;
                }
                UnitPerkDisplay SelectedPerk = UnitPerkTree.UnitPerkTreeView.SelectedPerk;
                LogDebug("ForgetPerk!!!");
                // 添加特性点
                UnitPerkTree.PlayableUnit.PerksPoints++;
                // 扣除污秽精华
                ApplicationManager.Application.DamnedSouls -= DamnedSoulsCost.Value;
                // 锁上特性
                SelectedPerk.Perk.PerkController.Lock();
                // 刷新选择的 UnitPerkTreeView 状态
                UnitPerkTree.UnitPerkTreeView.RefreshSelectedPerk(SelectedPerk);
                UnitPerkTree.UnitPerkTreeView.RefreshPerkPoints();
                // 更新按钮文案
                refreshTrainButton(ref SelectedPerk);

                // 触发回调 perk 更新
                // UnitPerkTree.UnitPerkTreeController.OnSetNewPerk(UnitPerkTree.UnitPerkTreeView.SelectedPerk.PerkDefinition.Id, UnitPerkTree.UnitPerkTreeView.SelectedPerk.Perk);
                OnSetNewPerk.Invoke(UnitPerkTree.UnitPerkTreeController, new object[]{SelectedPerk.PerkDefinition.Id, SelectedPerk.Perk, true});
                //UnitPerkTree.UnitPerkTreeController.UpdateTiersAvailability();
                UpdateTiersAvailability();

                // 更新 CharacterSheetPanel 状态
                TPSingleton<CharacterSheetPanel>.Instance.Refresh();
                // 刷新文案并展示动画
                PhasePanel PhasePanel = GameView.TopScreenPanel.TurnPanel.PhasePanel;
                TextMeshProUGUI soulsText = (TextMeshProUGUI)typeof(PhasePanel).GetField("soulsText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PhasePanel);

                GameObject gameObject = soulsText.gameObject;
                ResourceTextDisplay soulsTextDisplay = gameObject.GetComponent<ResourceTextDisplay>();
				if ((UnityEngine.Object)soulsTextDisplay == (UnityEngine.Object)null)
				{
					soulsTextDisplay = gameObject.AddComponent<ResourceTextDisplay>();
				}
                typeof(ResourceTextDisplay).GetField("text", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(soulsTextDisplay, soulsText);
                soulsTextDisplay.RefreshValue((int)ApplicationManager.Application.DamnedSouls, () => ObjectPooler.GetPooledComponent<GainDamnedSoulsDisplay>("GainDamnedSoulsDisplay", ResourcePooler.LoadOnce<GainDamnedSoulsDisplay>("Prefab/Displayable Effect/UI Effect Displays/GainDamnedSoulsDisplay", failSilently: false), null, dontSetParent: false));
            }

            public MethodInfo OnSetNewPerk {
                get {
                    MethodInfo OnSetNewPerk = typeof(UnitPerkTreeController).GetMethod("OnSetNewPerk", BindingFlags.NonPublic | BindingFlags.Instance);
                    return OnSetNewPerk;
                }
            }

            // 锁上不符合要求的特性栏
            private void UpdateTiersAvailability(int startIndex = 1, bool refreshView = true)
            {
                int i = startIndex;
                bool flag = true;
                for (; i < UnitPerkTree.UnitPerkTiers.Count; i++)
                {
                    if (UnitPerkTree.UnitPerkTiers[i].RequiredPerksCount > UnitPerkTree.PlayableUnit.UnlockedPerksCount)
                    {
                        UnitPerkTree.UnitPerkTiers[i].UnitPerkTierController.UnitPerkTier.Available = false;
                    }
                    if (refreshView)
                    {
                        UnitPerkTree.UnitPerkTiers[i].UnitPerkTierView.RefreshAvailability(true);
                    }
                }
            }
        }
    }
}
