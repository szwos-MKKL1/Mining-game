namespace Terrain.Noise
{
    public delegate float NoiseEquation(float x, float y);
    
    public class EquationNoise : INoise
    {
        private readonly NoiseEquation mNoiseEquation;

        public EquationNoise(NoiseEquation noiseEquation)
        {
            mNoiseEquation = noiseEquation;
        }

        public float GetNoise(float x, float y)
        {
            return mNoiseEquation(x,y);
        }
    }
}