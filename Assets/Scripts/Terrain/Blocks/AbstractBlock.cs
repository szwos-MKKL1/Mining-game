using UnityEngine.Tilemaps;

namespace Terrain.Blocks
{
    //TODO not sure if it is really needed
    public abstract class AbstractBlock
    {
        protected TileBase texture;

        public abstract void OnBreak(BlockEventData blockEventData);

        public TileBase Texture => texture;
    }
}