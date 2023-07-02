namespace Terrain.Noise
{
    public class FastNoiseAsINoise : INoise
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
    }
}