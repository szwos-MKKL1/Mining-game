using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terrain.Blocks;
using Terrain.DecorateGenerators.BlockProvider;
using Terrain.Generators;
using UnityEngine;

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
            foreach (KeyValuePair<Vector2Int, TerrainChunk> chunkPair in terrainData)
            {
                PopulateChunk(chunkPair.Value);
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