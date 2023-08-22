using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain.Blocks
{
    public class BlockBase
    {
        protected readonly TileBase texture;
        public BlockBase(TileBase texture)
        {
            this.texture = texture;
        }

        public virtual void OnBreak(BlockEventData blockEventData)
        {
            Debug.Log("Breaking block " + blockEventData.posInWorld);
        }
        public TileBase Texture => texture;
    }
}