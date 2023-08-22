using Terrain.Blocks;
using Terrain.Generator.DecorateGenerators.BlockProvider;
using Terrain.Generator.Noise;
using UnityEngine;

namespace Terrain.Generator.DecorateGenerators
{
    /**
     * Uses top values of perlin noise to generate veins
     */
    public class VeinGenerator : IDecorateGenerator
    {
        private readonly IBlockProvider blockProvider;
        private readonly INoise mNoise;
        private readonly float veinThreshold;

        //Uses simple simplex noise
        public VeinGenerator(IBlockProvider blockProvider, int seed, float frequency, float veinThreshold = 0.05f)
        {
            this.blockProvider = blockProvider;
            FastNoiseLite fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetSeed(seed);
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            fastNoiseLite.SetFrequency(frequency);
            this.mNoise = new FastNoiseAsINoise(fastNoiseLite);
            this.veinThreshold = veinThreshold;
        }

        //Can use custom noise
        public VeinGenerator(IBlockProvider blockProvider, INoise noise, float veinThreshold = 0.05f)
        {
            this.blockProvider = blockProvider;
            this.mNoise = noise;
            this.veinThreshold = veinThreshold;
        }

        public BlockBase GetBlock(float x, float y)
        {
            return mNoise.GetNoise(x, y) > (1 - veinThreshold) ? blockProvider.GetNextBlock() : null;
        }
        
        public BlockBase[,] Generate(Vector2Int size)
        {
            BlockBase[,] map = new BlockBase[size.x, size.y];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    map[x, y] = GetBlock(x, y);
                }
            }

            return map;
        }
    }


}