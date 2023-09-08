using System;
using BepInEx;
using HarmonyLib;
using Characters.Gear.Weapons;
using Services;
using Singletons;
using GameResources;
using Level;

using DropRarity;

namespace DropRarity.Implements
{
    public class PatchGetWeapon
    {
        [HarmonyPatch(typeof(GearManager), "GetWeaponToTake", new Type[]{typeof(Random), typeof(Rarity)})]
        public class GetWeaponToTake {
            static readonly object _object = new object();
            static int counter = 0;

            [HarmonyPrefix]
            static bool Prefix(ref Rarity rarity)
            {
                Rarity replacement = Helper.GetMinialRarity(rarity);
                lock (_object) {
                    if (counter == 0) {
                        rarity = replacement;
                    }
                    counter += 1;
                }
                return true;
            }

            [HarmonyPostfix]
            static void Postfix(ref Rarity rarity)
            {
                Helper.LogInfo($"GetWeaponToTake {rarity}");
                lock (_object) {
                    counter -=1;
                }
            }
        }

        [HarmonyPatch(typeof(GearManager), "GetWeaponListByRarity", new Type[]{typeof(Rarity)})]
        public class GetWeaponListByRarity {
            static readonly object _object = new object();
            static int counter;

            [HarmonyPrefix]
            static bool Prefix(ref Rarity rarity)
            {
                Rarity replacement = Helper.GetMinialRarity(rarity);
                lock (_object) {
                    if (counter == 0) {
                        rarity = replacement;
                    }
                    counter += 1;
                }
                return true;
            }

            [HarmonyPostfix]
            static void Postfix(ref Rarity rarity)
            {
                Helper.LogInfo($"GetWeaponListByRarity {rarity}");
                lock (_object) {
                    counter -=1;
                }
            }
        }

        [HarmonyPatch(typeof(GearManager), "GetWeaponByCategory", new Type[]{typeof(Random), typeof(Rarity), typeof(Weapon.Category)})]
        public class GetWeaponByCategory {
            static readonly object _object = new object();
            static int counter;

            [HarmonyPrefix]
            static bool Prefix(ref Rarity rarity)
            {
                Rarity replacement = Helper.GetMinialRarity(rarity);
                lock (_object) {
                    if (counter == 0) {
                        rarity = replacement;
                    }
                    counter += 1;
                }
                return true;
            }

            [HarmonyPostfix]
            static void Postfix(ref Rarity rarity)
            {
                Helper.LogInfo($"GetWeaponByCategory {rarity}");
                lock (_object) {
                    counter -=1;
                }
            }
        }
    }
}
