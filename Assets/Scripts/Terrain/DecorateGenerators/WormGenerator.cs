using InternalDebug;
using Terrain.Blocks;
using Terrain.DecorateGenerators.BlockProvider;
using Terrain.Noise;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace Terrain.DecorateGenerators
{
    public class WormGenerator : IDecorateGenerator
    {
        private readonly IBlockProvider blockProvider;
        private readonly INoise mNoise;
        private readonly float threshold;

        public WormGenerator(IBlockProvider blockProvider, int seed, float frequency, float threshold)
        {
            this.threshold = (threshold + 1)/2;
            this.blockProvider = blockProvider;
            FastNoiseLite noise1 = new FastNoiseLite();
            noise1.SetSeed(seed);
            noise1.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise1.SetFrequency(frequency);
            
            ImageDebug.SaveImg(new Vector2Int(1024, 1024), new FastNoiseAsINoise(noise1), "noise1.png");
            const float nextLayerOffset = 40f;
            //This algorithm uses noise1^2+noise2^2 to generate worm like caves
            //If performance ever becomes an issue, abs(noise1)+abs(noise2) can be used instead
            this.mNoise = new EquationNoise((x,y) => (math.pow(noise1.GetNoise(x,y,0),2) + math.pow(noise1.GetNoise(x,y,nextLayerOffset),2))/this.threshold);
            ImageDebug.SaveImg(new Vector2Int(1024, 1024), mNoise, "noise2.png");
        }
        
        public WormGenerator(IBlockProvider blockProvider, INoise3 noise, float threshold)
        {
            this.threshold = (threshold + 1)/2;
            const float nextLayerOffset = 40f;
            this.mNoise = new EquationNoise((x,y) => (math.pow(noise.GetNoise(x,y,0),2) + math.pow(noise.GetNoise(x,y,nextLayerOffset),2))/this.threshold);
            this.blockProvider = blockProvider;
            this.threshold = threshold;
        }
        
        public BlockBase GetBlock(float x, float y)
        {
            float n = mNoise.GetNoise(x, y);
            return n < threshold ? blockProvider.GetNextBlock() : null;
        }
    }
}