using System.Collections.Generic;
using QuikGraph;
using Unity.Jobs;
using Unity.Mathematics;
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
        private static int2 Normalize(int2 vec)
        {
            float mag = math.sqrt(vec.x * vec.x + vec.y * vec.y);
            return new int2((int)(vec.x / mag), (int)(vec.y / mag));
        }

        public IEnumerable<DungeonRoom> Rooms()
        {
            return rooms;
        }
        //end of tmp

        private void SeparateRooms(ICollection<DungeonRoom> locrooms)
        {
            bool regionsOk = false;
            int separationTicks = 0;
            while (!regionsOk && separationTicks < 2 * locrooms.Count)
            {
                regionsOk = true;
                foreach (DungeonRoom room in locrooms)
                {

                    int2 movement = int2.zero;
                    int separationCount = 0;

                    foreach (DungeonRoom other in locrooms)
                    {
                        if (room == other)
                            continue;

                        if (!room.Intersects(other))
                            continue;

                        movement += other.Center - room.Center;
                        ++separationCount;
                    }

                    if (separationCount > 0)
                    {
                        movement *= -1;
                        movement = Normalize(movement);
                        int2 newPos = room.Pos;
                        newPos += movement;

                        if (!newPos.Equals(room.Pos))
                        {
                            room.Pos = newPos;
                            regionsOk = false;
                        }
                    }
                }

                ++separationTicks;
            }
        }

        private IEnumerable<DungeonRoom> GetMainRooms(IEnumerable<DungeonRoom> separatedRooms)
        {
            return null;
        }
        
        private UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> GetConnectionGraph(IEnumerable<DungeonRoom> mainRooms)
        {
            //This method could be implemented with many different graph such as minimum spanning tree, gabriel graph
            return null;
        }
        
        private IEnumerable<DungeonRoom> FindConnectionRooms(IEnumerable<DungeonRoom> separatedRooms, UndirectedGraph<DungeonRoom, IEdge<DungeonRoom>> connectionGraph)
        {
            return null;
        }
        
        private void FindConnectionRooms(IEnumerable<DungeonRoom> separatedRooms, IEnumerable<DungeonRoom> connectionRooms)
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
        }
    }


    internal struct SeparateRoomsJob : IJob
    {
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}