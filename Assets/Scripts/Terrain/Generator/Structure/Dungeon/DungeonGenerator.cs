using System;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using InternalDebug;
using NativeTrees;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Serialization;
using Random;
using Terrain.Generator.PathGraph.Graphs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

namespace Terrain.Generator.Structure.Dungeon
{
    //This generator will implement algorithm that was used in TinyKeep game
    //https://www.reddit.com/r/gamedev/comments/1dlwc4/procedural_dungeon_generation_algorithm_explained/
    
    //TODO find some way to add pre-made rooms to generation
    
    public class DungeonGenerator : IDisposable
    {
        private DungeonRoomTree<DungeonRoom> rooms;//TODO dispose
        private Config config;
        private IRandom random;
        public DungeonGenerator(Config config, IRandom random)
        {
            this.config = config;
            this.random = random;
            Start();
        }

        private void Start()
        {
            //TODO better names for methods and varibles
            
            //Creating boxes in circle to be separated
            rooms = GetInitialDungeonRooms();
            //Separates rooms //TODO kinda slow
            SeparateRooms(rooms);
            //Chooses which rooms are considered to be most important (based on area)
            List<DungeonRoom> mainRooms = GetMainRooms(rooms).ToList();
            //Creates graph that shows how main rooms are connected
            UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> mainRoomsPath = GetMainRoomPathGraph(mainRooms);
            //Takes main room connection graph and makes lines straight (this step could be omitted)
            UndirectedGraph<Vector2, IEdge<Vector2>> mainRoomHallwayGraph = GetStraightHallways(mainRoomsPath);
            //Finds rooms that intersect with hallway graph edges, those rooms will be used to walk between main rooms
            List<DungeonRoom> standardRooms = GetConnectionRooms(rooms, mainRoomHallwayGraph);
            standardRooms.Draw(Color.green, 10f);//TODO remove debug
            //Similar to GetMainRoomPathGraph, this graph will show how to connect standard rooms with hallways
            UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> standardRoomPathGraph = GetStandardRoomPathGraph(standardRooms);
            //Creates straight hallways
            UndirectedGraph<Vector2, IEdge<Vector2>> finalHallwayGraph = GetStraightHallways(standardRoomPathGraph);
            finalHallwayGraph.UnityDraw(Color.magenta, 10f);
            GeneratePlacement(rooms, standardRooms, finalHallwayGraph);
        }
        private DungeonRoomTree<DungeonRoom> GetInitialDungeonRooms()
        {
            IRandomPointGenShape randomPointGenShape = config.RandomPointGenShapes;
            IRandomRoomSize randomRoomSize = config.RandomRoomSize;
            DungeonRoomTree<DungeonRoom> dungeonRoomTree = new(config.Bounds,Allocator.Persistent);
            for (int i = 0; i < config.RoomCount; i++)
            {
                DungeonRoom room = new DungeonRoom(new int2(randomPointGenShape.NextPoint()),
                    randomRoomSize.NextRandomSize());
                dungeonRoomTree.Rooms.Add(room);
                dungeonRoomTree.Tree.Insert(i, room.Rect);
            }

            return dungeonRoomTree;
        }

        private void SeparateRooms(DungeonRoomTree<DungeonRoom> dungeonRoomTree)
        {
            AABBSeparatorJob<DungeonRoom> separateRoomsJob = new(dungeonRoomTree);
            separateRoomsJob.Schedule().Complete();
        }

        private IEnumerable<DungeonRoom> GetMainRooms(IEnumerable<DungeonRoom> separatedRooms)
        {
            List<DungeonRoom> sorted = new(separatedRooms);
            sorted.Sort((a,b) => a.Area.CompareTo(b.Area));
            int mainRoomCount = 15;//TODO move to dungeon generator config
            return sorted.Skip(sorted.Count - mainRoomCount);
        }

        private UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> Delanuay(IEnumerable<DungeonRoom> roomList)
        {
            return new DelaunatorGraph<DungeonRoom>(roomList, room =>
                {
                    float2 center = room.Rect.Center;
                    return (IPoint)new Point(center.x, center.y);
                })
                .GetEdges()
                .ToUndirectedGraph<DungeonRoom, IEdge<DungeonRoom>>();
        }
        
        private UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> GetMainRoomPathGraph(IEnumerable<DungeonRoom> roomList)
        {
            //This method could be implemented with many different graph such as minimum spanning tree, gabriel graph
            UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> graph = Delanuay(roomList);
            List<IEdge<DungeonRoom>> minEdges = graph.MinimumSpanningTreePrim(edge =>
                DistanceMethods.SqEuclidianDistance(edge.Source.Rect.min, edge.Target.Rect.min)).ToList();
            List<IEdge<DungeonRoom>> graphEdges = graph.Edges.ToList();
            for (int i = 0; i < 5; i++)
            {
                minEdges.Add(graphEdges[random.NextInt(0, graphEdges.Count - 1)]);
            }
            return minEdges.ToUndirectedGraph<DungeonRoom, IEdge<DungeonRoom>>();
        }
        
        private UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> GetStandardRoomPathGraph(IEnumerable<DungeonRoom> connectionRooms)
        {
            UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> graph = Delanuay(connectionRooms);

            List<IEdge<DungeonRoom>> minEdges = graph.MinimumSpanningTreePrim(edge =>
                DistanceMethods.ManhattanDistance(edge.Source.Rect.min, edge.Target.Rect.min)).ToList();
            return minEdges.ToUndirectedGraph<DungeonRoom, IEdge<DungeonRoom>>();
        }

