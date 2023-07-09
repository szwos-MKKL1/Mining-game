using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Terrain.PathGraph
{
    public class CellularAutomataSimulator
    {
        private SimulationChunk[] chunks;
        private Vector2Int mapSize;
        private Vector2Int chunkCount;
        private Vector2Int chunkSize;

        private CellularAutomataSimulator(Vector2Int chunkCount, Vector2Int mapSize, Vector2Int chunkSize,
            SimulationChunk[] chunks)
        {
            this.chunkCount = chunkCount;
            this.mapSize = mapSize;
            this.chunkSize = this.chunkSize;
            this.chunks = chunks;
        }

        public static CellularAutomataSimulator Create(Vector2Int mapSize, bool[] initialMap, Vector2Int chunkSize)
        {
            Vector2Int chunkCount = new Vector2Int(Mathf.CeilToInt((float)mapSize.x / chunkSize.x),
                Mathf.CeilToInt((float)mapSize.y / chunkSize.y));
            SimulationChunk[] chunks = new SimulationChunk[chunkCount.x * chunkCount.y];
            for (int y = 0; y < chunkCount.y; y++)
            {
                int yChunkSize = Mathf.Min(mapSize.y - y * chunkSize.y, chunkSize.y);
                for (int x = 0; x < chunkCount.x; x++)
                {
                    int xChunkSize = Mathf.Min(mapSize.x - x * chunkSize.x, chunkSize.x);

                    Vector2Int thisChunkSize = new Vector2Int(xChunkSize, yChunkSize);
                    SimulationChunk simulationChunk = new SimulationChunk(thisChunkSize, new Vector2Int(x, y));
                    bool[] data = simulationChunk.Data;
                    for (int yLocal = 0; yLocal < yChunkSize; yLocal++)
                    {
                        for (int xLocal = 0; xLocal < xChunkSize; xLocal++)
                        {
                            data.SetFrom2D(xLocal, yLocal, xChunkSize, initialMap.GetFrom2D(xChunkSize * x + xLocal,
                                yChunkSize * y + yLocal, mapSize.x));
                        }
                    }

                    chunks[y * chunkCount.x + x] = simulationChunk;
                }
            }

            return new CellularAutomataSimulator(chunkCount, mapSize, chunkSize, chunks);
        }

        public static CellularAutomataSimulator CreateRandom(Vector2Int mapSize, Vector2Int chunkSize,
            float aliveChance, int seed = 0)
        {
            System.Random random = new System.Random(seed);
            Vector2Int chunkCount = new Vector2Int(Mathf.CeilToInt((float)mapSize.x / chunkSize.x),
                Mathf.CeilToInt((float)mapSize.y / chunkSize.y));
            SimulationChunk[] chunks = new SimulationChunk[chunkCount.x * chunkCount.y];
            for (int y = 0; y < chunkCount.y; y++)
            {
                int yChunkSize = Mathf.Min(mapSize.y - y * chunkSize.y, chunkSize.y);
                for (int x = 0; x < chunkCount.x; x++)
                {
                    int xChunkSize = Mathf.Min(mapSize.x - x * chunkSize.x, chunkSize.x);

                    Vector2Int thisChunkSize = new Vector2Int(xChunkSize, yChunkSize);
                    SimulationChunk simulationChunk = new SimulationChunk(thisChunkSize, new Vector2Int(x, y));
                    bool[] data = simulationChunk.Data;
                    for (int yLocal = 0; yLocal < yChunkSize; yLocal++)
                    {
                        for (int xLocal = 0; xLocal < xChunkSize; xLocal++)
                        {
                            data.SetFrom2D(xLocal, yLocal, xChunkSize, random.GetWithChance(aliveChance));
                        }
                    }

                    chunks[y * chunkCount.x + x] = simulationChunk;
                }
            }

            return new CellularAutomataSimulator(chunkCount, mapSize, chunkSize, chunks);
        }


        public void SimulationStep()
        {

        }

        private SimulationChunk SimulationStepChunk(SimulationChunk oldChunk)
        {
            Vector2Int size = oldChunk.Size;
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    
                }
            }

            return null;
        }

        private readonly Vector2Int[] neighbours = {
            new(1, 1),new(1, 0),new(1, -1),new(0, -1),
            new(-1, -1),new(-1, 0),new(-1, 1),new(0, 1)
        };
        private int CountNeighbours(SimulationChunk chunk, int xLocal, int yLocal)
        {
            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                Vector2Int localPos = new Vector2Int(xLocal, yLocal) + neighbours[i];
                if (IsAlive(chunk, localPos.x, localPos.y)) count++;
            }

            return count;
        }

        private bool IsAlive(SimulationChunk baseChunk, int xLocal, int yLocal)
        {
            Vector2Int originPos = baseChunk.Pos*chunkSize;
            Vector2Int thisChunkSize = baseChunk.Size;
            if (xLocal < 0 || xLocal >= thisChunkSize.x || yLocal < 0 || yLocal >= thisChunkSize.y)
            {
                return IsAlive(originPos.x + xLocal, originPos.y + yLocal);
            }

            return baseChunk.Data.GetFrom2D(xLocal, yLocal, thisChunkSize.x);
        }

        private bool IsAlive(int x, int y)
        {
            if (x < 0 || x >= mapSize.x || y < 0 || y >= mapSize.y) return false;
            return chunks.GetFrom2D(x / chunkSize.x, y / chunkSize.y, chunkSize.x)
                .Data.GetFrom2D(x % chunkSize.x, y % chunkSize.y, chunkSize.x);
        }
    }

    class SimulationChunk
    {
        private bool[] data;
        private Vector2Int size;
        private Vector2Int pos;

        public SimulationChunk(Vector2Int size, Vector2Int pos)
        {
            this.size = size;
            this.data = new bool[size.x*size.y];
            this.pos = pos;
        }
        public Vector2Int Size => size;
        public Vector2Int Pos => pos;
        public bool[] Data
        {
            get => data;
            set => data = value;
        }
    }
}