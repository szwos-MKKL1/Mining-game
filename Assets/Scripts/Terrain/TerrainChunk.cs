using Terrain.Blocks;
using UnityEngine;

namespace Terrain
{
    public class TerrainChunk
    {
        public const int ChunkSizeX = 512;
        public const int ChunkSizeY = 512;
        private BlockBase[,] blocks = new BlockBase[ChunkSizeX,ChunkSizeY];
        private bool[,] canBuild = new bool[ChunkSizeX,ChunkSizeY];
        private Vector2Int inWorldPosition;

        public TerrainChunk(Vector2Int inWorldPosition)
        {
            this.inWorldPosition = inWorldPosition;
        }

        public bool[,] CanBuild
        {
            get => canBuild;
            set => canBuild = value;
        }

        public BlockBase[,] Blocks
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