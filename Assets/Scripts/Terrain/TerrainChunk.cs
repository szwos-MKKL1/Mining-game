using Terrain.Blocks;
using UnityEngine;

namespace Terrain
{
    public class TerrainChunk : DataChunk
    {
        public const int ChunkSizeX = 128;
        public const int ChunkSizeY = 128;
        private BlockBase[,] blocks = new BlockBase[ChunkSizeX,ChunkSizeY];
        private bool[,] canBuild = new bool[ChunkSizeX,ChunkSizeY];

        public TerrainChunk(Vector2Int inWorldPosition) : base(inWorldPosition)
        {
        }

        public BlockBase GetBlock(Vector2Int localPos) => blocks[localPos.x, localPos.y];
        public bool GetBuildPermission(Vector2Int localPos) => canBuild[localPos.x, localPos.y];
        
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
    }
}