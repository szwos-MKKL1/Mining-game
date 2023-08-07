using System.Collections.Generic;
using InternalDebug;
using QuikGraph;
using Random;
using Terrain.Blocks;
using Terrain.Generator.PathGraph;
using Terrain.Generator.PathGraph.CellularAutomata;
using Terrain.Generator.PathGraph.Graphs;
using Terrain.Generator.Structure.Dungeon;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

namespace Terrain.Generator.Phases
{
    [PhaseDependency(typeof(RawPhase), DependencyOrder.Before)]
    public class StructurePhase : IGenerationPhase
    {
        private readonly GenerationData generationData;

        public StructurePhase(GenerationData generationData)
        {
            this.generationData = generationData;
        }
        
        public void Generate(TerrainData terrainData)
        {
            IRandom random = new SystemRandom();//TODO replace with random from terrain generator
            DungeonGenerator.Config config = new DungeonGenerator.Config(120,
                new BaseRandomRoomSize(new GaussianRandom(random, 0.2f, 0.7f), 15, 35, 15, 35), 
                new RandomPointCircle(random, new float2(500f, 500f), 50f));
            DungeonGenerator dungeonGenerator = new DungeonGenerator(config);
            
        }
    }
}