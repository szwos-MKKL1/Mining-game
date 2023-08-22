namespace Terrain.Generator.Noise
{
    public interface INoise3 : INoise
    {
        public float GetNoise(float x, float y, float z);
    }
}