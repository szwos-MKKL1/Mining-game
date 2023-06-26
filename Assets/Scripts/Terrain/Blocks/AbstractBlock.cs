using UnityEngine.Tilemaps;

namespace Terrain.Blocks
{
    public abstract class AbstractBlock
    {
        protected TileBase texture;

        public abstract void OnBreak(BlockEventData blockEventData);
    }
}