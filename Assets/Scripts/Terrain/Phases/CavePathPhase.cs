using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using InternalDebug;
using Terrain.PathGraph;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;

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
            
            
            for (int i = 0; i < 5; i++)
            {
                Path p = pathFinder.NextRandomPath();
                //GraphDebug.DrawPath(p, Color.blue, 200);
                pathList.Add(p);
            }

            Graph combinedGraph = new Graph(pathList);
            GraphDebug.DrawGraph(combinedGraph, Color.green, 10);
            Graph cavernConnectionGraph = new RandomGraph(combinedGraph).GetGraph();
            GraphDebug.DrawGraph(cavernConnectionGraph, Color.blue, 300);

            //Remove edges that are too long
            cavernConnectionGraph.RemoveWhere(edge => DistanceMethods.ManhattanDistance(edge.P.Pos, edge.Q.Pos) > 150);
            GraphDebug.DrawGraph(cavernConnectionGraph, Color.cyan, 300);

            InitialStateGenerator initialStateGenerator =
                new InitialStateGenerator(
                    terrainData.RealSize,
                    cavernConnectionGraph,
                    new LayerSettings[]
                    {
                        new(10, 100),
                        new(25, 45)
                    });
            
            bool[] initial = initialStateGenerator.GetInitialMap();
            for (int i = 0; i < 100; i++)
            {
                initial[i] = true;
            }
            //ImageDebug.SaveImg(initial, terrainData.RealSize, "initial.png");
            
            var sim = CellularAutomataSimulator.CreateFromMap(terrainData.RealSize, initial);
            //var sim = CellularAutomataSimulator.CreateRandom(new Vector2Int(100, 100), 0.4f, 0);
            sim.AliveThreshold = 4;
            ImageDebug.SaveImg(sim.CellMap.ToArray(), terrainData.RealSize, "step0.png");
            var realtimeSinceStartup = Time.realtimeSinceStartup;
            Profiler.BeginSample("CellularAutomataSimulator");
            int j = 1;
            for (int i = 0; i < 11; i++)
            {
                sim.ExecuteStep();
                if (i % 2 == 0)
                {
                    ImageDebug.SaveImg(sim.CellMap.ToArray(), terrainData.RealSize, "step"+j+".png");
                    j++;
                }
                    
            }
            Profiler.EndSample();
            sim.Dispose();
            Debug.Log($"Pathing took {Time.realtimeSinceStartup-realtimeSinceStartup}s");
        }
    }
}