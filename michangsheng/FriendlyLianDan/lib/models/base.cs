using System;
using ItemSystem;

namespace ItemSystem.Models
{
    public abstract class BaseModel
    {
        public ItemSystem.Loaders.Item _base;
        public int id;
        public string name;
        public int quality;
        public WuPingType type;
        public BaseModel(int itemID)
        {
            _base = ItemSystem.Loaders.Items.GetByItemID(itemID);
            id = _base.id;
            name = _base.name;
            quality = _base.quality;
            type = _base.type;
        }

        public override string ToString()
        {
            return _base.ToString();
        }
    }
}