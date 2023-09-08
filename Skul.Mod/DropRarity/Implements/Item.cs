using System;
using BepInEx;
using HarmonyLib;
using Services;
using Singletons;
using GameResources;
using Level;

using DropRarity;

namespace DropRarity.Implements
{
    public class PatchGetItem
    {
        [HarmonyPatch(typeof(GearManager), "GetItemToTake", new Type[]{typeof(Random), typeof(Rarity)})]
        public class GetItemToTake {
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
                Helper.LogInfo($"GetItemToTake {rarity}");
                lock (_object) {
                    counter -=1;
                }
            }
        }

        [HarmonyPatch(typeof(GearManager), "GetItemListByRarity", new Type[]{typeof(Rarity)})]
        public class GetItemListByRarity {
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
                Helper.LogInfo($"GetItemListByRarity {rarity}");
                lock (_object) {
                    counter -=1;
                }
            }
        }
    }
}
