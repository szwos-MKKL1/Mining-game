using Terrain.Blocks;
using Terrain.DecorateGenerators;

namespace Terrain.Phases
{
    public class DecoratorPhase : IGenerationPhase
    {
        private readonly GenerationData generationData;
        private readonly IDecorateGenerator[] mDecorateGenerators;

        public DecoratorPhase(GenerationData generationData, params IDecorateGenerator[] decorateGenerators)
        {
            this.generationData = generationData;
            this.mDecorateGenerators = decorateGenerators;
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
            BlockBase[,] blocks = terrainChunk.Blocks;
            bool[,] canBuild = terrainChunk.CanBuild;
            int chunkx = terrainChunk.WorldPos.x;
            int chunky = terrainChunk.WorldPos.y;
            for (int xInChunk = 0; xInChunk < TerrainChunk.ChunkSizeX; xInChunk++)
            {
                for (int yInChunk = 0; yInChunk < TerrainChunk.ChunkSizeY; yInChunk++)
                {
                    int xInWorld = chunkx * TerrainChunk.ChunkSizeX + xInChunk;
                    int yInWorld = chunky * TerrainChunk.ChunkSizeY + yInChunk;

                    if (!canBuild[xInChunk, yInChunk]) continue;

                    foreach (var generator in mDecorateGenerators)
                    {
                        BlockBase blockBase = generator.GetBlock(xInWorld, yInWorld);
                        if (blockBase != null) blocks[xInChunk, yInChunk] = blockBase;
                    }
                }
            }

            terrainChunk.Blocks = blocks;
            terrainChunk.CanBuild = canBuild;
        }
    }
}