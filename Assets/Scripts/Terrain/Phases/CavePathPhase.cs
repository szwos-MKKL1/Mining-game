﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using DelaunatorSharp;
using InternalDebug;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Serialization;
using Terrain.Blocks;
using Terrain.PathGraph;
using Terrain.PathGraph.CellularAutomata;
using Terrain.PathGraph.Graphs;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;

namespace Terrain.Phases
{
    [PhaseDependency(typeof(RawPhase), DependencyOrder.Before)]
    public class CavePathPhase : IGenerationPhase
    {
        private readonly GenerationData generationData;
        private UndirectedGraph<Vector2, IEdge<Vector2>> combinedGraph;

        public CavePathPhase(GenerationData generationData, out IEdgeSet<Vector2, IEdge<Vector2>> paths)
        {
            this.generationData = generationData;
            paths = combinedGraph;
        }
        
        public void Generate(TerrainData terrainData)
        {
            //Defining start and end points for path finding, TODO should be dynamic
            Vector2Int startPoint = new Vector2Int(100, 100);
            Vector2Int destinationPoint = new Vector2Int(1000, 1000);

            //Generating random graph on entire map by using poisson-disc distribution approximation and point-set triangulation
            RandomGraph randomGraph = RandomGraph.CreateFromPoints(terrainData.RealSize, 300, 68);
            UndirectedGraph<Vector2, IEdge<Vector2>> pathFinderGraph = randomGraph.GetGraph();
            
            
            pathFinderGraph.UnityDraw(Color.red, 300);
            //Removing vertices that are outside of buildable area
            pathFinderGraph.RemoveVertexIf(pos => !terrainData.IsBuildable(pos.AsVectorInt()));
            pathFinderGraph.UnityDraw(Color.blue, 300);

            //Creating weight map for path finding
            DistanceMap distanceMap = new DistanceMap(terrainData.GetNotBuildableMap(), terrainData.RealSize);
            distanceMap.Generate();

            //TODO add points of interest and path to them
            
            //Searching for 5 random paths in graph
            PathFinder pathFinder = new PathFinder(pathFinderGraph, pos =>
                {
                    ushort dis = distanceMap.GetDistance(pos.AsVectorInt());
                    return dis!=0 ? math.max(generationData.borderWeight-dis, 0) : ushort.MaxValue;
                }, startPoint, destinationPoint,
                terrainData.RealSize, generationData.pathFindingSettings);
            List<IEnumerable<IEdge<Vector2>>> pathList = new();
            for (int i = 0; i < 5; i++)
            {
                IEnumerable<IEdge<Vector2>> path = pathFinder.NextRandomPath();
                Debug.Assert(path!=null, "No path was found");
                pathList.Add(path);
            }
            
            //Combine all edges into one collection
            HashSet<IEdge<Vector2>> allEdges = new();
            foreach (IEnumerable<IEdge<Vector2>> path in pathList)
                allEdges.AddRange(path);
            
            combinedGraph = allEdges.ToUndirectedGraph<Vector2, IEdge<Vector2>>();
            combinedGraph.UnityDraw(Color.green, 100);
            
            //Create points around found paths and connect them to new graph that will be used in cavern generation
            UndirectedGraph<Vector2, IEdge<Vector2>> cavernConnectionGraph = RandomGraph.CreateAroundEdges(combinedGraph.Edges).GetGraph();
            //cavernConnectionGraph.UnityDraw(Color.blue, 300);
            
            //Remove edges that are too long
            cavernConnectionGraph.RemoveEdgeIf(edge => DistanceMethods.ManhattanDistance(edge.Target, edge.Source) > 150);
            //cavernConnectionGraph.UnityDraw(Color.cyan, 300);
            
            
            Layer[] layers = { new(100), new(45) };
            List<GeneratorNode> genNodes = new();
            foreach (Vector2 node in cavernConnectionGraph.Vertices)
            {
                LayerGenerationSettings[] genSettings = {
                    new(10, 0),
                    new(25, 1)
                };
                genNodes.Add(new GeneratorNode(node.AsVectorInt(), genSettings));
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