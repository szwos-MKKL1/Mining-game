using System.Collections.Generic;
using InternalDebug;
using QuikGraph;
using Random;
using Terrain.Blocks;
using Terrain.Generator.PathGraph;
using Terrain.Generator.PathGraph.CellularAutomata;
using Terrain.Generator.PathGraph.Graphs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

namespace Terrain.Generator.Phases
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
            cavernConnectionGraph.RemoveEdgeIf(edge => DistanceMethods.ManhattanDistance(edge.Target, edge.Source) > 50);
            //cavernConnectionGraph.UnityDraw(Color.cyan, 300);
            
            #region cellular_automata
            Layer[] layers = { new(100), new(70), new(45) };
            List<GeneratorNode> genNodes = new();
            IRandom random = new SystemRandom(0);//TODO replace with random from terrain generator
            foreach (Vector2 node in cavernConnectionGraph.Vertices)
            {
                LayerGenerationSettings[] genSettings = {
                    new((short)random.NextInt(4, 10), 0),
                    new((short)random.NextInt(14, 20), 2)
                };
                genNodes.Add(new GeneratorNode(node.AsVectorInt(), genSettings));
            }
            
            //TODO maze
            List<Line> lines = new List<Line>();
            foreach (IEdge<Vector2> edge in cavernConnectionGraph.Edges)
            {
                // lines.Add(new Line
                // {
                //     SourcePos = new int2(edge.Source),
                //     TargetPos = new int2(edge.Target),
                //     Width = 2,
                //     LayerID = 0
                // });
                lines.Add(new Line
                {
                    SourcePos = new int2(edge.Source),
                    TargetPos = new int2(edge.Target),
                    Width = 4,
                    LayerID = 1
                });
            }
            LinePathGen linePathGen = new LinePathGen(lines.ToArray());
            InitialMapGenerator initialMapGenerator = new InitialMapGenerator(terrainData.RealSize, layers, new ILayerGenerator[]
            {
                linePathGen,
                new CircleAroundNodeGen(genNodes)
            });
            bool[] initial = initialMapGenerator.GetInitialMap();
            linePathGen.Dispose();
            
            var sim = CellularAutomataSimulator.CreateFromMap(terrainData.RealSize, initial);
            sim.AliveThreshold = 5;
            for (int i = 0; i < 11; i++)
            {
                sim.ExecuteStep();
            }
            #endregion

            
            RoomFinder roomFinder = new RoomFinder(sim.CellMap, terrainData.RealSize);
            List<Room> roomList = roomFinder.GetRoomList();
            Vector2Int realsize = terrainData.RealSize;
            bool[] finalMap = new bool[realsize.x * realsize.y];
            foreach (var nativeRoom in roomList)
            {
                if (nativeRoom.Size > 40)
                {
                    foreach (var alivePos in nativeRoom)
                    {
                        finalMap[alivePos] = true;
                        terrainData.SetBlock(new Vector2Int(alivePos % realsize.x, alivePos / realsize.y), BlockRegistry.AIR);
                    }
                }
            }
            sim.Dispose();
        }
    }
}