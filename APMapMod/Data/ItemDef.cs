using ItemChanger;

namespace APMapMod.Data
{
    public class ItemDef
    {
        public ItemDef(AbstractItem item)
        {
            itemName = item.name;
            poolGroup = "Unknown";
        }

        public string itemName;
        public string poolGroup = "Unknown";
        public bool persistent = false;
    }
}
