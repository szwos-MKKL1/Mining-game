using System.Collections;
using System.Collections.Generic;
using Terrain.Blocks;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.Outputs
{
    public class BlockCollector : CollectorBase<PosPair<BlockBase>>
    {
        public void AddToTerrain(TerrainData terrainData)
        {
            foreach (PosPair<BlockBase> blockPos in Collector)
            {
                terrainData.SetBlock(blockPos);
            }
        }
    }
}