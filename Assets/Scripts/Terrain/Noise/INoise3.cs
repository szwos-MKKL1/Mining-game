namespace Terrain.Noise
{
    public interface INoise3 : INoise
    {
        public float GetNoise(float x, float y, float z);
    }
}