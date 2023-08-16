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
            rooms = GetInitialDungeonRooms();
            SeparateRooms(rooms);
            rooms.Draw(Color.blue);
            List<DungeonRoom> mainRooms = GetMainRooms(rooms).ToList();
            //mainRooms.Draw(Color.red);
            UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> connections = GetConnectionGraph(mainRooms);
            // connections.Edges.Select(roomEdge => 
            //     new QuikGraph.Edge<Vector2>(roomEdge.Source.Rect.Center, roomEdge.Target.Rect.Center))
            //     .UnityDraw(Color.cyan, 10f);
            UndirectedGraph<Vector2, IEdge<Vector2>> corridorGraph = MakeCorridorGraph(connections);
            corridorGraph.UnityDraw(Color.magenta, 10f);
            List<DungeonRoom> connectionRooms = FindConnectionRooms(rooms, mainRooms, corridorGraph);
            connectionRooms.Draw(Color.green, 10f);
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
            int mainRoomCount = 10;//TODO move to dungeon generator config
            return sorted.Skip(sorted.Count - mainRoomCount);
        }
        
        private UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> GetConnectionGraph(IEnumerable<DungeonRoom> mainRooms)
        {
            //This method could be implemented with many different graph such as minimum spanning tree, gabriel graph
            UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> graph = 
                new DelaunatorGraph<DungeonRoom>(mainRooms, room =>
                    {
                        float2 center = room.Rect.Center;
                        return (IPoint)new Point(center.x, center.y);
                    })
                    .GetEdges()
                .ToUndirectedGraph<DungeonRoom, IEdge<DungeonRoom>>();
            
            return graph.MinimumSpanningTreePrim(edge => DistanceMethods.SqEuclidianDistance(edge.Source.Rect.min, edge.Target.Rect.min))
                .ToUndirectedGraph<DungeonRoom, IEdge<DungeonRoom>>();
        }

        private UndirectedGraph<Vector2, IEdge<Vector2>> MakeCorridorGraph(IEdgeSet<DungeonRoom, IEdge<DungeonRoom>> connectionGraph)
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

        private List<DungeonRoom> FindConnectionRooms(
            DungeonRoomTree<DungeonRoom> dungeonRoomTree,
            ICollection<DungeonRoom> mainRooms,
            IEdgeSet<Vector2, IEdge<Vector2>> corridorGraph)
        {
            NativeParallelHashSet<int> roomIds = new(dungeonRoomTree.Rooms.Length, Allocator.Temp);
            const float tolerance = 0.05f;
            const float radius = 3f;
            foreach (IEdge<Vector2> edge in corridorGraph.Edges)
            {
                //TODO repetitive code
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
                    DungeonRoom dungeonRoom = dungeonRoomTree.Rooms[enumerator.Current];
                    //if(!mainRooms.Contains(dungeonRoom))
                        connectionRooms.Add(dungeonRoom);
                }
            }
            roomIds.Dispose();
            return connectionRooms;
        }
        
        private void BuildCorridors(IEnumerable<DungeonRoom> separatedRooms, IEnumerable<DungeonRoom> connectionRooms)
        {
            
            return;
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

        public void Dispose()
        {
            rooms?.Dispose();
        }
    }

    
}