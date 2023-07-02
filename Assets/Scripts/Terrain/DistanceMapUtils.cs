using System.Collections.Generic;
using InternalDebug;
using UnityEngine;

namespace Terrain
{
    public class DistanceMapUtils
    {
        public static Dictionary<Vector2Int, byte> DistanceMapNotBuildable(TerrainData terrainData, byte distance)
        {
            Vector2Int realSize = terrainData.RealSize;
            DistanceMap distanceMap = new DistanceMap(FindBorderingBlocks(terrainData), 
                pos => pos.x >= 0 && pos.x < realSize.x && pos.y >= 0 && pos.y < realSize.y &&
                terrainData.GetBuildPermission(pos));
            return distanceMap.Generate(distance);
        }

        private static readonly Vector2Int[] neighbors = { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };
        
        public static HashSet<Vector2Int> FindBorderingBlocks(TerrainData terrainData)
        {
            HashSet<Vector2Int> borderBlocks = new();
            Vector2Int realSize = terrainData.RealSize;
            for (int x = 0; x < realSize.x; x++)
            {
                for (int y = 0; y < realSize.y; y++)
                {
                    Vector2Int cellPos = new Vector2Int(x, y);
                    if (borderBlocks.Contains(cellPos) || terrainData.GetBuildPermission(cellPos)) continue;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2Int offset = cellPos + neighbors[i];
                        if (offset.x < 0 || offset.x >= realSize.x || offset.y < 0 || offset.y >= realSize.y || !terrainData.GetBuildPermission(offset)) continue;
                        borderBlocks.Add(cellPos);
                        break;
                    }
                }
            }

            return borderBlocks;
        }
    }
}