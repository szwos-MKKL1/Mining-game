using System;
using System.Collections.Generic;
using System.Linq;
using InternalDebug;
using NativeTrees;
using Terrain.Outputs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.Generator.Structure.Dungeon
{
    public class DungOutRasterisation
    {
        private readonly CollectorBase<PosPair<DungeonRoom.DungeonBlockTypes>> collector;
        
        public DungOutRasterisation(DungeonOutput<DungeonRoom> dungeonOutput)
        {
            FindWallsJob findWallsJob = new FindWallsJob(dungeonOutput);
            findWallsJob.Schedule(findWallsJob.ArraySize, 1024).Complete();
            collector = findWallsJob.GetResult();
            findWallsJob.Dispose();
        }

        public CollectorBase<PosPair<DungeonRoom.DungeonBlockTypes>> GetResult()
        {
            return collector;
        }

        [BurstCompile]
        private struct FindWallsJob : IJobParallelFor, IDisposable
        {
            [ReadOnly]
            private NativeArray<DungeonRoom.DungeonSpaceType> blocks;
            [WriteOnly]
            private NativeArray<DungeonRoom.DungeonBlockTypes> result;
            private int2 size;
            private int2 minpos;
            private int count;

            public FindWallsJob(DungeonOutput<DungeonRoom> dungeonOutput)
            {
                float minx = float.MaxValue, miny = float.MaxValue, maxx = float.MinValue, maxy = float.MinValue;
                foreach (var rect in dungeonOutput.Rooms.Select(room => room.Rect))
                {
                    if (rect.min.x < minx) minx = rect.min.x;
                    if (rect.min.y < miny) miny = rect.min.y;
                    if (rect.max.x > maxx) maxx = rect.max.x;
                    if (rect.max.y > maxy) maxy = rect.max.y;
                }

                AABB2D bounds = new AABB2D(new float2(minx, miny), new float2(maxx+1, maxy+1));
                minpos = new int2(bounds.min);
                size = new int2(bounds.max - bounds.min);
                count = size.x * size.y;
                blocks = new NativeArray<DungeonRoom.DungeonSpaceType>(count, Allocator.TempJob);
                foreach (var hallway in dungeonOutput.Hallways.Edges)
                {
                    List<float2> points = hallway.Points;
                    for (int i = 0; i < points.Count-1; i++)
                    {
                        AABB2D rect = DungeonExtensions.RectOnLine(points[i], points[i + 1], 3, 0.05f);
                        SetRect(blocks, size.x, new int2(rect.min)-minpos, new int2(rect.max)-minpos, (byte)DungeonRoom.DungeonSpaceType.HALLWAY);
                    }
                }

                int2 roomOffset = new int2(1, 1);
                foreach (var room in dungeonOutput.Rooms)
                {
                    byte v;
                    switch (room.RoomType)
                    {
                        case DungeonRoom.DungeonRoomType.STANDARD:
                            v = (byte)DungeonRoom.DungeonSpaceType.STANDARD;
                            break;
                        case DungeonRoom.DungeonRoomType.MAIN:
                            v = (byte)DungeonRoom.DungeonSpaceType.MAIN;
                            break;
                        default:
                            continue;
                    }
                    SetRect(blocks, size.x, new int2(room.Rect.min)-minpos+roomOffset, new int2(room.Rect.max)-minpos-roomOffset, v);
                }
                // ImageDebug.SaveImg(blocks.ToArray(), new Vector2Int(size.x, size.y), type =>
                // {
                //     return type switch
                //     {
                //         DungeonRoom.DungeonSpaceType.NONE => Color.black,
                //         DungeonRoom.DungeonSpaceType.STANDARD => Color.green,
                //         DungeonRoom.DungeonSpaceType.MAIN => Color.red,
                //         DungeonRoom.DungeonSpaceType.HALLWAY => Color.cyan,
                //         _ => Color.black
                //     };
                // }, "blocks.png");
                result = new NativeArray<DungeonRoom.DungeonBlockTypes>(count, Allocator.TempJob);
            }

            private static unsafe void SetRect(NativeArray<DungeonRoom.DungeonSpaceType> _blocks,int sizex, int2 min, int2 max, byte value)
            {
                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        _blocks[y * sizex + x] = (DungeonRoom.DungeonSpaceType)value;
                    }
                }
                // byte* blockPointer = (byte*)_blocks.GetUnsafePtr();
                // int2 diff = max - min;
                // int minIndex = min.y * sizex + min.x;
                // for (int i = 0; i < diff.y; i++)
                // {
                //     UnsafeUtility.MemSet(blockPointer + minIndex, value, diff.x);
                //     minIndex += sizex;
                // }
                
            }

            public void Execute(int index)
            {
                DungeonRoom.DungeonSpaceType spaceType = blocks[index];
                if (spaceType == DungeonRoom.DungeonSpaceType.NONE)
                {
                    result[index] = DungeonRoom.DungeonBlockTypes.NONE;
                    return;
                }
                int2 pos = new int2(index % size.x, index / size.x);
                bool hasNoneNeighbour = false;
                // bool hasMainNeighbour = false;
                // bool hasStandardNeighbour = false;
                // bool hasHallwayNeighbour = false;
                for (int xoffset = -1; xoffset < 2; xoffset++)
                {
                    for (int yoffset = -1; yoffset < 2; yoffset++)
                    {
                        if (xoffset == 0 && yoffset == 0) continue; //Skip parent cell, with 0 offset
                        int x = pos.x + xoffset;
                        int y = pos.y + yoffset;
                        
                        if (x < 0 || x >= size.x || y < 0 || y >= size.y) continue;
                        switch (blocks[x + y * size.x])
                        {
                            case DungeonRoom.DungeonSpaceType.NONE:
                                hasNoneNeighbour = true;
                                break;
                            // case DungeonRoom.DungeonSpaceType.STANDARD:
                            //     hasStandardNeighbour = true;
                            //     break;
                            // case DungeonRoom.DungeonSpaceType.MAIN:
                            //     hasMainNeighbour = true;
                            //     break;
                            // case DungeonRoom.DungeonSpaceType.HALLWAY:
                            //     hasHallwayNeighbour = true;
                            //     break;
                        }
                    }
                }


                if (!hasNoneNeighbour)
                {
                    result[index] = DungeonRoom.DungeonBlockTypes.AIR;
                    return;
                }

                result[index] = spaceType switch
                {
                    DungeonRoom.DungeonSpaceType.MAIN => DungeonRoom.DungeonBlockTypes.MAIN_ROOM_WALL,
                    DungeonRoom.DungeonSpaceType.STANDARD => DungeonRoom.DungeonBlockTypes.STANDARD_ROOM_WALL,
                    DungeonRoom.DungeonSpaceType.HALLWAY => DungeonRoom.DungeonBlockTypes.HALLWAY_WALL,
                    _ => result[index]
                };
            }

            public int ArraySize => count;

            public CollectorBase<PosPair<DungeonRoom.DungeonBlockTypes>> GetResult()
            {
                // ImageDebug.SaveImg(result.ToArray(), new Vector2Int(size.x, size.y), type =>
                // {
                //     return type switch
                //     {
                //         DungeonRoom.DungeonBlockTypes.NONE => Color.black,
                //         DungeonRoom.DungeonBlockTypes.STANDARD_ROOM_WALL => Color.green,
                //         DungeonRoom.DungeonBlockTypes.MAIN_ROOM_WALL => Color.red,
                //         DungeonRoom.DungeonBlockTypes.HALLWAY_WALL => Color.cyan,
                //         _ => Color.black
                //     };
                // }, "result.png");
                
                CollectorBase<PosPair<DungeonRoom.DungeonBlockTypes>> collector = new();
                
                for (int y = 0; y < size.y; y++)
                {
                    int ysize = y * size.x;
                    for (int x = 0; x < size.x; x++)
                    {
                        var b = result[ysize + x];
                        if(b != DungeonRoom.DungeonBlockTypes.NONE)
                            collector.Add(new PosPair<DungeonRoom.DungeonBlockTypes>(b, new int2(x, y)+minpos));
                    }
                }

                return collector;
            }

            public void Dispose()
            {
                blocks.Dispose();
                result.Dispose();
            }
        }
    }
}