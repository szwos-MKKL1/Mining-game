namespace Terrain.Generator.Noise
{
    public class FastNoiseAsINoise : INoise3
    {
        private readonly FastNoiseLite mFastNoiseLite;

        public FastNoiseAsINoise(FastNoiseLite fastNoiseLite)
        {
            mFastNoiseLite = fastNoiseLite;
        }

        public float GetNoise(float x, float y)
        {
            return mFastNoiseLite.GetNoise(x, y);
        }

        public float GetNoise(float x, float y, float z)
        {
            return mFastNoiseLite.GetNoise(x, y, z);
        }
    }
}