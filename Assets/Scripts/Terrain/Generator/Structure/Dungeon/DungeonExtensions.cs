using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Generator.Structure.Dungeon
{
    public static class DungeonExtensions
    {
        public static void Draw(this IEnumerable<DungeonGenerator.DungeonRoom> dungeonRooms, Color color, float duration = 10f)
        {
            foreach (var room in dungeonRooms)
            {
                Vector3 pos0 = new Vector3(room.Pos.x, room.Pos.y);
                Vector3 size = new Vector3(room.Size.x, room.Size.y);
                DrawLineScaled(pos0, pos0 + new Vector3(size.x, 0), color, duration);
                DrawLineScaled(pos0, pos0 + new Vector3(0, size.y), color, duration);
                DrawLineScaled(pos0 + new Vector3(0, size.y), pos0 + new Vector3(size.x, size.y), color, duration);
                DrawLineScaled(pos0 + new Vector3(size.x, 0), pos0 + new Vector3(size.x, size.y), color, duration);
            }
        }
        
        private static void DrawLineScaled(Vector3 a, Vector3 b, Color color, float duration = 10f)
        {
            Debug.DrawLine(a * 0.16f, b * 0.16f, color, duration, false);
        }
    }
}