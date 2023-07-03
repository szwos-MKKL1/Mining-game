using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using InternalDebug;
using Terrain.PathGraph;
using UnityEngine;
namespace Terrain.Phases
{
    public class CavePathPhase : IGenerationPhase
    {
        private readonly GenerationData generationData;

        public CavePathPhase(GenerationData generationData)
        {
            this.generationData = generationData;
        }

        public void Generate(TerrainData terrainData)
        {
            Dictionary<Vector2Int, byte> dict = DistanceMapUtils.DistanceMapNotBuildable(terrainData, generationData.borderWeight);

            Vector2Int startPoint = new Vector2Int(100, 100);
            
            RandomGraph randomGraph = new RandomGraph(terrainData.RealSize, 50, 68);
            Graph graph = randomGraph.GetGraph();
            GraphDebug.DrawGraph(graph, Color.red, 200);
            graph.RemoveWhere(s => !terrainData.GetBuildPermission(new Vector2Int((int)s.Pos.x, (int)s.Pos.y)));
            GraphDebug.DrawGraph(graph, Color.white, 200);

            const int seed = 69;
            CostApplicatorSettings costApplicatorSettings = new CostApplicatorSettings(terrainData.RealSize)
                {
                    Destination = startPoint,
                    BorderMultiplier = 1f,
                    DistanceMultiplier = 100f,
                    RandomMax = 100,
                    RandomMin = 100
                };

            CostApplicator costApplicator = new CostApplicator(costApplicatorSettings, dict, seed);
            costApplicator.Apply(graph);
        }
    }
}