using System;
using System.Collections.Generic;
using NativeTrees;
using Packages.Rider.Editor.UnitTesting;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Rendering.VirtualTexturing;

namespace Terrain.Generator.Structure.Dungeon
{
    [BurstCompile]
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

        //As quadtree doesn't have a method for updating or removing points, we have to re-insert all elements each iteration
        //To compensate for that I have made it so that all the collision calculations are independent from each other on each iteration,
        // updating quadtree only at the end
        public void Execute()
        {
            NativeArray<AABB2D> newRects = new(count, Allocator.Temp); 
            NativeList<int> collidesWithRect = new(count, Allocator.Temp);
            float2 defmovement = new float2(0.5f, 0.5f);
            bool ok = false;
            int separationTicks = 0;
            while (!ok && separationTicks < 3 * count)
            {
                ok = true;
                tree.Clear();
                for (int i = 0; i < count; i++)
                {
                    tree.Insert(i, rects[i]);
                }
                //TODO this could be processed in multiple threads
                for (int i = 0; i < count; i++)
                {
                    AABB2D currentRect = rects[i];
                    tree.RangeAABB2DUnique(rects[i], collidesWithRect);
                    
                    float2 movement = float2.zero;
                    int separationCount = 0;
                    foreach (var otherIndex in collidesWithRect)
                    {
                        AABB2D otherRect = rects[otherIndex];
                        movement += otherRect.Center - currentRect.Center;
                        ++separationCount;
                    }
                    collidesWithRect.Clear();

                    AABB2D newRect = currentRect;
                     
                    if (separationCount > 0)
                    {
                        movement = math.normalizesafe(movement, defmovement);
                        newRect = new AABB2D(currentRect.min - movement, currentRect.max - movement);
                        ok = false;
                    }
                    
                    newRects[i] = newRect;
                }
                
                (rects, newRects) = (newRects, rects);//Swapping with temporary array to reduce reallocation of memory
                separationTicks++;
            }
            if(separationTicks%2!=0) (rects, newRects) = (newRects, rects);//Ensuring that rects is returned
            newRects.Dispose();
            collidesWithRect.Dispose();
        }

        public void Dispose()
        {
            tree.Dispose();
        }
    }
}