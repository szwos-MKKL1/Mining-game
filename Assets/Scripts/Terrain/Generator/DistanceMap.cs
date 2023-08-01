using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.Generator
{

    //TODO this is just flood fill with limited amount of steps, also could change this algorithm to use burst
    public class DistanceMap : IDisposable
    {
        
        private NativeArray<ushort> map;
        private readonly int2 size;

        public DistanceMap(bool[] startMap, Vector2Int size)
        {
            this.size = size.AsInt2();
            int sizexy = size.x * size.y;
            map = new NativeArray<ushort>(sizexy, Allocator.Persistent);
            for (int i = 0; i < sizexy; i++)
            {
                map[i] = startMap[i] ? (ushort)0 : ushort.MaxValue;
            }
        }
        
        public DistanceMap(Func<Vector2Int, bool> startFunc, Vector2Int size)
        {
            this.size = size.AsInt2();
            int sizexy = size.x * size.y;
            map = new NativeArray<ushort>(sizexy, Allocator.Persistent);
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    map[x + y * size.x] = startFunc(new Vector2Int(x, y)) ? (ushort)0 : ushort.MaxValue;
                }
            }
        }

        public void Generate()
        {
            new LeftRightPropagation
            {
                DistanceMap = map,
                Size = size
            }.Schedule(size.x, 32).Complete();
            
            new TopBottomPropagation
            {
                DistanceMap = map,
                Size = size
            }.Schedule(size.y, 32).Complete();
            
            new RightLeftPropagation()
            {
                DistanceMap = map,
                Size = size
            }.Schedule(size.x, 32).Complete();
            
            new BottomTopPropagation()
            {
                DistanceMap = map,
                Size = size
            }.Schedule(size.y, 32).Complete();
        }

        public ushort GetDistance(Vector2Int pos) => map[pos.x + pos.y * size.x];
        public ushort[] GetDistanceMap => map.ToArray();

        public void Dispose()
        {
            map.Dispose();
        }
    }

    [BurstCompile]
    internal struct MathFunc
    {
        [BurstCompile]
        public static ushort Min(ushort p1, ushort p2)
        {
            return p1 > p2 ? p2 : p1;
        }
    }

    [BurstCompile]
    internal struct LeftRightPropagation : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<ushort> DistanceMap;
        [ReadOnly] public int2 Size;
        //Index is row
        public void Execute(int index)
        {
            int start = Size.y * index;
            for (int x = 1; x < Size.y; x++)
            {
                int j = start + x;
                ushort p1 = DistanceMap[j];
                ushort p2 = DistanceMap[j - 1];
                p2 = p2 == ushort.MaxValue ? p2 : (ushort)(p2 + 1); //Ensuring that p2 never overflows
                DistanceMap[j] = MathFunc.Min(p1, p2);//min of dis[j] and dis[j-1]+1
            }
        }
    }
    
    [BurstCompile]
    internal struct TopBottomPropagation : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<ushort> DistanceMap;
        [ReadOnly] public int2 Size;
        //Index is column
        public void Execute(int index)
        {
            int lastj = index;
            for (int y = 1; y < Size.y; y++)
            {
                int j = index + Size.x*y;//theoretically we could add size.x to j instead of multiplying it with y
                //TODO remove repeating code
                ushort p1 = DistanceMap[j];
                ushort p2 = DistanceMap[lastj];
                lastj = j;
                p2 = p2 == ushort.MaxValue ? p2 : (ushort)(p2 + 1); //Ensuring that p2 never overflows
                DistanceMap[j] = MathFunc.Min(p1, p2);//min of dis[j] and dis[j-1]+1
            }
        }
    }
    
    [BurstCompile]
    internal struct RightLeftPropagation : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<ushort> DistanceMap;
        [ReadOnly] public int2 Size;
        //Index is row
        public void Execute(int index)
        {
            int end = index * Size.x;
            for (int j = (index+1) * Size.x-2; j >= end; j--)
            {
                //TODO remove repeating code
                ushort p1 = DistanceMap[j];
                ushort p2 = DistanceMap[j + 1];
                p2 = p2 == ushort.MaxValue ? p2 : (ushort)(p2 + 1); //Ensuring that p2 never overflows
                DistanceMap[j] = MathFunc.Min(p1, p2);//min of dis[j] and dis[j+1]+1
            }
        }
    }
    
    [BurstCompile]
    internal struct BottomTopPropagation : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<ushort> DistanceMap;
        [ReadOnly] public int2 Size;
        //Index is column
        public void Execute(int index)
        {
            int lastj = (Size.y-1) * Size.x + index;
            for (int y = Size.y-2; y >= 0; y--)
            {
                int j = y * Size.x + index;
                //TODO remove repeating code
                ushort p1 = DistanceMap[j];
                ushort p2 = DistanceMap[lastj];
                lastj = j;
                p2 = p2 == ushort.MaxValue ? p2 : (ushort)(p2 + 1); //Ensuring that p2 never overflows
                DistanceMap[j] = MathFunc.Min(p1, p2);//min of dis[j] and dis[j+1]+1
            }
        }
    }

}