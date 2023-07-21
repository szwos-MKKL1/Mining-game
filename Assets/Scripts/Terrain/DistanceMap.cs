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
        
        private NativeArray<ushort> initialMap;
        private readonly int2 size;

        public DistanceMap(IEnumerable<Vector2Int> startPoints, Vector2Int size)
        {
            this.size = size.AsInt2();
            initialMap = new NativeArray<ushort>(this.size.x * this.size.y, Allocator.Persistent);
            foreach (var point in startPoints)
            {
                initialMap[point.x + point.y * this.size.x] = 0;
            }
        }

        public void Generate(ushort steps)
        {
            for (int i = 0; i < steps; i++)
            {
                FloodFillJob floodFillJob = new FloodFillJob
                {
                    Map = initialMap,
                    Size = size,
                    FillStepFrom = (ushort)i
                };
                floodFillJob.Schedule(size.x * size.y, 1024).Complete();
            }
        }

        public ushort Distance(Vector2Int pos) => initialMap[pos.x + pos.y * size.x];

        public void Dispose()
        {
            initialMap.Dispose();
        }
    }

    [BurstCompile]
    internal struct FloodFillJob : IJobParallelFor
    {
        [ReadOnly] private static readonly int2 Up = new int2(0, 1);
        [ReadOnly] private static readonly int2 Down = new int2(0, -1);
        [ReadOnly] private static readonly int2 Left = new int2(-1, 0);
        [ReadOnly] private static readonly int2 Right = new int2(1, 0);
        
        [NativeDisableParallelForRestriction] public NativeArray<ushort> Map;
        [ReadOnly] public int2 Size;
        [ReadOnly] public ushort FillStepFrom;
        [BurstCompile]
        public void Execute(int index)
        {
            if (Map[index] != FillStepFrom) return;
            ProcessNeighbour(IntToPos(index, Size) + Up);
            ProcessNeighbour(IntToPos(index, Size) + Down);
            ProcessNeighbour(IntToPos(index, Size) + Left);
            ProcessNeighbour(IntToPos(index, Size) + Right);
        }
        
        [BurstCompile]
        private void ProcessNeighbour(int2 neighbourPos)
        {
            if (neighbourPos.x < 0 || neighbourPos.x >= Size.x || neighbourPos.y < 0 ||
                neighbourPos.y >= Size.y) return;
            int neighbourIndex = PosToInt(neighbourPos, Size.x);
            if (Map[neighbourIndex] != 0) return;
            Map[neighbourIndex] = (ushort)(FillStepFrom + 1);
        }
        [BurstCompile]
        private int2 IntToPos(int index, int2 size) => new int2(index % size.x, index / size.y);
        [BurstCompile]
        private int PosToInt(int2 pos, int sizex) => pos.x + pos.y * sizex;
    }
}