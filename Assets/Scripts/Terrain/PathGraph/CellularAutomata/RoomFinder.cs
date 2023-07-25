using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.PathGraph.CellularAutomata
{
    public class RoomFinder
    {
        private NativeArray<bool> aliveMap;
        private int2 size;
        public RoomFinder(NativeArray<bool> aliveMap, Vector2Int size)
        {
            this.aliveMap = aliveMap;
            this.size = new int2(size.x, size.y);
        }

        //TODO can be async
        public unsafe List<Room> GetRoomList()
        {
            RoomFinderJob roomFinderJob = new RoomFinderJob(aliveMap, size);
            roomFinderJob.Schedule().Complete();
            
            List<Room> safeRoomList = new();
            NativeArray<UnsafeList<int>> roomList = roomFinderJob.Rooms;
            for (int i = 0; i < roomList.Length; i++)
            {
                ref UnsafeList<int> room = ref UnsafeUtility.ArrayElementAsRef<UnsafeList<int>>(roomList.GetUnsafePtr(), i);
                safeRoomList.Add(new Room(ref room));
            }
            roomFinderJob.Dispose();
            return safeRoomList;
        }
        
        [BurstCompile]
        private struct RoomFinderJob : IJob, IDisposable
        {
            private static readonly int2 UP = new int2(0, 1);
            private static readonly int2 DOWN = new int2(0, -1);
            private static readonly int2 LEFT = new int2(-1, 0);
            private static readonly int2 RIGHT = new int2(1, 0);
            
            [ReadOnly]
            private NativeArray<bool> aliveMap;
            [ReadOnly]
            private int2 size;
            
            private NativeList<UnsafeList<int>> rooms;

            private int arrayCount;
            private NativeArray<bool> visitedMap;
            private NativeQueue<int> activeNodes;
            private int activeCount;

            public RoomFinderJob(NativeArray<bool> aliveMap, int2 size) : this()
            {
                this.aliveMap = aliveMap;
                this.size = size;

                arrayCount = size.x * size.y;
                visitedMap = new NativeArray<bool>(arrayCount, Allocator.TempJob);
                activeNodes = new NativeQueue<int>(Allocator.TempJob);
                rooms = new NativeList<UnsafeList<int>>(Allocator.Persistent);
                activeCount = 0;
            }

            public void Execute()
            {
                
                for (int i = 0; i < arrayCount; i++)
                {
                    if(!aliveMap[i] || visitedMap[i]) continue;
                    //If node is alive and not visited start flood filling
                    //After entire room is found (activeNodes queue is empty) go to next node
                    
                    activeNodes.Enqueue(i);
                    activeCount++; //Using counter instead of NativeQueue.isEmpty()
                    UnsafeList<int> roomPositions = new(1, Allocator.Persistent);
                    roomPositions.Add(i);
                    while (activeCount > 0)
                    {
                        int nodePosInt = activeNodes.Dequeue();
                        activeCount--;
                        int2 pos = new int2(nodePosInt % size.x, nodePosInt / size.y);
                        //Get and process 4 neighbours
                        ProcessNeighbour(pos + UP, ref roomPositions);
                        ProcessNeighbour(pos + DOWN, ref roomPositions);
                        ProcessNeighbour(pos + LEFT, ref roomPositions);
                        ProcessNeighbour(pos + RIGHT, ref roomPositions);
                    }
                    rooms.Add(roomPositions);
                }
            }

            private void ProcessNeighbour(int2 neighbourPos, ref UnsafeList<int> roomPositions)
            {
                int intPos = neighbourPos.x + neighbourPos.y * size.x;
                if (intPos < 0 || intPos >= arrayCount) return;
                if(!aliveMap[intPos] || visitedMap[intPos]) return;
                visitedMap[intPos] = true;
                activeNodes.Enqueue(intPos);
                activeCount++;
                roomPositions.Add(intPos);
            }

            public NativeArray<UnsafeList<int>> Rooms => rooms;

            public unsafe void Dispose()
            {
                visitedMap.Dispose();
                activeNodes.Dispose();
                for (int i = 0; i < rooms.Length; i++)
                {
                    ref UnsafeList<int> room = ref UnsafeUtility.ArrayElementAsRef<UnsafeList<int>>(rooms.GetUnsafePtr(), i);
                    if(room.IsCreated)
                        room.Dispose();
                }

                rooms.Dispose();
            }
        }
    }

    public class Room : IEnumerable<int>
    {
        private List<int> posList;
        public Room(ref UnsafeList<int> posUnsafeList)
        {
            //Converting unsafe pointer to managed array TODO could be done with some cast
            int[] arr = new int[posUnsafeList.Length];
            for (int i = 0; i < posUnsafeList.Length; i++)
            {
                arr[i] = posUnsafeList[i];
            }

            posList = new List<int>(arr);
        }

        public int Size => posList.Count;

        public IEnumerator<int> GetEnumerator()
        {
            return posList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}