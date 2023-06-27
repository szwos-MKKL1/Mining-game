using Terrain.Blocks;
using Terrain.Noise;
using Unity.Mathematics;
using Random = System.Random;

namespace Terrain.Generators
{
    public class WormCaveGenerator : IDecorateGenerator
    {
        private readonly IBlockProvider blockProvider;
        private readonly INoise mNoise;
        private readonly float start;
        private readonly float end;
        
        public WormCaveGenerator(IBlockProvider blockProvider, int seed, float frequency, float start, float end)
        {
            this.blockProvider = blockProvider;

            System.Random random = new Random(seed); 
            
            FastNoiseLite noise1 = new FastNoiseLite();
            noise1.SetSeed(random.Next());
            noise1.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise1.SetFrequency(frequency);

            this.mNoise = new EquationNoise((x,y) => (math.pow(noise1.GetNoise(x,y,0),2) + math.pow(noise1.GetNoise(x,y,40),2))/0.15f);
            this.start = start;
            this.end = end;
        }
        
        public WormCaveGenerator(IBlockProvider blockProvider, INoise noise, float start, float end)
        {
            this.blockProvider = blockProvider;
            this.mNoise = noise;
            this.start = start;
            this.end = end;
        }
        
        public BlockBase GetBlock(float x, float y)
        {
            float n = mNoise.GetNoise(x, y);
            return n > start && n < end ? blockProvider.GetNextBlock() : null;
        }
    }
}