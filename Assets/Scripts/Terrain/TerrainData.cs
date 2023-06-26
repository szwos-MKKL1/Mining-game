using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain
{
    public class TerrainData
    {
        public int chunkSizeX;
        public int chunkSizeY;

        private TerrainChunk[,] terrainChunks;
        
        public TerrainData(Vector2Int size)
        {
            chunkSizeX = size.x;
            chunkSizeY = size.y;
            terrainChunks = new TerrainChunk[size.x,size.y];
        }


    }
}