using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain
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
                Columns = size.y
            }.Schedule(size.x, 128).Complete();
            
            new TopBottomPropagation
            {
                DistanceMap = map,
                Rows = size.x
            }.Schedule(size.y, 128).Complete();
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
        [ReadOnly] public int Columns;
        //Index is row
        public void Execute(int index)
        {
            int start = Columns * index;
            for (int x = 1; x < Columns; x++)
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
        [ReadOnly] public int Rows;
        //Index is column
        public void Execute(int index)
        {
            int start = Rows * index;
            for (int y = 1; y < Rows; y++)
            {
                int j = start + y;
                //TODO remove repeating code
                ushort p1 = DistanceMap[j];
                ushort p2 = DistanceMap[j - 1];
                p2 = p2 == ushort.MaxValue ? p2 : (ushort)(p2 + 1); //Ensuring that p2 never overflows
                DistanceMap[j] = MathFunc.Min(p1, p2);//min of dis[j] and dis[j-1]+1
            }
        }
    }

}