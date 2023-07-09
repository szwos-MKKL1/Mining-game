using System;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Terrain.PathGraph
{
    public class CellularAutomataSimulator
    {
        private NativeArray<bool> cellMap;
        private int2 mapSize;
        public static CellularAutomataSimulator CreateFromMap(Vector2Int mapSize, bool[] cellMap)
        {
            if (mapSize.x * mapSize.y != cellMap.Length) throw new Exception("MapSize doesn't fit cellMap");//TODO message
            var simulator = new CellularAutomataSimulator
            {
                cellMap = new NativeArray<bool>(cellMap, Allocator.Persistent),
                mapSize = new int2(mapSize.x, mapSize.y) 
            };
            return simulator;
        }
        
        public static CellularAutomataSimulator CreateRandom(Vector2Int mapSize, float aliveChance, int seed)
        {
            System.Random random = new System.Random(seed);
            int size = mapSize.x * mapSize.y;
            bool[] map = new bool[size];
            for (int i = 0; i < size; i++)
            {
                map[i] = random.GetWithChance(aliveChance);
            }

            return CreateFromMap(mapSize, map);
        } 

        public NativeArray<bool> CellMap => cellMap;

        public int2 MapSize => mapSize;

        public void ExecuteStep(int innerloopBatchCount=64)
        {
            var job = GetSimulationStepJob();
            job.Schedule(job.ArraySize, innerloopBatchCount).Complete();
            cellMap.Dispose();
            cellMap = job.Result;
        }

        //Set cellmap to result of job after completion
        public CellularAutomataJob GetSimulationStepJob()
        {
            int cellCount = mapSize.x*mapSize.y;
            var job = new CellularAutomataJob
            {
                mapSize = mapSize,
                oldMap = cellMap,
                newMap = new NativeArray<bool>(cellCount, Allocator.Persistent),
            };
            
            return job;
        }
    }

    [BurstCompile]
    public struct CellularAutomataJob : IJobParallelFor
    {
        [ReadOnly]
        public int2 mapSize;
        [ReadOnly]
        public NativeArray<bool> oldMap;
        [WriteOnly]
        public NativeArray<bool> newMap;

        public void Execute(int index)
        {
            int2 pos = IntToPos(index);
            int count = NeighbourCount(pos) + (oldMap[index] ? 1 : 0);
            newMap[index] = count >= 4;
        }

        private int NeighbourCount(int2 pos)
        {
            int count = 0;
            for (int xoffset = 0; xoffset < 3; xoffset++)
            {
                for (int yoffset = 0; yoffset < 3; yoffset++)
                {
                    if (xoffset == 0 && yoffset == 0) continue; //Skip parent cell, with 0 offset
                    int x = pos.x + xoffset;
                    int y = pos.y + yoffset;
                    
                    if (x < 0) x = mapSize.x - 1;
                    else if (x >= mapSize.x) x = 0;
                    
                    if (y < 0) y = mapSize.y - 1;
                    else if (y >= mapSize.y) y = 0;

                    if (oldMap[PosToInt(x, y)]) count++;
                }
            }

            return count;
        }

        //index = pos.x + pos.y * mapSize.x
        private int2 IntToPos(int index) => new int2(index % mapSize.x, index / mapSize.y);
        private int PosToInt(int2 pos) => pos.x + pos.y * mapSize.x;
        private int PosToInt(int x, int y) => x + y * mapSize.x;

        public int ArraySize => mapSize.x*mapSize.y;

        public NativeArray<bool> Result => newMap;
    }
}