using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using InternalDebug;
using Terrain.PathGraph;
using UnityEngine;
namespace Terrain.Phases
{
    [PhaseDependency(typeof(RawPhase), DependencyOrder.Before)]
    public class CavePathPhase : IGenerationPhase
    {
        private readonly GenerationData generationData;
        private List<Path> pathList = new();

        public CavePathPhase(GenerationData generationData, out IEnumerable<Path> paths)
        {
            this.generationData = generationData;
            paths = pathList;
        }

        public void Generate(TerrainData terrainData)
        {
            //Dictionary<Vector2Int, byte> dict = DistanceMapUtils.DistanceMapNotBuildable(terrainData, generationData.borderWeight);

            Vector2Int startPoint = new Vector2Int(100, 100);
            Vector2Int destinationPoint = new Vector2Int(1000, 1000);
            
            RandomGraph randomGraph = new RandomGraph(terrainData.RealSize, 300, 68);
            Graph graph = randomGraph.GetGraph();
            //GraphDebug.DrawGraph(graph, Color.red, 200);
            graph.RemoveWhere(s => !terrainData.GetBuildPermission(new Vector2Int((int)s.Pos.x, (int)s.Pos.y)));
            //GraphDebug.DrawGraph(graph, Color.white, 200);
            PathFinder pathFinder = new PathFinder(graph, startPoint, destinationPoint, terrainData.RealSize, generationData.pathFindingSettings);
            var realtimeSinceStartup = Time.realtimeSinceStartup;
            
            for (int i = 0; i < 5; i++)
            {
                Path p = pathFinder.NextRandomPath();
                //GraphDebug.DrawPath(p, Color.blue, 200);
                pathList.Add(p);
            }

            Debug.Log($"Pathing took {Time.realtimeSinceStartup-realtimeSinceStartup}s");
        }
    }
}