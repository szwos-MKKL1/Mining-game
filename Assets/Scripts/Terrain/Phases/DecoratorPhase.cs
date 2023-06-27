using Terrain.Blocks;
using Terrain.Generators;

namespace Terrain
{
    public class DecoratorPhase : IGenerationPhase
    {
        private readonly GenerationData generationData;
        private VeinGenerator mVeinGenerator;

        public DecoratorPhase(GenerationData generationData, VeinGenerator veinGenerator)
        {
            this.generationData = generationData;
            this.mVeinGenerator = veinGenerator;
        }


        public void Generate(TerrainData terrainData)
        {
            for (int chunkx = 0; chunkx < generationData.chunkSize.x; chunkx++)
            {
                for (int chunky = 0; chunky < generationData.chunkSize.y; chunky++)
                {
                    PopulateChunk(terrainData.terrainChunks[chunkx, chunky]);
                }
            }
        }
        
        private void PopulateChunk(TerrainChunk terrainChunk)
        {
            BlockBase[,] blocks = terrainChunk.Blocks;
            bool[,] canBuild = terrainChunk.CanBuild;
            int chunkx = terrainChunk.InWorldPosition.x;
            int chunky = terrainChunk.InWorldPosition.y;
            for (int xInChunk = 0; xInChunk < TerrainChunk.ChunkSizeX; xInChunk++)
            {
                for (int yInChunk = 0; yInChunk < TerrainChunk.ChunkSizeY; yInChunk++)
                {
                    int xInWorld = chunkx * TerrainChunk.ChunkSizeX + xInChunk;
                    int yInWorld = chunky * TerrainChunk.ChunkSizeY + yInChunk;

                    if (!canBuild[xInChunk, yInChunk]) continue;
                    
                    BlockBase blockBase = mVeinGenerator.GetBlock(xInWorld, yInWorld);
                    if (blockBase != null) blocks[xInChunk, yInChunk] = blockBase;
                }
            }

            terrainChunk.Blocks = blocks;
            terrainChunk.CanBuild = canBuild;
        }
    }
}