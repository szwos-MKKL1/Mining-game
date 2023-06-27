using Terrain.Blocks;
using UnityEngine;

namespace Terrain
{
    //First phase of terrain generation where only basic caves/holes are generated
    public class RawPhase : IGenerationPhase
    {
        private readonly GenerationData generationData;
        private readonly FastNoiseLite fastNoiseLite;
        private IBorderShape borderShape;

        public RawPhase(GenerationData generationData)
        {
            this.generationData = generationData;
            fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            fastNoiseLite.SetFrequency(0.02f);
            
            borderShape = generationData.BorderShape;
        }

        public void Generate(TerrainData terrainData)
        {
            
            for (int chunkx = 0; chunkx < generationData.chunkSize.x; chunkx++)
            {
                for (int chunky = 0; chunky < generationData.chunkSize.y; chunky++)
                {
                    terrainData.terrainChunks[chunkx, chunky] = GenerateChunk(chunkx, chunky);
                }
            }
        }

        private TerrainChunk GenerateChunk(int chunkx, int chunky)
        {
            TerrainChunk terrainChunk = new TerrainChunk(new Vector2Int(chunkx, chunky));
            BlockBase[,] blocks = terrainChunk.Blocks;
            for (int xInChunk = 0; xInChunk < TerrainChunk.ChunkSizeX; xInChunk++)
            {
                for (int yInChunk = 0; yInChunk < TerrainChunk.ChunkSizeY; yInChunk++)
                {
                    int xInWorld = chunkx * 512 + xInChunk;
                    int yInWorld = chunky * 512 + yInChunk;

                    if (borderShape.IsInsideBorder(xInWorld, yInWorld))
                    {
                        blocks[xInChunk, yInChunk] = fastNoiseLite.GetNoise(xInWorld, yInWorld) > 0
                            ? BlockRegistry.AIR
                            : BlockRegistry.ROCK;
                    }
                    else blocks[xInChunk, yInChunk] = BlockRegistry.BEDROCK;
                }
            }

            return terrainChunk;
        }
    }
}