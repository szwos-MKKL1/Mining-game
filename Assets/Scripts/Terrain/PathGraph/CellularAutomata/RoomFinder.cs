using System;
using System.Collections;
using System.Collections.Generic;
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
        public NativeRoomList GetRoomList()
        {
            RoomFinderJob roomFinderJob = new RoomFinderJob(aliveMap, size);
            roomFinderJob.Schedule().Complete();
            roomFinderJob.Dispose();
            return roomFinderJob.NativeRooms; //TODO return as managed
        }
        

        //TODO a lot of unsafe
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

            private NativeList<NativeRoom> rooms;

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
                rooms = new NativeList<NativeRoom>(Allocator.Persistent);
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
                        ProcessNeighbour(pos + UP, roomPositions);
                        ProcessNeighbour(pos + DOWN, roomPositions);
                        ProcessNeighbour(pos + LEFT, roomPositions);
                        ProcessNeighbour(pos + RIGHT, roomPositions);
                    }
                    rooms.Add(new NativeRoom(roomPositions));
                }
            }

            private void ProcessNeighbour(int2 neighbourPos, UnsafeList<int> roomPositions)
            {
                int intPos = neighbourPos.x + neighbourPos.y * size.x;
                if (intPos < 0 || intPos >= arrayCount) return;
                if(!aliveMap[intPos] || visitedMap[intPos]) return;
                visitedMap[intPos] = true;
                activeNodes.Enqueue(intPos);
                activeCount++;
                roomPositions.Add(intPos);
            }

            public NativeRoomList NativeRooms => new(rooms);


            public void Dispose()
            {
                visitedMap.Dispose();
                activeNodes.Dispose();
            }
        }
    }

    public struct NativeRoomList : IEnumerable<NativeRoom>, IDisposable
    {
        private NativeArray<NativeRoom> rooms;

        public NativeRoomList(NativeArray<NativeRoom> rooms)
        {
            this.rooms = rooms;
        }

        public IEnumerator<NativeRoom> GetEnumerator()
        {
            return rooms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        //TODO copy to managed

        public void Dispose()
        {
            foreach (var room in rooms)
            {
                room.Dispose();
            }

            rooms.Dispose();
        }
    }
    
    public struct NativeRoom : IEnumerable<int>, IDisposable
    {
        private UnsafeList<int> roomPos;

        public NativeRoom(UnsafeList<int> roomPos)
        {
            this.roomPos = roomPos;
            Size = this.roomPos.Length;
        }

        public int Size { get; }

        public IEnumerator<int> GetEnumerator()
        {
            return roomPos.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            roomPos.Dispose();
        }
    }
}