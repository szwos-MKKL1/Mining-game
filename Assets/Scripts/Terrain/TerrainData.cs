using System.Collections.Generic;
using Terrain.Blocks;
using Terrain.Chunk;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain
{
    public class TerrainData : ChunkedData<TerrainChunk>
    {
        public TerrainData(Vector2Int chunkCount) : base(chunkCount, new Vector2Int(TerrainChunk.ChunkSizeX, TerrainChunk.ChunkSizeY))
        {
        }

        public BlockBase GetBlock(Vector2Int realPos)
        {
            return GetChunk(realPos).GetBlock(GetLocalPos(realPos));
        }
        
        public void SetBlock(Vector2Int realPos, BlockBase block)
        {
            GetChunk(realPos).SetBlock(GetLocalPos(realPos), block);
        }

        public bool IsBuildable(Vector2Int realPos)
        {
            return GetChunk(realPos).IsBuildable(GetLocalPos(realPos));
        }
    }
}