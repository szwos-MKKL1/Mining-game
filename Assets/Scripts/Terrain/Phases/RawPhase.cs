using Terrain.Blocks;
using UnityEngine;

namespace Terrain.Phases
{
    //First phase of terrain generation where only barrier is set and terrain filled with base material
    public class RawPhase : IGenerationPhase
    {
        private readonly GenerationData generationData;
        private readonly IBorderShape borderShape;

        public RawPhase(GenerationData generationData)
        {
            this.generationData = generationData;
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
            bool[,] canBuild = terrainChunk.CanBuild;
            for (int xInChunk = 0; xInChunk < TerrainChunk.ChunkSizeX; xInChunk++)
            {
                for (int yInChunk = 0; yInChunk < TerrainChunk.ChunkSizeY; yInChunk++)
                {
                    int xInWorld = chunkx * TerrainChunk.ChunkSizeX + xInChunk;
                    int yInWorld = chunky * TerrainChunk.ChunkSizeY + yInChunk;

                    if (borderShape.IsInsideBorder(xInWorld, yInWorld))
                    {
                        canBuild[xInChunk, yInChunk] = true;
                        blocks[xInChunk, yInChunk] = BlockRegistry.AIR;
                    }
                    else blocks[xInChunk, yInChunk] = BlockRegistry.BEDROCK;
                }
            }

            // //TODO is this needed?
            // terrainChunk.Blocks = blocks;
            // terrainChunk.CanBuild = canBuild;

            return terrainChunk;
        }
    }
}