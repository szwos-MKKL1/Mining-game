using Terrain.Item;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain.Blocks
{
    public class RockBlock : BlockBase, IDropable
    {

        public override void OnBreak(BlockEventData blockEventData)
        {
            base.OnBreak(blockEventData);
            ((IDropable)this).DropItems(blockEventData, new ItemStack(ItemRegistry.NULLITEM, 1));
        }

        public RockBlock(TileBase texture) : base(texture)
        {
            Debug.Log("dupa " + texture);
        }
    }
}