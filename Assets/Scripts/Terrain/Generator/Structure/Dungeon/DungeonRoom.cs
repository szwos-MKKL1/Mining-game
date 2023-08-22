using NativeTrees;
using Terrain.Outputs;
using Unity.Mathematics;

namespace Terrain.Generator.Structure.Dungeon
{
    public struct DungeonRoom : IDungeonRoom
    {
        public AABB2D Rect { get; set; }

        public DungeonRoomType RoomType { get; set; }
        public float Area
        {
            get
            {
                float2 d = Rect.max - Rect.min;
                return d.x * d.y;
            }
        }

        public DungeonRoom(int2 pos, int2 size)
        {
            Rect = new AABB2D(pos, pos + size);
            RoomType = DungeonRoomType.NONE;
        }

        public enum DungeonRoomType : byte
        {
            NONE,
            STANDARD,
            MAIN
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
            NONE = 0,
            AIR,
            MAIN_ROOM_WALL,
            STANDARD_ROOM_WALL,
            HALLWAY_WALL,
        }

        public enum DungeonSpaceType : byte
        {
            NONE = 0,
            STANDARD,
            MAIN,
            HALLWAY
        }
    }
}