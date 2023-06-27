using System.Collections;
using System.Collections.Generic;
using Terrain.Blocks;
using Terrain.Generators;
using UnityEngine;

namespace Terrain
{
    public class TerrainGenerator
    {

        //TODO can be async
        public TerrainData GenerateTerrain(GenerationData generationData)
        {
            TerrainData terrainData = new TerrainData(generationData.chunkSize);
            
            foreach (var vPhase in generationData.generationPhases)
            {
                vPhase.Generate(terrainData);
            }

            return terrainData;
        }
    }
}