        private UndirectedGraph<Vector2, IEdge<Vector2>> GetStraightHallways(IEdgeSet<DungeonRoom, IEdge<DungeonRoom>> connectionGraph)
        {
            UndirectedGraph<Vector2, IEdge<Vector2>> corridorGraph = new();
            foreach (IEdge<DungeonRoom> edge in connectionGraph.Edges)
            {
                bool useX = false;
                bool useY = false;
                AABB2D sourceRect = edge.Source.Rect;
                AABB2D targetRect = edge.Target.Rect;
                float2 sourceCenter = sourceRect.Center;
                float2 targetCenter = targetRect.Center;
                float2 center = (sourceCenter + targetCenter) / 2f;
                
                if (center.x >= sourceRect.min.x && center.x <= sourceRect.max.x &&
                    center.x >= targetRect.min.x && center.x <= targetRect.max.x) 
                    useX = true;
                
                if (center.y >= sourceRect.min.y && center.y <= sourceRect.max.y &&
                    center.y >= targetRect.min.y && center.y <= targetRect.max.y) 
                    useY = true;

                if (useX && useY)
                {
                    if (random.NextInt(0, 1) == 0)
                        useX = false;
                    else useY = false;
                }

                if (useX)
                    corridorGraph.AddVerticesAndEdge(new Edge<Vector2>(new Vector2(center.x, sourceCenter.y), new Vector2(center.x, targetCenter.y)));
                else if(useY)
                    corridorGraph.AddVerticesAndEdge(new Edge<Vector2>(new Vector2(sourceCenter.x, center.y), new Vector2(targetCenter.x, center.y)));
                else
                {
                    corridorGraph.AddVerticesAndEdge(
                        new Edge<Vector2>(
                            new Vector2(sourceCenter.x, sourceCenter.y),
                            new Vector2(sourceCenter.x, targetCenter.y)
                        ));
                    corridorGraph.AddVerticesAndEdge(
                        new Edge<Vector2>(
                            new Vector2(sourceCenter.x, targetCenter.y),
                            new Vector2(targetCenter.x, targetCenter.y)
                        ));
                }
            }

            return corridorGraph;
        }

        private List<DungeonRoom> GetConnectionRooms(
            DungeonRoomTree<DungeonRoom> dungeonRoomTree,
            IEdgeSet<Vector2, IEdge<Vector2>> corridorGraph)
        {
            NativeParallelHashSet<int> roomIds = new(dungeonRoomTree.Rooms.Length, Allocator.Temp);
            const float tolerance = 0.05f;
            const float radius = 3f;
            foreach (IEdge<Vector2> edge in corridorGraph.Edges)
            {
                //TODO repetitive code
                //Making bounding boxes around vertical and horizontal lines
                //defaults to vertical
                float2 min;
                float2 max;
                if (Math.Abs(edge.Source.x - edge.Target.x) < tolerance)
                {
                    //X is equal
                    if (edge.Source.y < edge.Target.y)
                    {
                        min = new float2(edge.Source.x - radius, edge.Source.y - radius);
                        max = new float2(edge.Source.x + radius, edge.Target.y + radius);
                    }
                    else 
                    {
                        min = new float2(edge.Source.x - radius, edge.Target.y - radius);
                        max = new float2(edge.Source.x + radius, edge.Source.y + radius);
                    }
                }
                else
                {
                    //Y is equal
                    if (edge.Source.x < edge.Target.x)
                    {
                        min = new float2(edge.Source.x + radius, edge.Source.y - radius);
                        max = new float2(edge.Target.x - radius, edge.Source.y + radius);
                    }
                    else
                    {
                        min = new float2(edge.Target.x + radius, edge.Source.y - radius);
                        max = new float2(edge.Source.x - radius, edge.Source.y + radius);
                    }
                }
                AABB2D edgeBox = new AABB2D(min, max);
                dungeonRoomTree.Tree.RangeAABBUnique(edgeBox, roomIds);
            }

            
            List<DungeonRoom> connectionRooms = new();
            using (NativeParallelHashSet<int>.Enumerator enumerator = roomIds.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    connectionRooms.Add(dungeonRoomTree.Rooms[enumerator.Current]);
                }
            }
            roomIds.Dispose();
            return connectionRooms;
        }
        
        private void GeneratePlacement(DungeonRoomTree<DungeonRoom> dungeonRoomTree, List<DungeonRoom> standardRooms, UndirectedGraph<Vector2, IEdge<Vector2>> finalHallwayGraph)
        {
            
        }
        
        //TODO return dungeon structure maybe by rooms and corridors
        //well we may also need some way to return caves generated inside dungeon


        public class Config
        {
            public readonly int RoomCount;
            public IRandomRoomSize RandomRoomSize;
            public IRandomPointGenShape RandomPointGenShapes;
            public AABB2D Bounds;//TODO checks for setting

            public Config(int roomCount, IRandomRoomSize randomRoomSize, IRandomPointGenShape randomPointGenShapes)
            {
                RoomCount = roomCount;
                RandomRoomSize = randomRoomSize;
                RandomPointGenShapes = randomPointGenShapes;
            }
        }

        public enum DungeonBlockTypes
        {
            MAIN_ROOM_WALL,
            STANDARDROOM_WALL,
            HALLWAY_WALL,
        }

        public void Dispose()
        {
            rooms?.Dispose();
        }
    }

    
}