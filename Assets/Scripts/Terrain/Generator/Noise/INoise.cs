namespace Terrain.Generator.Noise
{
    public interface INoise
    {
        /**
         * Noise value [-1f,1f]
         */
        public float GetNoise(float x, float y);
    }
}