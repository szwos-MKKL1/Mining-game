using Terrain.Blocks;
using Terrain.DecorateGenerators.BlockProvider;
using Terrain.Noise;
using UnityEngine;

namespace Terrain.DecorateGenerators
{
    /**
     * Uses top values of perlin noise to generate veins
     */
    public class VeinGenerator : IDecorateGenerator
    {
        private readonly IBlockProvider blockProvider;
        private INoise mNoise;
        private float veinSize;

        //Uses simple simplex noise
        public VeinGenerator(IBlockProvider blockProvider, int seed, float frequency, float veinSize = 0.05f)
        {
            this.blockProvider = blockProvider;
            FastNoiseLite fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetSeed(seed);
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            fastNoiseLite.SetFrequency(frequency);
            this.mNoise = new FastNoiseAsINoise(fastNoiseLite);
            this.veinSize = veinSize;
        }

        //Can use custom noise
        public VeinGenerator(IBlockProvider blockProvider, INoise noise, float veinSize = 0.05f)
        {
            this.blockProvider = blockProvider;
            this.mNoise = noise;
            this.veinSize = veinSize;
        }

        public BlockBase GetBlock(float x, float y)
        {
            return mNoise.GetNoise(x, y) > (1 - veinSize) ? blockProvider.GetNextBlock() : null;
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