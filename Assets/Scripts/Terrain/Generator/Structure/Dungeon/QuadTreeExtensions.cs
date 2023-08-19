using System;
using NativeTrees;
using QuikGraph;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.Generator.Structure.Dungeon
{
    public static class QuadTreeExtensions
    {
        [BurstCompile]
        public static void RangeAABB2DUnique<T>(this NativeQuadtree<T> quadtree, AABB2D range, NativeList<T> results) 
            where T : unmanaged, IEquatable<T>
        {
            RangeAABB2DUniqueVisitor<T> visitor = new()
            {
                Results = results
            };
 
            quadtree.Range(range, ref visitor);
        }
        
        [BurstCompile]
        private struct RangeAABB2DUniqueVisitor<T> : IQuadtreeRangeVisitor<T> where T : unmanaged, IEquatable<T>
        {
            public NativeList<T> Results;
    
            public bool OnVisit(T obj, AABB2D objBounds, AABB2D queryRange)
            {
                // check if our object's AABB2D overlaps with the query AABB2D
                if (objBounds.Overlaps(queryRange))
                    Results.Add(obj);

                return true; // always keep iterating, we want to catch all objects
            }
        }
        
        // public static void LineAABB2D<T, Q>(this NativeQuadtree<T> quadtree, IEdge<Q> range, NativeList<T> results) 
        //     where T : unmanaged, IEquatable<T>
        // {
        //     RangeAABB2DUniqueVisitor<T> visitor = new()
        //     {
        //         Results = results
        //     };
        //
        //     quadtree.Raycast(new Ray2D())
        //     quadtree.Range(range, ref visitor);
        // }
        //
        // private struct RangeAABB2DUniqueVisitor<T> : IQuadtreeRangeVisitor<T> where T : unmanaged, IEquatable<T>
        // {
        //     public NativeList<T> Results;
        //
        //     public bool OnVisit(T obj, AABB2D objBounds, AABB2D queryRange)
        //     {
        //         // check if our object's AABB2D overlaps with the query AABB2D
        //         if (objBounds.IntersectsRay(queryRange))
        //             Results.Add(obj);
        //
        //         return true; // always keep iterating, we want to catch all objects
        //     }
        // }

        public static void Draw(this AABB2D aabb2D, Color color, float duration = 10f)
        {
            float2 min = aabb2D.min;
            float2 max = aabb2D.max;
            DrawLineScaled(min, new float2(max.x, min.y), color, duration);
            DrawLineScaled(min, new float2(min.x, max.y), color, duration);
            DrawLineScaled(new float2(min.x, max.y), max, color, duration);
            DrawLineScaled(new float2(max.x, min.y), max, color, duration);
        }
        
        private static void DrawLineScaled(float2 a, float2 b, Color color, float duration = 10f)
        {
            Debug.DrawLine(new Vector2(a.x, a.y) * 0.16f, new Vector2(b.x, b.y) * 0.16f, color, duration, false);
        }
    }
}