namespace Terrain.Noise
{
    public interface INoise
    {
        /**
         * Noise value [-1f,1f]
         */
        float GetNoise(float x, float y);
    }
}