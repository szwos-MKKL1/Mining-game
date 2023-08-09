using System;
using System.Collections.Generic;
using System.Linq;
using InternalDebug;
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
            SeparateRooms(rooms);
            rooms.Draw(Color.blue);
            List<DungeonRoom> mainRooms = GetMainRooms(rooms).ToList();
            mainRooms.Draw(Color.red);
            UndirectedGraph<Vector2, IEdge<Vector2>> connections = GetConnectionGraph(mainRooms);
            connections.UnityDraw(Color.cyan, 10f);
            List<DungeonRoom> connectionRooms = FindConnectionRooms(rooms, connections);
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

        private void SeparateRooms(List<DungeonRoom> locrooms)
        {
            SeparateRoomsJob separateRoomsJob = new SeparateRoomsJob(locrooms);
            separateRoomsJob.Schedule().Complete();
            separateRoomsJob.ApplyResult(locrooms);
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
        
        private List<DungeonRoom> FindConnectionRooms(IEnumerable<DungeonRoom> separatedRooms, IEdgeSet<Vector2, IEdge<Vector2>> connectionGraph)
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
                float t = rdet * ((a.x - c.x) * d.y + (a.y - c.y) * d.x);
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

    [BurstCompile]
    internal struct SeparateRoomsJob : IJob, IDisposable
    {
        private NativeArray<JobDungeonRoom> rooms;
        [ReadOnly]
        private int count;
        public SeparateRoomsJob(IReadOnlyList<DungeonGenerator.DungeonRoom> generatorRooms)
        {
            count = generatorRooms.Count;
            rooms = new NativeArray<JobDungeonRoom>(count, Allocator.TempJob);
            for (int i = 0; i < count; i++)
            {
                DungeonGenerator.DungeonRoom dungeonRoom = generatorRooms[i];
                rooms[i] = new JobDungeonRoom(new float2(dungeonRoom.Pos), dungeonRoom.Size);
            }
        }
        //TODO this algorithm uses n^2 checks each tick. Replace it with quad tree or something similar
        public unsafe void Execute()
        {
            JobDungeonRoom* roomArrayPtr = (JobDungeonRoom*)rooms.GetUnsafePtr();
            bool ok = false;
            int separationTicks = 0;
            while (!ok && separationTicks < 2 * count)
            {
                ok = true;
                for (int i = 0; i < count; i++)
                {
                    JobDungeonRoom current = rooms[i];

                    float2 movement = float2.zero;
                    int separationCount = 0;
                    for (int j = 0; j < count; j++)
                    {
                        if (i == j) continue;
                        JobDungeonRoom other = rooms[j];
                        if (!current.Intersects(other)) continue;
                        
                        movement += other.Center - current.Center;
                        ++separationCount;
                    }
                    
                    if (separationCount > 0)
                    {
                        movement *= -1;
                        movement = math.normalize(movement);
                        float2 newPos = current.Pos;
                        newPos += movement;
                        if (!newPos.Equals(current.Pos))
                        {
                            (roomArrayPtr + i)->Pos = newPos;
                            ok = false;
                        }
                    }
                }
                separationTicks++;
            }
        }

        public bool ApplyResult(List<DungeonGenerator.DungeonRoom> dungeonRooms)
        {
            for (int i = 0; i < rooms.Length; i++)
            {
                dungeonRooms[i].Pos = new int2(rooms[i].Pos);
            }
            
            return true;
        }
        
        private static void DrawLineScaled(Vector3 a, Vector3 b)
        {
            Debug.DrawLine(a * 0.16f, b * 0.16f, Color.magenta, 60f, false);
        }

        //TODO remove repeating code
        //copied DungeonRoom but Pos is float2, 
        private struct JobDungeonRoom
        {
            public float2 Pos;
            public int2 Size { get; }

            public JobDungeonRoom(float2 pos, int2 size)
            {
                Pos = pos;
                Size = size;
            }
            
            public float2 Center => Pos + Size / 2;
            
            public bool Intersects(JobDungeonRoom other)
            {
                return Pos.x + Size.x >= other.Pos.x && other.Pos.x + other.Size.x >= Pos.x &&
                       Pos.y + Size.y >= other.Pos.y && other.Pos.y + other.Size.y >= Pos.y;
            }
            
        }

        public void Dispose()
        {
            rooms.Dispose();
        }
    }
}