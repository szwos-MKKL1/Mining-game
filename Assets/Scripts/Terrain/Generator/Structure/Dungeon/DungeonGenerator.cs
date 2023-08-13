using System;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using InternalDebug;
using NativeTrees;
using QuikGraph;
using QuikGraph.Algorithms;
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
        public DungeonGenerator(Config config)
        {
            this.config = config;
            Start();
        }

        private void Start()
        {
            rooms = GetInitialDungeonRooms();
            SeparateRooms(rooms);
            rooms.Draw(Color.blue);
            List<DungeonRoom> mainRooms = GetMainRooms(rooms).ToList();
            mainRooms.Draw(Color.red);
            UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> connections = GetConnectionGraph(mainRooms);
            connections.Edges.Select(roomEdge => 
                new QuikGraph.Edge<Vector2>(roomEdge.Source.Rect.Center, roomEdge.Target.Rect.Center))
                .UnityDraw(Color.cyan, 10f);
            // List<DungeonRoom> connectionRooms = FindConnectionRooms(rooms, mainRooms, connections);
            // connectionRooms.Draw(Color.green, 10f);
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
        
        //TODO tmp
        public IEnumerable<DungeonRoom> Rooms()
        {
            return rooms;
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
        
        private List<DungeonRoom> FindConnectionRooms(
            IEnumerable<DungeonRoom> separatedRooms, 
            IEnumerable<DungeonRoom> mainRooms, //TODO could be replaced with vertices of connectionGraph but it's easier for now to do this
            IEdgeSet<Vector2, IEdge<Vector2>> connectionGraph)
        {
            HashSet<DungeonRoom> connectionRooms = new();
            
        
            connectionRooms.RemoveWhere(mainRooms.Contains); //Removes main rooms from list of connection rooms
            return connectionRooms.ToList();
        }
        
        private void MakeCorridors(IEnumerable<DungeonRoom> separatedRooms, IEnumerable<DungeonRoom> connectionRooms)
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