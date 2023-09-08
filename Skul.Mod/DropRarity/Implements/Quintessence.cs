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
    public class PatchGetQuintessence
    {
        [HarmonyPatch(typeof(GearManager), "GetQuintessenceToTake", new Type[]{typeof(Random), typeof(Rarity)})]
        public class GetQuintessenceToTake {
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
                Helper.LogInfo($"GetQuintessenceToTake {rarity}");
                lock (_object) {
                    counter -=1;
                }
            }
        }

        [HarmonyPatch(typeof(GearManager), "GetEssenceListByRarity", new Type[]{typeof(Rarity)})]
        public class GetEssenceListByRarity {
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
                Helper.LogInfo($"GetEssenceListByRarity {rarity}");
                lock (_object) {
                    counter -=1;
                }
            }
        }
    }
}
