﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void Draw(this IEnumerable<DungeonOutput<DungeonRoom>.Hallway> edges, Color color, float duration)
        {
            foreach (var hallwaysEdge in edges)
            {
                List<float2> points = hallwaysEdge.Points;
                for (int i = 0; i < hallwaysEdge.Points.Count-1; i++)
                {
                    DrawLineScaled(points[i].AsVector(), points[i+1].AsVector(), color, duration);
                }
            } 
        }

        public static void Draw(this DungeonOutput<DungeonRoom> dungeonOutput, float duration = 10f)
        {
            dungeonOutput.Rooms.Draw(Color.green, duration);
            foreach (var room in dungeonOutput.Rooms)
            {
                Color color = room.RoomType switch
                {
                    DungeonRoom.DungeonRoomType.NONE => Color.gray,
                    DungeonRoom.DungeonRoomType.STANDARD => Color.green,
                    DungeonRoom.DungeonRoomType.MAIN => Color.red,
                    _ => throw new ArgumentOutOfRangeException()
                };
                room.Rect.Draw(color, duration);
            }

            dungeonOutput.Hallways.Edges.Draw(Color.cyan, duration);

            Vector2 v = new Vector2(2f, 2f);
            Vector2 v2 = new Vector2(2f, -2f);
            
            foreach (var point in dungeonOutput.RoomEntryPoints.Values.SelectMany(entryPointList => entryPointList))
            {
                DrawLineScaled(point.AsVector() + v, point.AsVector() - v, Color.blue, duration);
                DrawLineScaled(point.AsVector() + v2, point.AsVector() - v2, Color.blue, duration);
            }
        }
    }
}