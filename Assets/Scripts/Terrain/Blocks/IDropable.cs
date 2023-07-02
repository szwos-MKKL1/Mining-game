using Terrain.Item;
using UnityEngine;

namespace Terrain.Blocks
{
    public interface IDropable
    {

        public void DropItems(BlockEventData blockEventData, ItemStack item)
        {
            //TODO spawn items?
            Debug.Log("Dropping " + item + " at " + blockEventData.posInWorld);
        }
    }
}