using System.Collections.Generic;
using Terrain.Blocks;
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

        public bool GetBuildPermission(Vector2Int realPos)
        {
            return GetChunk(realPos).GetBuildPermission(GetLocalPos(realPos));
        }
    }
}