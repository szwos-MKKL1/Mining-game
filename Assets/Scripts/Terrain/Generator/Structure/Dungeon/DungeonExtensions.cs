using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.Generator.Structure.Dungeon
{
    public static class DungeonExtensions
    {
        public static void Draw(this IEnumerable<DungeonRoom> dungeonRooms, Color color, float duration = 10f)
        {
            foreach (var room in dungeonRooms)
            {
                Vector2 min = room.Rect.min;
                Vector2 max = room.Rect.max;
                DrawLineScaled(min, new Vector2(max.x, min.y), color, duration);
                DrawLineScaled(min, new Vector2(min.x, max.y), color, duration);
                DrawLineScaled(new Vector2(min.x, max.y), max, color, duration);
                DrawLineScaled(new Vector2(max.x, min.y), max, color, duration);
            }
        }
        
        private static void DrawLineScaled(Vector3 a, Vector3 b, Color color, float duration = 10f)
        {
            Debug.DrawLine(a * 0.16f, b * 0.16f, color, duration, false);
        }
    }
}