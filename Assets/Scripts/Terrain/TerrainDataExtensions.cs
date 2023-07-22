using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Terrain
{
    public static class TerrainDataExtensions
    {
        public static bool[] GetNotBuildableMap(this TerrainData terrainData) //TODO takes 150ms - too long
        {
            bool[] buildableMap = new bool[terrainData.RealSize.x * terrainData.RealSize.y];
            foreach (KeyValuePair<Vector2Int, TerrainChunk> chunkPair in terrainData)
            {
                bool[] chunkBuildableMap = chunkPair.Value.CanBuild;
                bool anyNotBuildable = chunkBuildableMap.Any(x => x == false);
                if (!anyNotBuildable) continue;

                Vector2Int chunkRealPos = chunkPair.Key * terrainData.ChunkSize;
                for (int i = 0; i < chunkBuildableMap.Length; i++)
                {
                    Vector2Int localPos = new Vector2Int(i / terrainData.ChunkSize.y, i % terrainData.ChunkSize.x); //TODO this is temporary fix
                    Vector2Int pos = chunkRealPos + localPos;
                    buildableMap[pos.x + pos.y * terrainData.RealSize.x] = !chunkBuildableMap[i];
                }
            }

            return buildableMap;
        }
    }
}