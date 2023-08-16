using System.Collections.Generic;
using InternalDebug;
using NativeTrees;
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
                new BaseRandomRoomSize(new GaussianRandom(random, 0.2f, 0.5f), 20, 40, 0.2f), 
                new RandomPointCircle(random, new float2(500f, 500f), 50f));
            config.Bounds = new AABB2D(new float2(300, 300), new float2(700, 700));
            DungeonGenerator dungeonGenerator = new DungeonGenerator(config, random);
            dungeonGenerator.Dispose();
        }
    }
}