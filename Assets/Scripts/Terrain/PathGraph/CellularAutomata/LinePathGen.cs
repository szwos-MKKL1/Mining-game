using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Terrain.PathGraph.CellularAutomata
{
    public class LinePathGen: ILayerGenerator, IDisposable
    {
        private NativeArray<Line> lines;

        public LinePathGen(Line[] lines)
        {
            this.lines = new NativeArray<Line>(lines, Allocator.TempJob);
        }

        public void GenerateLayer(NativeArray<byte> baseMap, Layer[] layers, int2 mapSize)
        {
            new GenerateLineJob(lines, baseMap, mapSize).Schedule(lines.Length, 32).Complete();
        }

        struct GenerateLineJob : IJobParallelFor
        {
            private NativeArray<Line> lines;
            [NativeDisableParallelForRestriction]
            private NativeArray<byte> baseMap;
            private int2 mapSize;

            public GenerateLineJob(NativeArray<Line> lines, NativeArray<byte> baseMap, int2 mapSize)
            {
                this.lines = lines;
                this.baseMap = baseMap;
                this.mapSize = mapSize;
            }

            public void Execute(int index)
            {
                Line currentLine = lines[index];
                int2 dpos = math.abs(currentLine.TargetPos - currentLine.SourcePos);
                int sx = currentLine.SourcePos.x < currentLine.TargetPos.x ? 1 : -1;
                int sy = currentLine.SourcePos.y < currentLine.TargetPos.y ? 1 : -1;
        
                // Initialize error term and starting point
                int err = dpos.x-dpos.y;
                int x = currentLine.SourcePos.x;
                int y = currentLine.SourcePos.y;

                int x2 =0 ;
                
                float ed = dpos.x+dpos.y == 0 ? 1 : math.sqrt((float)dpos.x*dpos.y+(float)dpos.x*dpos.y);
                // Loop over line until endpoint is reached
                while (true) {
                    baseMap[x + y * mapSize.x] = (byte)(currentLine.layerID+1);
                    // Calculate error term and update current point
                    int e2 = err;
                    if (e2*2 > -dpos.y)
                    {
                        e2 += dpos.y;
                        for (int y2 = y; e2 < ed*currentLine.Width && (currentLine.TargetPos.x != y2 || dpos.x > dpos.y); e2 += dpos.x)
                            baseMap[x + (y2 += sy) * mapSize.x] = (byte)(currentLine.layerID+1);
                        if (x == currentLine.TargetPos.x) break;
                        e2 = err;
                        err -= dpos.y;
                        x += sx;
                    }
                    if (e2*2 < dpos.x)
                    {
                        e2 += dpos.x-e2;
                        for (; e2 < ed*currentLine.Width && (currentLine.TargetPos.x != x2 || dpos.x < dpos.y); e2 += dpos.y)
                            baseMap[(x2 += sx) + y * mapSize.x] = (byte)(currentLine.layerID+1);
                        if(y == currentLine.TargetPos.y) break;
                        err += dpos.x; 
                        y += sy; 
                    }
                }
        
                // Set pixel color in image data for endpoint
                baseMap[currentLine.TargetPos.x + currentLine.TargetPos.y * mapSize.x] = (byte)(currentLine.layerID+1);
            }
        }

        public void Dispose()
        {
            lines.Dispose();
        }
    }

    public struct Line
    {
        public int2 SourcePos;
        public int2 TargetPos;
        public uint Width;
        
        public byte layerID;
    }
}