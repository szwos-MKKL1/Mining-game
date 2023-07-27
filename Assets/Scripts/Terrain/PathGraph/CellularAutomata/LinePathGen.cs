using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
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

        [BurstCompile]
        struct GenerateLineJob : IJobParallelFor
        {
            [ReadOnly]
            private NativeArray<Line> lines;
            [NativeDisableParallelForRestriction]
            private NativeArray<byte> layerIdMap;
            [ReadOnly]
            private int2 mapSize;

            public GenerateLineJob(NativeArray<Line> lines, NativeArray<byte> layerIdMap, int2 mapSize)
            {
                this.lines = lines;
                this.layerIdMap = layerIdMap;
                this.mapSize = mapSize;
            }

            public void Execute(int index)
            {
                Line currentLine = lines[index];
                plotLineWidth(
                    currentLine.SourcePos.x,
                    currentLine.SourcePos.y,
                    currentLine.TargetPos.x,
                    currentLine.TargetPos.y,
                    currentLine.Width,
                    currentLine.LayerID);
            }

            private void SetPixel(int x, int y, byte id)
            {
                int i = x + y * mapSize.x;
                byte nId = (byte)(id + 1);
                int oldval = layerIdMap[i];
                if(oldval == 0 || nId < oldval)
                    layerIdMap[i] = nId;
            }

            //TODO optimize this please
            //ported from http://members.chello.at/%7Eeasyfilter/bresenham.js
            private void plotLineWidth(int x0, int y0, int x1, int y1, int th, byte id)
            {                              /* plot an anti-aliased line of width th pixel */
               int dx = math.abs(x1-x0), sx = x0 < x1 ? 1 : -1; 
               int dy = math.abs(y1-y0), sy = y0 < y1 ? 1 : -1; 
               int err, e2 = (int)math.sqrt(dx*dx+dy*dy);                            /* length */

               // if (th <= 1 || e2 == 0) return plotLineAA(x0,y0, x1,y1);         /* assert */
               dx *= 255/e2; dy *= 255/e2; th = 255*(th-1);               /* scale values */

               if (dx < dy) {                                               /* steep line */
                  x1 = (int)math.round((e2+(float)th/2)/dy);                          /* start offset */
                  err = x1*dy-th/2;                  /* shift error value to offset width */
                  for (x0 -= x1*sx; ; y0 += sy) {
                      SetPixel(x1 = x0, y0, id);                  /* aliasing pre-pixel */
                     for (e2 = dy-err-th; e2+dy < 255; e2 += dy)  
                        SetPixel(x1 += sx, y0, id);                      /* pixel on the line */
                     SetPixel(x1+sx, y0, id);                    /* aliasing post-pixel */
                     if (y0 == y1) break;
                     err += dx;                                                 /* y-step */
                     if (err > 255) { err -= dy; x0 += sx; }                    /* x-step */ 
                  }
               } else {                                                      /* flat line */
                  y1 = (int)math.round((e2+(float)th/2)/dx);                          /* start offset */
                  err = y1*dx-th/2;                  /* shift error value to offset width */
                  for (y0 -= y1*sy; ; x0 += sx) {
                      SetPixel(x0, y1 = y0, id);                  /* aliasing pre-pixel */
                     for (e2 = dx-err-th; e2+dx < 255; e2 += dx) 
                        SetPixel(x0, y1 += sy, id);                      /* pixel on the line */
                     SetPixel(x0, y1+sy, id);                    /* aliasing post-pixel */
                     if (x0 == x1) break;
                     err += dy;                                                 /* x-step */ 
                     if (err > 255) { err -= dx; y0 += sy; }                    /* y-step */
                  } 
               }
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
        public int Width;
        
        public byte LayerID;
    }
}