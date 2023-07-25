using System;
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
using Edge = DelaunatorSharp.Edge;
using Random = System.Random;

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
            
            
            //pathFinderGraph.UnityDraw(Color.red, 300);
            //Removing vertices that are outside of buildable area
            pathFinderGraph.RemoveVertexIf(pos => !terrainData.IsBuildable(pos.AsVectorInt()));
            //pathFinderGraph.UnityDraw(Color.blue, 300);

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
            //float g = 0.0f;
            for (int i = 0; i < 5; i++)
            {
                IEnumerable<IEdge<Vector2>> path = pathFinder.NextRandomPath();
                Debug.Assert(path!=null, "No path was found");
                path.UnityDraw(new Color(0, 255, 0), 100);
                pathList.Add(path);
            }
            
            //Combine all edges into one collection
            HashSet<IEdge<Vector2>> allEdges = new();
            foreach (IEnumerable<IEdge<Vector2>> path in pathList)
                allEdges.AddRange(path);
            
            combinedGraph = allEdges.ToUndirectedGraph<Vector2, IEdge<Vector2>>();
            //combinedGraph.UnityDraw(Color.green, 100);
            
            //Create points around found paths and connect them to new graph that will be used in cavern generation
            UndirectedGraph<Vector2, IEdge<Vector2>> cavernConnectionGraph = RandomGraph.CreateAroundEdges(combinedGraph.Edges).GetGraph();
            //cavernConnectionGraph.UnityDraw(Color.blue, 300);
            
            //Remove edges that are too long
            cavernConnectionGraph.RemoveEdgeIf(edge => DistanceMethods.ManhattanDistance(edge.Target, edge.Source) > 100);
            //cavernConnectionGraph.UnityDraw(Color.cyan, 300);
            
            
            Layer[] layers = { new(100), new(45) };
            List<GeneratorNode> genNodes = new();
            Random random = new Random(0);
            foreach (Vector2 node in cavernConnectionGraph.Vertices)
            {
                LayerGenerationSettings[] genSettings = {
                    new((short)random.Next(8, 15), 0),
                    new((short)random.Next(18, 30), 1)
                };
                genNodes.Add(new GeneratorNode(node.AsVectorInt(), genSettings));
            }
            
            //TODO maze
            
            Line[] lines = cavernConnectionGraph.Edges.Select(edge => 
                new Line { SourcePos = new int2(edge.Source), 
                    TargetPos = new int2(edge.Target), 
                    Width = 4, 
                    layerID = 0 }).ToArray();
            LinePathGen linePathGen = new LinePathGen(lines);
            InitialMapGenerator initialMapGenerator = new InitialMapGenerator(terrainData.RealSize, layers, new ILayerGenerator[]
            {
                new CircleAroundNodeGen(genNodes),
                linePathGen
            });
            bool[] initial = initialMapGenerator.GetInitialMap();
            linePathGen.Dispose();
            
            var sim = CellularAutomataSimulator.CreateFromMap(terrainData.RealSize, initial);
            ImageDebug.SaveImg(sim.CellMap.ToArray(), terrainData.RealSize, "step"+0+".png");
            sim.AliveThreshold = 5;
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

            RoomFinder roomFinder = new RoomFinder(sim.CellMap, terrainData.RealSize);
            List<Room> roomList = roomFinder.GetRoomList();
            Vector2Int realsize = terrainData.RealSize;
            foreach (var nativeRoom in roomList)
            {
                if (nativeRoom.Size > 40)
                {
                    foreach (var alivePos in nativeRoom)
                    {
                        terrainData.SetBlock(new Vector2Int(alivePos % realsize.x, alivePos / realsize.y), BlockRegistry.AIR);
                    }
                }
            }
            sim.Dispose();
        }
    }
}