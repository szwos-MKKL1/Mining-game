using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using InternalDebug;
using NativeTrees;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Collections;
using QuikGraph.Serialization;
using Random;
using Terrain.Generator.PathGraph.Graphs;
using Terrain.Outputs;
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
    
    public class DungeonGenerator : IDungeonGenerator, IDisposable
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
            List<DungeonRoom> mainRooms = SelectMainRooms(rooms);
            //Creates graph that shows how main rooms are connected
            UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> mainRoomsPath = GetMainRoomPathGraph(mainRooms);
            //Takes main room connection graph and makes lines straight (this step could be omitted)
            UndirectedGraph<DungeonRoom, DungeonOutput<DungeonRoom>.Hallway> mainRoomHallwayGraph = GetStraightHallways(mainRoomsPath);
            //Finds rooms that intersect with hallway graph edges, those rooms will be used to walk between main rooms
            List<DungeonRoom> standardAndMainRooms = GetConnectionRooms(rooms, mainRoomHallwayGraph);
            standardAndMainRooms.Draw(Color.green, 10f);//TODO remove debug
            
            //TODO move to DungeonRoomTree
            rooms.Rooms.Clear();
            rooms.Tree.Clear();
            AddToTree(rooms, standardAndMainRooms);

            //Similar to GetMainRoomPathGraph, this graph will show how to connect standard rooms with hallways
            UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> standardRoomPathGraph = GetStandardRoomPathGraph(standardAndMainRooms);
            //Creates straight hallways
            UndirectedGraph<DungeonRoom, DungeonOutput<DungeonRoom>.Hallway> finalHallwayGraph = GetStraightHallways(standardRoomPathGraph);
            //finalHallwayGraph.UnityDraw(Color.magenta, 10f); //TODO not working
            //GeneratePlacement(rooms, standardAndMainRooms, finalHallwayGraph);
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

        //Updates values in dungeonRoomTree as well as returns main room list
        private unsafe List<DungeonRoom> SelectMainRooms(DungeonRoomTree<DungeonRoom> dungeonRoomTree)
        {
            List<int> indexes = new();
            int length = dungeonRoomTree.Rooms.Length;
            DungeonRoom* dungeonRooms = (DungeonRoom*)dungeonRoomTree.Rooms.GetUnsafePtr();
            for (int i = 0; i < length; i++)
            {
                indexes.Add(i);
            }
            indexes.Sort((a,b) => dungeonRooms[a].Area.CompareTo(dungeonRooms[b].Area));
            int mainRoomCount = 15;//TODO move to dungeon generator config
            List<DungeonRoom> mainRooms = new(mainRoomCount);
            foreach (var mainRoomIndex in indexes.Skip(indexes.Count - mainRoomCount))
            {
                if(mainRoomIndex < 0 || mainRoomIndex >= length) continue;
                dungeonRooms[mainRoomIndex].RoomType = DungeonRoom.DungeonRoomType.MAIN;
                mainRooms.Add(dungeonRooms[mainRoomIndex]);
            }

            return mainRooms;
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

        private UndirectedGraph<DungeonRoom, DungeonOutput<DungeonRoom>.Hallway> GetStraightHallways(IEdgeSet<DungeonRoom, IEdge<DungeonRoom>> connectionGraph)
        {
            UndirectedGraph<DungeonRoom, DungeonOutput<DungeonRoom>.Hallway> corridorGraph = new();
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

                DungeonOutput<DungeonRoom>.Hallway hallway;

                if (useX)
                    hallway = new DungeonOutput<DungeonRoom>.Hallway(
                        new List<float2>
                        {
                            new(center.x, sourceCenter.y),
                            new(center.x, targetCenter.y)
                        }, edge.Source, edge.Target);
                else if (useY)
                    hallway = new DungeonOutput<DungeonRoom>.Hallway(
                        new List<float2>
                        {
                            new(sourceCenter.x, center.y),
                            new(targetCenter.x, center.y)
                        }, edge.Source, edge.Target);
                else
                {
                    hallway = new DungeonOutput<DungeonRoom>.Hallway(
                        new List<float2>
                        {
                            new(sourceCenter.x, sourceCenter.y),
                            new(sourceCenter.x, targetCenter.y),
                            new(targetCenter.x, targetCenter.y)
                        }, edge.Source, edge.Target);
                }

                corridorGraph.AddVerticesAndEdge(hallway);
            }

            return corridorGraph;
        }

        private unsafe List<DungeonRoom> GetConnectionRooms(
            DungeonRoomTree<DungeonRoom> dungeonRoomTree,
            IEdgeSet<DungeonRoom, DungeonOutput<DungeonRoom>.Hallway> corridorGraph)
        {
            NativeParallelHashSet<int> roomIds = new(dungeonRoomTree.Rooms.Length, Allocator.Temp);
            const float tolerance = 0.05f;
            const float radius = 3f;
            foreach (var edge in corridorGraph.Edges)
            {
                if(!edge.IsValid) continue;
                List<float2> points = edge.Points;
                for (int i = 0; i < points.Count-1; i++)
                {
                    //TODO repetitive code
                    //Making bounding boxes around vertical and horizontal lines
                    //defaults to vertical
                    float2 source = points[i];
                    float2 target = points[i + 1];
                    float2 min;
                    float2 max;
                    if (Math.Abs(source.x - target.x) < tolerance)
                    {
                        //X is equal
                        if (source.y < target.y)
                        {
                            min = new float2(source.x - radius, source.y - radius);
                            max = new float2(source.x + radius, target.y + radius);
                        }
                        else 
                        {
                            min = new float2(source.x - radius, target.y - radius);
                            max = new float2(source.x + radius, source.y + radius);
                        }
                    }
                    else
                    {
                        //Y is equal
                        if (source.x < target.x)
                        {
                            min = new float2(source.x + radius, source.y - radius);
                            max = new float2(target.x - radius, source.y + radius);
                        }
                        else
                        {
                            min = new float2(target.x + radius, source.y - radius);
                            max = new float2(source.x - radius, source.y + radius);
                        }
                    }
                    AABB2D edgeBox = new AABB2D(min, max);
                    dungeonRoomTree.Tree.RangeAABBUnique(edgeBox, roomIds);
                }
            }

            DungeonRoom* dungeonRooms = (DungeonRoom*)dungeonRoomTree.Rooms.GetUnsafePtr();
            List<DungeonRoom> connectionRooms = new();
            using (NativeParallelHashSet<int>.Enumerator enumerator = roomIds.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    DungeonRoom currentDungeonRoom = dungeonRooms[enumerator.Current];
                    if (currentDungeonRoom.RoomType == DungeonRoom.DungeonRoomType.NONE)
                        dungeonRooms[enumerator.Current].RoomType = DungeonRoom.DungeonRoomType.MAIN; //TODO will currentDungeonRoom.RoomType = ... work?
                    connectionRooms.Add(currentDungeonRoom);
                }
            }
            roomIds.Dispose();
            return connectionRooms;
        }

        private void AddToTree(DungeonRoomTree<DungeonRoom> dungeonRoomTree, IEnumerable<DungeonRoom> roomsToAdd)
        {
            foreach (var room in roomsToAdd)
            {
                int i = dungeonRoomTree.Rooms.Length;
                dungeonRoomTree.Rooms.Add(room);
                dungeonRoomTree.Tree.Insert(i, room.Rect);
            }
        }

        private DungeonOutput<DungeonRoom> GenerateOutput(
            DungeonRoomTree<DungeonRoom> dungeonRoomTree,
            UndirectedGraph<DungeonRoom, DungeonOutput<DungeonRoom>.Hallway> hallwayGraph)
        {
            List<DungeonRoom> roomList;
            UndirectedGraph<DungeonRoom, DungeonOutput<DungeonRoom>.Hallway> hallways;
            Dictionary<DungeonRoom, Vector2> roomEntryPoints;

            //TODO implement
            return null;
        }

        public DungeonOutput<DungeonRoom> GeneratorOutput { get; }

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

        public struct DungeonBlock : IPosHolder
        {
            public DungeonBlock(DungeonBlockTypes blockType, int2 pos)
            {
                this.BlockType = blockType;
                this.Pos = pos;
            }

            public int2 Pos { get; }

            public DungeonBlockTypes BlockType { get; }
        }

        public enum DungeonBlockTypes : byte
        {
            MAIN_ROOM_WALL,
            STANDARD_ROOM_WALL,
            HALLWAY_WALL,
        }
        
        public enum DungeonRoomTypes : byte
        {
            MAIN_ROOM,
            STANDARD_ROOM,
            HALLWAY,
        }

        public void Dispose()
        {
            rooms?.Dispose();
        }
    }

    
}