using System.Collections;
using System.Collections.Generic;
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

            TerrainData terrainData = new TerrainData(new Vector2Int(generationData.SizeX, generationData.SizeY));
            
            foreach (var vPhase in generationPhases)
            {
                vPhase.Generate(terrainData);
            }

            return terrainData;
        }
    }
}