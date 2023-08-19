namespace Terrain.Item
{
    public struct ItemStack
    {
        public ItemBase item;
        public uint count;

        public ItemStack(ItemBase itemBase, uint count)
        {
            this.item = itemBase;
            this.count = count;
        }
    }
}