using NativeTrees;
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
    }
}