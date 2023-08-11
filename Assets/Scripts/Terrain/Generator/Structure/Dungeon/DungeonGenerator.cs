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
        private List<DungeonRoom> rooms;
        private Config config;
        public DungeonGenerator(Config config)
        {
            this.config = config;
            Start();
        }

        private void Start()
        {
            rooms = GetInitialDungeonRooms();
            SeparateRooms(rooms, config.RandomPointGenShapes.Bounds());
            rooms.Draw(Color.blue);
            List<DungeonRoom> mainRooms = GetMainRooms(rooms).ToList();
            mainRooms.Draw(Color.red);
            UndirectedGraph<Vector2, IEdge<Vector2>> connections = GetConnectionGraph(mainRooms);
            connections.UnityDraw(Color.cyan, 10f);
            List<DungeonRoom> connectionRooms = FindConnectionRooms(rooms, mainRooms, connections);
            connectionRooms.Draw(Color.green, 10f);
        }
        
        private List<DungeonRoom> GetInitialDungeonRooms()
        {
            IRandomPointGenShape randomPointGenShape = config.RandomPointGenShapes;
            IRandomRoomSize randomRoomSize = config.RandomRoomSize;
            List<DungeonRoom> dungeonRooms = new();
            for (int i = 0; i < config.RoomCount; i++)
            {
                dungeonRooms.Add(new DungeonRoom(new int2(randomPointGenShape.NextPoint()), randomRoomSize.NextRandomSize()));
            }

            return dungeonRooms;
        }
        
        //TODO tmp
        public IEnumerable<DungeonRoom> Rooms()
        {
            return rooms;
        }

        private void SeparateRooms(List<DungeonRoom> locrooms, float2x2 roomBounds)
        {
            float2 center = math.abs(roomBounds.c0-roomBounds.c1)/2f;
            //TODO tmp
            float2 min = roomBounds.c0 - center * 5;
            float2 max = roomBounds.c1 + center * 5;
            AABB2D bounds = new AABB2D(min, max);
            NativeArray<AABB2D> rects = new(locrooms.Count, Allocator.TempJob);
            for (int i = 0; i < locrooms.Count; i++)
            {
                DungeonRoom room = locrooms[i];
                rects[i] = new AABB2D(room.Pos, room.Pos + room.Size);
            }
            AABBSeparatorJob separateRoomsJob = new AABBSeparatorJob(rects, bounds);
            separateRoomsJob.Execute();
            for (int i = 0; i < locrooms.Count; i++)
            {
                AABB2D rect = rects[i];
                locrooms[i].Pos = (int2)rect.min;
                //Size doesn't change
            }
            separateRoomsJob.Dispose();
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
                new DelaunatorGraph(mainRooms.Select(room => new float2(room.Center)))
                    .GetEdges()
                .ToUndirectedGraph<Vector2, IEdge<Vector2>>();
            
            return graph.MinimumSpanningTreePrim(edge => DistanceMethods.SqEuclidianDistance(edge.Source, edge.Target))
                .ToUndirectedGraph<Vector2, IEdge<Vector2>>();
        }
        
        private List<DungeonRoom> FindConnectionRooms(
            IEnumerable<DungeonRoom> separatedRooms, 
            IEnumerable<DungeonRoom> mainRooms, //TODO could be replaced with vertices of connectionGraph but it's easier for now to do this
            IEdgeSet<Vector2, IEdge<Vector2>> connectionGraph)
        {
            HashSet<DungeonRoom> connectionRooms = new();
            List<DungeonRoom> toProcess = new(separatedRooms);
            foreach (IEdge<Vector2> edge in connectionGraph.Edges)
            {
                //Iterating every room in room list to find which rooms intersect with line
                //TODO this could be made faster by using quad tree collection
                HashSet<DungeonRoom> toRemove = new();
                foreach (var room in toProcess.Where(room => room.Intersects(new int2(edge.Source), new int2(edge.Target))))
                {
                    connectionRooms.Add(room);
                    toRemove.Add(room);
                }

                toProcess.RemoveAll(room => toRemove.Contains(room));
            }

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

            public Config(int roomCount, IRandomRoomSize randomRoomSize, IRandomPointGenShape randomPointGenShapes)
            {
                RoomCount = roomCount;
                RandomRoomSize = randomRoomSize;
                RandomPointGenShapes = randomPointGenShapes;
            }
        }

        public class DungeonRoom
        {
            public DungeonRoom(int2 pos, int2 size)
            {
                Pos = pos;
                Size = size;
            }

            public int2 Pos { get; set; }

            public int2 Size { get; set; }

            public int Area => Size.x * Size.y;

            public int2 Center => Pos + Size / 2;

            public bool Intersects(DungeonRoom other)
            {
                return Pos.x < other.Pos.x + other.Size.x &&
                       Pos.x + other.Size.x > other.Pos.x &&
                       Pos.y < other.Pos.y + other.Size.y &&
                       Pos.y + other.Size.y > other.Pos.y;
            }
            
            public bool Intersects(int2 lineStart, int2 lineEnd) //TODO there is a bug that causes wrong rooms to be selected
            {
                return WallLines.Any(line => LineIntersects(line.Start, line.End, lineStart, lineEnd));
            }

            /// <summary>
            /// A and B are points that define line 0, C and D define line 1
            /// </summary>
            private static bool LineIntersects(int2 a, int2 b, int2 c, int2 d)
            {
                //a x00 y00
                //b x01 y01
                //c x10 y10
                //d x11 y11
                float rdet = 1f/(d.x * b.y - b.x * d.y);
                //int det = x11 * y01 - x01 * y11;
                float s = rdet * ((a.x - c.x) * b.y - (a.y - c.y) * b.x);
                //s = (1/d)  ((x00 - x10) y01 - (y00 - y10) x01)
                float t = rdet * -(-(a.x - c.x) * d.y + (a.y - c.y) * d.x);
                //t = (1/d) -(-(x00 - x10) y11 + (y00 - y10) x11)
                return s is >= 0 and <= 1 && t is >= 0 and <= 1;
            }

            public DungeonWallLine[] WallLines => new DungeonWallLine[]
            {
                new(Pos, Pos + new int2(Size.x, 0)),
                new(Pos, Pos + new int2(0, Size.y)),
                new(Pos + new int2(0, Size.y), Pos + Size),
                new(Pos + new int2(Size.x, 0), Pos + Size)
            };

            public struct DungeonWallLine
            {
                public readonly int2 Start;
                public readonly int2 End;

                public DungeonWallLine(int2 start, int2 end)
                {
                    Start = start;
                    End = end;
                }
            }
        }
    }

    
}