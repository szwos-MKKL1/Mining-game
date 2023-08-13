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
    public struct AABBSeparatorJob<T> : IJob where T : unmanaged, IDungeonRoom
    {
        private NativeQuadtree<int> tree;
        private NativeArray<T> rects;
        [ReadOnly] private readonly int count;
        
        public AABBSeparatorJob(NativeArray<T> rects, NativeQuadtree<int> tree)
        {
            count = rects.Length;
            this.tree = tree;
            this.rects = rects;
        }
        
        public AABBSeparatorJob(DungeonRoomTree<T> dungeonRoomTree) : this(dungeonRoomTree.Rooms, dungeonRoomTree.Tree) { }

        //As quadtree doesn't have a method for updating or removing points, we have to re-insert all elements each iteration
        //To compensate for that I have made it so that all the collision calculations are independent from each other on each iteration,
        // updating quadtree only at the end
        public void Execute()
        {
            NativeArray<T> newRects = new(count, Allocator.Temp); 
            NativeList<int> collidesWithRect = new(count, Allocator.Temp);
            float2 defmovement = new float2();
            bool ok = false;
            int separationTicks = 0;
            while (!ok && separationTicks < 3 * count)
            {
                ok = true;
                //TODO this could be processed in multiple threads
                for (int i = 0; i < count; i++)
                {
                    T currentRect = rects[i];
                    tree.RangeAABB2DUnique(rects[i].Rect, collidesWithRect);
                    
                    float2 movement = float2.zero;
                    int separationCount = 0;
                    foreach (var otherIndex in collidesWithRect)
                    {
                        T otherRect = rects[otherIndex];
                        movement += otherRect.Rect.Center - currentRect.Rect.Center;
                        ++separationCount;
                    }
                    collidesWithRect.Clear();

                    T newRect = currentRect;
                     
                    if (separationCount > 0)
                    {
                        movement = math.normalizesafe(movement, defmovement);
                        if (!movement.Equals(defmovement))
                        {
                            newRect.Rect = new AABB2D(currentRect.Rect.min - movement, currentRect.Rect.max - movement);
                        }
                        ok = false;
                    }
                    
                    newRects[i] = newRect;
                }
                
                (rects, newRects) = (newRects, rects);//Swapping with temporary array to reduce reallocation of memory
                tree.Clear();
                for (int i = 0; i < count; i++)
                {
                    tree.Insert(i, rects[i].Rect);
                }
                separationTicks++;
            }
            if(separationTicks%2!=0) (rects, newRects) = (newRects, rects);//Ensuring that rects is returned
            newRects.Dispose();
            collidesWithRect.Dispose();
        }
    }
}