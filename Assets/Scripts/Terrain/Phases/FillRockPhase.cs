using System;
using System.Diagnostics.CodeAnalysis;
using Terrain.Blocks;
using Terrain.DecorateGenerators.BlockProvider;
using Terrain.Generators;

namespace Terrain.Phases
{
    [PhaseDependency(typeof(RawPhase), DependencyOrder.Before)]
    public class FillRockPhase : IGenerationPhase
    {
        private readonly GenerationData generationData;
        private readonly IBlockProvider blockProvider;

        public FillRockPhase(GenerationData generationData, IBlockProvider blockProvider)
        {
            this.generationData = generationData;
            this.blockProvider = blockProvider;
        }

        public void Generate(TerrainData terrainData)
        {
            for (int chunkx = 0; chunkx < generationData.chunkSize.x; chunkx++)
            {
                for (int chunky = 0; chunky < generationData.chunkSize.y; chunky++)
                {
                    PopulateChunk(terrainData.Chunks[chunkx, chunky]);
                }
            }
        }

        private void PopulateChunk(TerrainChunk terrainChunk)
        {

            BlockBase[] blocks = terrainChunk.Blocks;
            bool[] canBuild = terrainChunk.CanBuild;
            for (int xInChunk = 0; xInChunk < TerrainChunk.ChunkSizeX; xInChunk++)
            {
                for (int yInChunk = 0; yInChunk < TerrainChunk.ChunkSizeY; yInChunk++)
                {
                    int loc = xInChunk * TerrainChunk.ChunkSizeX + yInChunk;
                    if (canBuild[loc])
                        blocks[loc] = blockProvider.GetNextBlock();
                }
            }
        }
    }
}