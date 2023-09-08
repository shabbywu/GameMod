using BepInEx;
using HarmonyLib;
using Services;
using DropRarity.Implements;

namespace DropRarity
{
    [BepInPlugin("cn.shabywu.skul_mod.drop_rarity", "掉落稀有度控制", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Helper.Logger = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(PatchGetItem.GetItemToTake));
            Harmony.CreateAndPatchAll(typeof(PatchGetItem.GetItemListByRarity));
            
            Harmony.CreateAndPatchAll(typeof(PatchGetQuintessence.GetQuintessenceToTake));
            Harmony.CreateAndPatchAll(typeof(PatchGetQuintessence.GetEssenceListByRarity));

            Harmony.CreateAndPatchAll(typeof(PatchGetWeapon.GetWeaponToTake));
            Harmony.CreateAndPatchAll(typeof(PatchGetWeapon.GetWeaponListByRarity));
            Harmony.CreateAndPatchAll(typeof(PatchGetWeapon.GetWeaponByCategory));
        }
    }
}
