using System.Collections.Generic;
using ItemSystem;
using ItemSystem.Models;

namespace FriendlyLianDan
{

    public class Combination
    {
        public int num;
        public Herbs herbs;

        public Combination(int num, Herbs herbs)
        {
            this.num = num;
            this.herbs = herbs;
        }

        public override string ToString()
        {
            return $"{num} * {herbs}";
        }
    }

    public class Tables
    {
        private static Tables instance = null;
        private static readonly object _lock = new object();
        private static int threshold = 11;

        public Dictionary<YaoXing, List<Combination>> intro;
        public Dictionary<YaoXing, List<Combination>> main;
        public Dictionary<YaoXing, List<Combination>> deputy;

        public static IEnumerable<Combination> listPossibleIntro(YaoXing key, bool fuzzy = false, int maxNum = -1)
        {
            if (Instance.intro.ContainsKey(key))
            {
                foreach (var item in filterByNum(Instance.intro[key], maxNum))
                {
                    yield return item;
                }
            }

            if (fuzzy)
            {
                foreach (var possibleKey in Instance.intro.Keys)
                {
                    if (key.kind == possibleKey.kind && key.intensity < possibleKey.intensity)
                    {
                        foreach (var item in filterByNum(Instance.intro[possibleKey], maxNum))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        public static IEnumerable<Combination> listPossibleMain(YaoXing key, bool fuzzy = false, int maxNum = -1)
        {
            if (Instance.main.ContainsKey(key))
            {
                foreach (var item in filterByNum(Instance.main[key], maxNum))
                {
                    yield return item;
                }
            }

            if (fuzzy)
            {
                foreach (var possibleKey in Instance.main.Keys)
                {
                    if (key.kind == possibleKey.kind && key.intensity < possibleKey.intensity)
                    {
                        foreach (var item in filterByNum(Instance.main[possibleKey], maxNum))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        public static IEnumerable<Combination> listPossibleDeputy(YaoXing key, bool fuzzy = false, int maxNum = -1)
        {
            if (Instance.deputy.ContainsKey(key))
            {
                foreach (var item in filterByNum(Instance.deputy[key], maxNum))
                {
                    yield return item;
                }
            }

            if (fuzzy)
            {
                foreach (var possibleKey in Instance.deputy.Keys)
                {
                    if (key.kind == possibleKey.kind && key.intensity < possibleKey.intensity)
                    {
                        foreach (var item in filterByNum(Instance.deputy[possibleKey], maxNum))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        private static IEnumerable<Combination> filterByNum(IEnumerable<Combination> iterable, int maxNum = -1)
        {
            foreach (var item in iterable)
            {
                if (maxNum == -1 || item.num <= maxNum)
                {
                    yield return item;
                }
            }
        }

        private Tables()
        {
            string[] fields = new string[] { "intro", "main", "deputy" };

            intro = new Dictionary<YaoXing, List<Combination>>();
            main = new Dictionary<YaoXing, List<Combination>>();
            deputy = new Dictionary<YaoXing, List<Combination>>();

            foreach (var item in ItemSystem.Shims.Items.FilterByType(WuPingType.草药))
            {
                Herbs herbs = new Herbs(item.id);        
                foreach (string field in fields)
                {
                    var herbsYaoXing = (YaoXing)typeof(Herbs).GetField(field).GetValue(herbs);
                    var container = (Dictionary<YaoXing, List<Combination>>)typeof(Tables).GetField(field).GetValue(this);
                    for (int i = 1; i <= threshold; i++)
                    {
                        var k = herbsYaoXing * i;
                        if (!container.ContainsKey(k))
                        {
                            container.Add(k, new List<Combination>());
                        }
                        container[k].Add(new Combination(i, herbs));
                    }
                }
            }
        }

        public static Tables Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = new Tables();
                        }
                    }
                }
                return instance;
            }
        }
    }
}