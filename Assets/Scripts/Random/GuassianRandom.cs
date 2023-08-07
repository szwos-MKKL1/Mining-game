using Unity.Mathematics;

namespace Random
{
    public class GaussianRandom : IRandom
    {
        private readonly IRandom baseRandom;
        private float next = 0f;
        private readonly float _mean;
        private readonly float _std;
        
        public GaussianRandom(IRandom baseRandom, float mean = 0f, float std = 1f)
        {
            this.baseRandom = baseRandom;
            this._mean = mean;
            this._std = std;
        }

        public int NextInt(int min, int max)
        {
            return (int)NextFloat(min, max);
        }

        public float NextFloat()
        {
            return NextGaussianFloat(_mean, _std);
        }

        public float NextFloat(float min, float max)
        {
            return NextGaussianFloat(_mean + (min+max)/2f, _std * (max-min));
        }

        public float NextGaussianFloat(float mean, float std)
        {
            float result;
            if (next == 0f)
            {
                float u1 = 1.0f - baseRandom.NextFloat(); // [0, 1) -> (0, 1]
                float u2 = baseRandom.NextFloat();
                float radius = math.sqrt(-2 * math.log(u1));
                float theta = math.PI * 2 * u2;
                result = radius * math.cos(theta);
                next = radius * math.sin(theta);
            }
            else
            {
                result = next;
                next = 0f;
            }

            return result * std + mean;
        }
    }
}