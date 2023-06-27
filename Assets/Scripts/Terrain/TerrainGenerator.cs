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
            List<IGenerationPhase> generationPhases = new List<IGenerationPhase>();
            generationPhases.Add(new RawPhase(generationData));
            generationPhases.Add(new FillRockPhase(generationData, new StandardProvider(BlockRegistry.ROCK)));
            generationPhases.Add(new DecoratorPhase(generationData, new VeinGenerator(new StandardProvider(BlockRegistry.ORE), 0.05f)));
            TerrainData terrainData = new TerrainData(generationData.chunkSize);
            
            foreach (var vPhase in generationPhases)
            {
                vPhase.Generate(terrainData);
            }

            return terrainData;
        }
    }
}