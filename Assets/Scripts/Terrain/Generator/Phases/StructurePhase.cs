using System.Collections.Generic;
using System.Numerics;
using InternalDebug;
using NativeTrees;
using QuikGraph;
using Random;
using Terrain.Blocks;
using Terrain.Generator.PathGraph;
using Terrain.Generator.PathGraph.CellularAutomata;
using Terrain.Generator.PathGraph.Graphs;
using Terrain.Generator.Structure;
using Terrain.Generator.Structure.Dungeon;
using Terrain.Outputs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;
using Vector2 = UnityEngine.Vector2;

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
            Structure.Structure dungeonStructure = new DungeonStructure();
            dungeonStructure.getStructureBlocks(new Structure.Structure.Context(new SystemRandom(),
                new Vector2(300, 300))).AddToTerrain(terrainData);
        }
    }
}