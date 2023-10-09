using System;
using System.Reflection;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

using TMPro;
using TPLib;
using TPLib.Localization;
using TheLastStand.Model;
using TheLastStand.Manager;
using TheLastStand.View.CharacterSheet;
using TheLastStand.Model.Unit.Perk;
using TheLastStand.View.Unit.Perk;
using TheLastStand.Controller.Unit.Perk;
using TheLastStand.Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ForgetPerk
{
    [BepInPlugin("cn.shabywu.the_last_stand.forget_perk", "遗忘特性", "1.0.0")]
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
        }

        static void refreshTrainButton(ref UnitPerkDisplay selectedPerk) {
            BetterButton trainButton = (BetterButton)typeof(UnitPerkTreeView).GetField("trainButton", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(TPSingleton<CharacterSheetPanel>.Instance.UnitPerkTreeView);
            TextMeshProUGUI text = (TextMeshProUGUI)typeof(BetterButton).GetField("buttonText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(trainButton);
            text.richText = true;
            if (selectedPerk.Perk != null && selectedPerk.Perk.Unlocked) {
                LogDebug("set trainButton text to CharacterSheet_ForgetPerkButton");
                trainButton.ChangeText(Localizer.Format("CharacterSheet_ForgetPerkButton", new object[]{DamnedSoulsCost.Value}));
                trainButton.Interactable = UnitPerkTreeController.CanForgetPerk();
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
                (new UnitPerkTreeController(UnitPerkTree)).ForgetPerk();
                return false;
            }
            return true;
        }

        public class UnitPerkTreeController {
            public UnitPerkTree UnitPerkTree;

            public UnitPerkTreeController(UnitPerkTree UnitPerkTree) {
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
                LogDebug("ForgetPerk!!!");
                // 添加特性点
                UnitPerkTree.PlayableUnit.PerksPoints++;
                // 扣除污秽精华
                ApplicationManager.Application.DamnedSouls -= DamnedSoulsCost.Value;
                // 锁上特性
                UnitPerkTree.UnitPerkTreeView.SelectedPerk.Perk.PerkController.Lock();
                // 刷新选择的 UnitPerkTreeView 状态
                UnitPerkTree.UnitPerkTreeView.RefreshSelectedPerk(UnitPerkTree.UnitPerkTreeView.SelectedPerk);
                UnitPerkTree.UnitPerkTreeView.RefreshPerkPoints();
                // 更新按钮文案
                UnitPerkDisplay SelectedPerk = UnitPerkTree.UnitPerkTreeView.SelectedPerk;
                refreshTrainButton(ref SelectedPerk);

                // 触发回调 perk 更新
                // UnitPerkTree.UnitPerkTreeController.OnSetNewPerk(UnitPerkTree.UnitPerkTreeView.SelectedPerk.PerkDefinition.Id, UnitPerkTree.UnitPerkTreeView.SelectedPerk.Perk);
                OnSetNewPerk.Invoke(UnitPerkTree.UnitPerkTreeController, new object[]{UnitPerkTree.UnitPerkTreeView.SelectedPerk.PerkDefinition.Id, UnitPerkTree.UnitPerkTreeView.SelectedPerk.Perk});
                //UnitPerkTree.UnitPerkTreeController.UpdateTiersAvailability();
                UpdateTiersAvailability.Invoke(UnitPerkTree.UnitPerkTreeController, new object[]{});

                // 更新 CharacterSheetPanel 状态
                TPSingleton<CharacterSheetPanel>.Instance.Refresh();
                // 更新
            }

            public MethodInfo OnSetNewPerk {
                get {
                    MethodInfo OnSetNewPerk = typeof(UnitPerkTreeController).GetMethod("OnSetNewPerk", BindingFlags.NonPublic | BindingFlags.Instance);
                    return OnSetNewPerk;
                }
            }

            public MethodInfo UpdateTiersAvailability {
                get {
                    MethodInfo UpdateTiersAvailability = typeof(UnitPerkTreeController).GetMethod("UpdateTiersAvailability", BindingFlags.NonPublic | BindingFlags.Instance);
                    return UpdateTiersAvailability;
                }
            }
        }
    }
}
