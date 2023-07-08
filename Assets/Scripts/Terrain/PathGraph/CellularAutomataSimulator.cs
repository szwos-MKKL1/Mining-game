using Unity.Mathematics;
using UnityEngine;

namespace Terrain.PathGraph
{
    public class CellularAutomataSimulator
    {
        private SimulationChunk[] chunks;
        private Vector2Int chunkCount;

        private CellularAutomataSimulator(Vector2Int chunkCount, SimulationChunk[] chunks)
        {
            
        }

        public static CellularAutomataSimulator Create(Vector2Int mapSize, bool[] initialMap, Vector2Int chunkSize)
        {
            Vector2Int chunkCount = new Vector2Int(Mathf.CeilToInt((float)mapSize.x / chunkSize.x),
                Mathf.CeilToInt((float)mapSize.y / chunkSize.y));
            SimulationChunk[] chunks = new SimulationChunk[chunkCount.x*chunkCount.y];
            for (int x = 0; x < chunkCount.x; x++)
            {
                for (int y = 0; y < chunkCount.y; y++)
                {
                    chunks[x*chunkCount.x+chunkCount.y] = new SimulationChunk(chunkSize);
                }
            }
            

            return new CellularAutomataSimulator(chunkCount, chunks);
        }
        
    }

    class SimulationChunk
    {
        private bool[] data;

        public SimulationChunk(Vector2Int size)
        {
            this.data = new bool[size.x*size.y];
        }

        public bool[] Data
        {
            get => data;
            set => data = value;
        }
    }
}