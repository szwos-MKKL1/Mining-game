using Unity.Mathematics;

namespace Random
{
    public class GaussianRandom : IRandom
    {
        private readonly IRandom baseRandom;
        private float next = 0f;
        private readonly float _mean;
        private readonly float _std;
        
        public GaussianRandom(IRandom baseRandom, float mean = 0.5f, float std = 0.5f/3f)//0.5/3 gives 99.7% certainty that value will be in [0, 1] range
        {
            this.baseRandom = baseRandom;
            _mean = mean;
            _std = std;
        }

        public int NextInt(int min, int max)
        {
            return (int)NextFloat(min, max);
        }

        //TODO clamping values that are out of range, this is kinda bad, this will make more numbers to generate exactly on edges
        public float NextFloat()
        {
            return math.clamp(NextGaussianFloat(_mean, _std), float.Epsilon, 1f);
        }

        public float NextFloat(float min, float max)
        {
            return math.clamp(NextGaussianFloat(_mean, _std)*(max-min)+min, min, max);
        }

        //https://en.wikipedia.org/wiki/Box%E2%80%93Muller_transform#Basic_form
        public float NextGaussianFloat(float mean, float std)
        {
            float result;
            if (next == 0f)
            {
                float u1 = 1.0f - baseRandom.NextFloat(); //cannot be 0
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