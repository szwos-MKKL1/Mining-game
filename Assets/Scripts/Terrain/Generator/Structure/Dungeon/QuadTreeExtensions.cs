using System;
using NativeTrees;
using Unity.Burst;
using Unity.Collections;

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
    }
}