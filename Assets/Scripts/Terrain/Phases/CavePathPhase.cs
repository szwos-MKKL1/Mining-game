﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DelaunatorSharp;
using InternalDebug;
using Terrain.Blocks;
using Terrain.PathGraph;
using Terrain.PathGraph.CellularAutomata;
using Terrain.PathGraph.Graphs;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;
using Vector2 = System.Numerics.Vector2;

namespace Terrain.Phases
{
    [PhaseDependency(typeof(RawPhase), DependencyOrder.Before)]
    public class CavePathPhase : IGenerationPhase
    {
        private readonly GenerationData generationData;
        private List<Path<WeightedGraphNode>> pathList = new();

        public CavePathPhase(GenerationData generationData, out IEnumerable<Path<WeightedGraphNode>> paths)
        {
            this.generationData = generationData;
            paths = pathList;
        }

        public void Generate(TerrainData terrainData)
        {
            //Dictionary<Vector2Int, byte> dict = DistanceMapUtils.DistanceMapNotBuildable(terrainData, generationData.borderWeight);

            Vector2Int startPoint = new Vector2Int(100, 100);
            Vector2Int destinationPoint = new Vector2Int(1000, 1000);

            RandomGraph<WeightedGraphNode> randomGraph = RandomGraph<WeightedGraphNode>.CreateFromPoints(v => new WeightedGraphNode(v),terrainData.RealSize, 300, 68);
            Graph<WeightedGraphNode> graph = randomGraph.GetGraph();
            //GraphDebug.DrawGraph(graph, Color.red, 200);
            graph.RemoveWhere(s => !terrainData.GetBuildPermission(new Vector2Int((int)s.Pos.x, (int)s.Pos.y)));
            //GraphDebug.DrawGraph(graph, Color.white, 200);
            PathFinder pathFinder = new PathFinder(graph, startPoint, destinationPoint, terrainData.RealSize, generationData.pathFindingSettings);
            
            
            for (int i = 0; i < 5; i++)
            {
                Path<WeightedGraphNode> p = pathFinder.NextRandomPath();
                //GraphDebug.DrawPath(p, Color.blue, 200);
                pathList.Add(p);
            }

            Graph<WeightedGraphNode> combinedGraph = new Graph<WeightedGraphNode>(pathList);
            GraphDebug.DrawGraph(combinedGraph, Color.green, 10);
            Graph<PosGraphNode> cavernConnectionGraph = RandomGraph<PosGraphNode>.CreateAroundGraph(combinedGraph, pos => new PosGraphNode(pos)).GetGraph();
            GraphDebug.DrawGraph(cavernConnectionGraph, Color.blue, 300);

            //Remove edges that are too long
            cavernConnectionGraph.RemoveWhere(edge => DistanceMethods.ManhattanDistance(edge.P.Pos, edge.Q.Pos) > 150);
            GraphDebug.DrawGraph(cavernConnectionGraph, Color.cyan, 300);

            Layer[] layers = { new(100), new(45) };
            List<GeneratorNode> genNodes = new();
            foreach (var node in cavernConnectionGraph)
            {
                LayerGenerationSettings[] genSettings = {
                    new(10, 0),
                    new(25, 1)
                };
                genNodes.Add(new GeneratorNode(node.Pos.ToVectorInt(), genSettings));
            }
            
            //TODO maze
            
            InitialMapGenerator initialMapGenerator = new InitialMapGenerator(terrainData.RealSize, layers, new []
            {
                new CircleAroundNodeGen(genNodes)
            });
            
            bool[] initial = initialMapGenerator.GetInitialMap();
            for (int i = 0; i < 100; i++)
            {
                initial[i] = true;
            }
            //ImageDebug.SaveImg(initial, terrainData.RealSize, "initial.png");
            
            var sim = CellularAutomataSimulator.CreateFromMap(terrainData.RealSize, initial);
            //var sim = CellularAutomataSimulator.CreateRandom(new Vector2Int(100, 100), 0.4f, 0);
            sim.AliveThreshold = 5;
            //ImageDebug.SaveImg(sim.CellMap.ToArray(), terrainData.RealSize, "step0.png");
            var realtimeSinceStartup = Time.realtimeSinceStartup;
            Profiler.BeginSample("CellularAutomataSimulator");
            int j = 1;
            for (int i = 0; i < 11; i++)
            {
                sim.ExecuteStep();
                if (i % 2 == 0)
                {
                    //ImageDebug.SaveImg(sim.CellMap.ToArray(), terrainData.RealSize, "step"+j+".png");
                    j++;
                }
                    
            }
            Profiler.EndSample();

            Vector2Int realsize = terrainData.RealSize;
            int index = 0;
            foreach (var alive in sim.CellMap)
            {
                if (alive)
                {
                    terrainData.SetBlock(new Vector2Int(index % realsize.x, index / realsize.y), BlockRegistry.AIR);
                }

                index++;
            }
            sim.Dispose();
            Debug.Log($"Pathing took {Time.realtimeSinceStartup-realtimeSinceStartup}s");
        }
    }
}