namespace Terrain.Generator.Noise
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
    
    public delegate float NoiseEquation3(float x, float y, float z);
    
    public class EquationNoise3 : INoise3
    {
        private readonly NoiseEquation3 mNoiseEquation;

        public EquationNoise3(NoiseEquation3 noiseEquation)
        {
            mNoiseEquation = noiseEquation;
        }

        public float GetNoise(float x, float y)
        {
            return mNoiseEquation(x,y,0);
        }

        public float GetNoise(float x, float y, float z)
        {
            return mNoiseEquation(x,y,z);
        }
    }
}