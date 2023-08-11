using System;
using System.Collections.Generic;
using NativeTrees;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Rendering.VirtualTexturing;

namespace Terrain.Generator.Structure.Dungeon
{
    public struct AABBSeparatorJob : IJob, IDisposable
    {
        private NativeQuadtree<int> tree;
        private NativeArray<AABB2D> rects;
        [ReadOnly] private readonly int count;
        
        public AABBSeparatorJob(NativeArray<AABB2D> rects, AABB2D bounds)
        {
            count = rects.Length;
            tree = new NativeQuadtree<int>(bounds, Allocator.TempJob);
            this.rects = rects;
        }
        
        public AABBSeparatorJob(NativeArray<AABB2D> rects, NativeQuadtree<int> tree)
        {
            count = rects.Length;
            this.tree = tree;
            this.rects = rects;
        }

        public void Execute()
        {
            NativeArray<AABB2D> newRects = new(count, Allocator.Temp);

            bool ok = false;
            int separationTicks = 0;
            while (!ok && separationTicks < 2 * count)
            {
                ok = true;
                tree.Clear();
                for (int i = 0; i < count; i++)
                {
                    tree.Insert(i, rects[i]);
                }
                for (int i = 0; i < count; i++)
                {
                    AABB2D currentRect = rects[i];
                    NativeParallelHashSet<int> collidesWithRect = new(count, Allocator.Temp);
                    tree.RangeAABB2DUnique(rects[i], collidesWithRect);
                    
                    float2 movement = float2.zero;
                    int separationCount = 0;
                    foreach (var otherIndex in collidesWithRect)
                    {
                        AABB2D otherRect = rects[otherIndex];
                        movement += otherRect.Center - currentRect.Center;
                        ++separationCount;
                    }

                    AABB2D newRect = currentRect;
                    if (separationCount > 0)
                    {
                        movement *= -1;
                        movement = math.normalize(movement);
                        newRect = new AABB2D(currentRect.min + movement, currentRect.max + movement);
                        ok = false;
                    }

                    newRects[i] = newRect;
                }
                
                (rects, newRects) = (newRects, rects);//Swapping with temporary array to reduce reallocation of memory
                separationTicks++;
            }
            //TODO ensure that rects is containing initial buffer and not 
            newRects.Dispose();
        }

        public void Dispose()
        {
            tree.Dispose();
        }
    }
}