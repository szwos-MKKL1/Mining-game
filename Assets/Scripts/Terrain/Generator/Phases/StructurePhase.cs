﻿using System.Collections.Generic;
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
            DungeonGenerator.Config config = new DungeonGenerator.Config(50,
                new BaseRandomRoomSize(random, 5, 30, 5, 30), 
                new RandomPointCircle(random, new float2(500f, 500f), 50f));
            DungeonGenerator dungeonGenerator = new DungeonGenerator(config);
            foreach (var room in dungeonGenerator.Rooms())
            {
                Vector3 pos0 = new Vector3(room.Pos.x, room.Pos.y);
                Vector3 size = new Vector3(room.Size.x, room.Size.y);
                DrawLineScaled(pos0, pos0 + new Vector3(size.x, 0));
                DrawLineScaled(pos0, pos0 + new Vector3(0, size.y));
                DrawLineScaled(pos0 + new Vector3(0, size.y), pos0 + new Vector3(size.x, size.y));
                DrawLineScaled(pos0 + new Vector3(size.x, 0), pos0 + new Vector3(size.x, size.y));
            }
        }

        private static void DrawLineScaled(Vector3 a, Vector3 b)
        {
            Debug.DrawLine(a * 0.16f, b * 0.16f, Color.blue, 60f, false);
        }
    }
}