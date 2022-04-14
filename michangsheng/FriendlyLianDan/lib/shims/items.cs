using System.Collections.Generic;
using JSONClass;

namespace ItemSystem.Shims
{
    public class Items
    {
        public static _ItemJsonData GetByItemID(int itemID)
        {
            return _ItemJsonData.DataDict[itemID];
        }

        public static IEnumerable<_ItemJsonData> FilterByType(WuPingType type)
        {
            foreach (var value in _ItemJsonData.DataList)
            {
                if (value.type == ((int)type))
                {
                    yield return value;
                }
            }
        }
    }
}
