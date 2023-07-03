using System.Collections.Generic;
using InternalDebug;
using UnityEngine;

namespace Terrain
{
    public delegate bool IsNeighbor(Vector2Int pos);
    
    public class DistanceMap
    {
        private Dictionary<Vector2Int, byte> activeCells = new();
        private Dictionary<Vector2Int, byte> cells = new();
        private IsNeighbor mIsNeighbor;

        public DistanceMap(IEnumerable<Vector2Int> startPoints, IsNeighbor isNeighbor)
        {
            foreach (var point in startPoints)
            {
                activeCells.Add(point, 0);
                cells.Add(point, 0);
            }
            mIsNeighbor = isNeighbor;
        }

        private readonly Vector2Int[] neighbors = { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };

        public Dictionary<Vector2Int, byte> Generate(byte steps)
        {
            for (int s = 0; s < steps; s++)
            {
                Dictionary<Vector2Int, byte> nextActiveCells = new();
                foreach (KeyValuePair<Vector2Int, byte> cell in activeCells)
                {
                    //Find neighbors
                    foreach (var neighborvec in neighbors)
                    {
                        Vector2Int neighborPos = cell.Key + neighborvec;
                        //If neighbor is blocked, go to next
                        if(!mIsNeighbor(neighborPos)) continue;
                        
                        
                        byte val = 0;
                        bool b = cells.TryGetValue(neighborPos, out val);
                        byte dist = (byte)Mathf.Max(val, cell.Value+1);
                        
                        
                        cells[neighborPos] = dist;
                        if (!b) nextActiveCells[neighborPos] = dist;
                    }
                }
                activeCells = nextActiveCells;
            }

            return cells;
        }
        
        public byte[,] Generate(Vector2Int size,byte steps)
        {
            
            byte[,] distMap = new byte[size.x,size.y];
            foreach (KeyValuePair<Vector2Int, byte> cell in Generate(steps))
            {
                distMap[cell.Key.x, cell.Key.y] = cell.Value;
            }

            return distMap;
        }
    }
}