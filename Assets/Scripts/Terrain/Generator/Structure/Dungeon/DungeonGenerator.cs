using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public class DungeonGenerator
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
            UndirectedGraph<Vector2, IEdge<Vector2>> connections = GetConnectionGraph(mainRooms);
            connections.UnityDraw(Color.cyan, 10f);
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
        
        //TODO should return UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>>
        private UndirectedGraph<Vector2, IEdge<Vector2>> GetConnectionGraph(IEnumerable<DungeonRoom> mainRooms)
        {
            //This method could be implemented with many different graph such as minimum spanning tree, gabriel graph
            UndirectedGraph<Vector2, IEdge<Vector2>> graph = 
                new DelaunatorGraph(mainRooms.Select(room => new float2(room.Rect.Center)))
                    .GetEdges()
                .ToUndirectedGraph<Vector2, IEdge<Vector2>>();
            
            return graph.MinimumSpanningTreePrim(edge => DistanceMethods.SqEuclidianDistance(edge.Source, edge.Target))
                .ToUndirectedGraph<Vector2, IEdge<Vector2>>();
        }
        
        // private List<DungeonRoom> FindConnectionRooms(
        //     IEnumerable<DungeonRoom> separatedRooms, 
        //     IEnumerable<DungeonRoom> mainRooms, //TODO could be replaced with vertices of connectionGraph but it's easier for now to do this
        //     IEdgeSet<Vector2, IEdge<Vector2>> connectionGraph)
        // {
        //     HashSet<DungeonRoom> connectionRooms = new();
        //     List<DungeonRoom> toProcess = new(separatedRooms);
        //     foreach (IEdge<Vector2> edge in connectionGraph.Edges)
        //     {
        //         //Iterating every room in room list to find which rooms intersect with line
        //         //TODO this could be made faster by using quad tree collection
        //         HashSet<DungeonRoom> toRemove = new();
        //         foreach (var room in toProcess.Where(room => room.Intersects(new int2(edge.Source), new int2(edge.Target))))
        //         {
        //             connectionRooms.Add(room);
        //             toRemove.Add(room);
        //         }
        //
        //         toProcess.RemoveAll(room => toRemove.Contains(room));
        //     }
        //
        //     connectionRooms.RemoveWhere(mainRooms.Contains); //Removes main rooms from list of connection rooms
        //     return connectionRooms.ToList();
        // }
        
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
    }

    
}