using System;
using System.Collections.Generic;
using BepInEx.Logging;
using GameResources;
using Level;
using Services;
using Singletons;

namespace DropRarity
{
    public class Helper
    {

        public static ManualLogSource Logger;
        public static void LogInfo(object data) {
            Logger.LogInfo(data);
        }
        /// <summary>
        /// Returns the minial rarity depending on the current chapter.
        /// Later Chapters have higher rarity
        /// </summary>
        public static Rarity GetMinialRarity(Rarity fallback = Rarity.Common)
        {
            Chapter.Type chapterType = Singleton<Service>.Instance.levelManager.currentChapter.type;
            Rarity rarity = fallback;
            List<Rarity> l;
        
            switch (chapterType)
            {
                case Chapter.Type.Castle:
                case Chapter.Type.HardmodeCastle:
                    l = new List<Rarity>(){Rarity.Common, Rarity.Rare, Rarity.Unique, Rarity.Legendary};
                    rarity = l[new Random().Next(l.Count)];
                    break;
                case Chapter.Type.Chapter1:
                case Chapter.Type.HardmodeChapter1:
                    l = new List<Rarity>(){Rarity.Common, Rarity.Rare};
                    rarity = l[new Random().Next(l.Count)];
                    break;
                case Chapter.Type.Chapter2:
                case Chapter.Type.HardmodeChapter2:
                    l = new List<Rarity>(){Rarity.Rare, Rarity.Unique};
                    rarity = l[new Random().Next(l.Count)];
                    break;
                case Chapter.Type.Chapter3:
                case Chapter.Type.HardmodeChapter3:
                    rarity = Rarity.Unique;
                    break;
                case Chapter.Type.Chapter4:
                case Chapter.Type.HardmodeChapter4:
                    l = new List<Rarity>(){Rarity.Unique, Rarity.Legendary};
                    rarity = l[new Random().Next(l.Count)];
                    break;
                case Chapter.Type.Chapter5:
                case Chapter.Type.HardmodeChapter5:
                    rarity = Rarity.Legendary;
                    break;
            }
            return rarity > fallback? rarity : fallback;
        }
    }
}
