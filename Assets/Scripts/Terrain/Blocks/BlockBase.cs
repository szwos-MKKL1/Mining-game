using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain.Blocks
{
    public class BlockBase : AbstractBlock
    {
        public BlockBase(TileBase texture)
        {
            this.texture = texture;
        }

        public override void OnBreak(BlockEventData blockEventData)
        {
            Debug.Log("Breaking block " + blockEventData.posInWorld);
        }
    }
}