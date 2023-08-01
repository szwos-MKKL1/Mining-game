using Terrain.Blocks;
using Terrain.Chunk;
using UnityEngine;

namespace Terrain
{
    public class TerrainChunk : DataChunk
    {
        public const int ChunkSizeX = 128;
        public const int ChunkSizeY = 128;
        private BlockBase[] blocks = new BlockBase[ChunkSizeX*ChunkSizeY];
        private bool[] canBuild = new bool[ChunkSizeX*ChunkSizeY];

        public TerrainChunk(Vector2Int inWorldPosition) : base(inWorldPosition)
        {
        }

        public BlockBase GetBlock(Vector2Int localPos) => blocks[localPos.x*ChunkSizeX+localPos.y];
        public void SetBlock(Vector2Int localPos, BlockBase block) => blocks[localPos.x*ChunkSizeX+localPos.y] = block;
        public bool IsBuildable(Vector2Int localPos) => canBuild[localPos.x*ChunkSizeX+localPos.y];
        
        public bool[] CanBuild
        {
            get => canBuild;
            set => canBuild = value;
        }
        
        public BlockBase[] Blocks
        {
            get => blocks;
            set => blocks = value;
        }
    }
}