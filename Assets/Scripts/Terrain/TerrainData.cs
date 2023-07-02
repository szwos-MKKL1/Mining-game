using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain
{
    public class TerrainData
    {
        public Vector2Int chunkSize;

        public TerrainChunk[,] terrainChunks;
        
        public TerrainData(Vector2Int size)
        {
            chunkSize = size;
            terrainChunks = new TerrainChunk[size.x,size.y];
        }

        public Vector2Int WorldSize => new Vector2Int(chunkSize.x * TerrainChunk.ChunkSizeX, chunkSize.y * TerrainChunk.ChunkSizeY);
    }
}