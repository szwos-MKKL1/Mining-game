using Terrain.Blocks;
using UnityEngine;

namespace Terrain
{
    public class TerrainChunk
    {
        private AbstractBlock[,] blocks = new AbstractBlock[512,512];
        private Vector2Int inWorldPosition;//TODO should be int or vector2int?

        
        
        public AbstractBlock[,] Blocks
        {
            get => blocks;
            set => blocks = value;
        }

        public Vector2Int InWorldPosition
        {
            get => inWorldPosition;
            set => inWorldPosition = value;
        }
    }
}