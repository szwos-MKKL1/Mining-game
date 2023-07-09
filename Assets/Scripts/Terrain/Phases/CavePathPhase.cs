using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using InternalDebug;
using Terrain.PathGraph;
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
            var realtimeSinceStartup = Time.realtimeSinceStartup;
            Profiler.BeginSample("CellularAutomataSimulator");
            //CellularAutomataSimulator simulator = CellularAutomataSimulator.Create(new Vector2Int(128*10, 1150), new bool[128*10 * 1150],
            //    new Vector2Int(128, 128));
            CellularAutomataSimulator simulator = CellularAutomataSimulator.CreateRandom(new Vector2Int(50,50), new Vector2Int(1, 1), 0.4f);
            ImageDebug.SaveImg(simulator.GetCurrentMap(), "simulation-step0.png");
            simulator.SimulationStep();
            ImageDebug.SaveImg(simulator.GetCurrentMap(), "simulation-step1.png");
            simulator.SimulationStep();
            ImageDebug.SaveImg(simulator.GetCurrentMap(), "simulation-step2.png");
            simulator.SimulationStep();
            ImageDebug.SaveImg(simulator.GetCurrentMap(), "simulation-step3.png");
            
            Profiler.EndSample();


            Debug.Log($"Pathing took {Time.realtimeSinceStartup-realtimeSinceStartup}s");
        }
    }
}