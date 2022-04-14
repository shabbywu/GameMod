using System;
using JSONClass;

namespace ItemSystem.Models
{
    public abstract class BaseModel
    {
        public _ItemJsonData _base;
        public int id;
        public string name;
        public int quality;
        public WuPingType type;
        public BaseModel(int itemID)
        {
            _base = ItemSystem.Shims.Items.GetByItemID(itemID);
            id = _base.id;
            name = _base.name;
            quality = _base.quality;
            type = (WuPingType)_base.type;
        }

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(WuPingType), type)}<{id}: {name}>";
        }
    }
}