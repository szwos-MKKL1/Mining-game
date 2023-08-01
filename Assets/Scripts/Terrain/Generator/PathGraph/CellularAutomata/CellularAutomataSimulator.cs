using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.Generator.PathGraph.CellularAutomata
{
    public class CellularAutomataSimulator : IDisposable
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

        /// <summary>
        /// Count of cells in 3x3 neighbourhood, at which cell in middle becomes alive.
        /// If there are "AliveTreshold" or more, alive cells in 3x3 box, cell in middle becomes alive
        /// </summary>
        public int AliveThreshold { get; set; } = 5;

        public NativeArray<bool> CellMap => cellMap;

        public int2 MapSize => mapSize;

        /// <summary>
        /// Uses CellularAutomataJob to execute one step of simulation
        /// </summary>
        /// <param name="innerloopBatchCount">Used in JobHandle.Schedule</param>
        public void ExecuteStep(int innerloopBatchCount=64)
        {
            var job = GetSimulationStepJob();
            job.Schedule(job.ArraySize, innerloopBatchCount).Complete();
            cellMap.Dispose();
            cellMap = job.Result;
        }
        
        
        /// <summary>
        /// Returns Job used to execute one step of simulation.
        /// It should be noted that after each step, cellMap of CellularAutomataSimulator has to be set to result of job for further processing
        /// </summary>
        public CellularAutomataJob GetSimulationStepJob()
        {
            int cellCount = mapSize.x*mapSize.y;
            var job = new CellularAutomataJob
            {
                mapSize = mapSize,
                oldMap = cellMap,
                newMap = new NativeArray<bool>(cellCount, Allocator.Persistent),
                alivethreshold = AliveThreshold
            };
            
            return job;
        }

        public void Dispose()
        {
            cellMap.Dispose();
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
        [ReadOnly]
        public int alivethreshold;

        public void Execute(int index)
        {
            int2 pos = IntToPos(index);
            int count = NeighbourCount(pos) + (oldMap[index] ? 1 : 0);
            newMap[index] = count >= alivethreshold;
        }

        private int NeighbourCount(int2 pos)
        {
            int count = 0;
            //Loop thru all nodes in 3x3 neighborhood of node given by pos
            for (int xoffset = -1; xoffset < 2; xoffset++)
            {
                for (int yoffset = -1; yoffset < 2; yoffset++)
                {
                    if (xoffset == 0 && yoffset == 0) continue; //Skip parent cell, with 0 offset
                    int x = pos.x + xoffset;
                    int y = pos.y + yoffset;
                    
                    
                    //TODO allow user to choose what to do when x,y goes out of map bounds
                    //If x or y goes outside map bounds, value is looped around
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