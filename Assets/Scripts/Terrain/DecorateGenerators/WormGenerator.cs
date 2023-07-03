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
            float revreshold = 1f / this.threshold;
            this.blockProvider = blockProvider;
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetSeed(seed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(frequency);
            
            //ImageDebug.SaveImg(new Vector2Int(1024, 1024), new FastNoiseAsINoise(noise), "noise1.png");
            const float nextLayerOffset = 40f;
            //This algorithm uses noise1^2+noise2^2 to generate worm like caves
            //If performance ever becomes an issue, abs(noise1)+abs(noise2) can be used instead
            //this.mNoise = new EquationNoise((x,y) => (math.pow(noise1.GetNoise(x,y,0),2) + math.pow(noise1.GetNoise(x,y,nextLayerOffset),2))/this.threshold);
            this.mNoise = new EquationNoise((x,y) => (math.abs(noise.GetNoise(x,y,0)) + math.abs(noise.GetNoise(x,y,nextLayerOffset)))*revreshold);
            //ImageDebug.SaveImg(new Vector2Int(1024, 1024), mNoise, "noise2.png");
        }
        
        public WormGenerator(IBlockProvider blockProvider, INoise3 noise, float threshold)
        {
            this.threshold = (threshold + 1)/2;
            float revreshold = 1f / this.threshold;
            const float nextLayerOffset = 40f;
            this.mNoise = new EquationNoise((x,y) => (math.abs(noise.GetNoise(x,y,0)) + math.abs(noise.GetNoise(x,y,nextLayerOffset)))*revreshold);
            this.blockProvider = blockProvider;
        }
        
        public BlockBase GetBlock(float x, float y)
        {
            float n = mNoise.GetNoise(x, y);
            return n < threshold ? blockProvider.GetNextBlock() : null;
        }
    }
}