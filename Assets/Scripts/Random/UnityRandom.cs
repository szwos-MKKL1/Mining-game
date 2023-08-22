namespace Random
{
    public class UnityRandom : IRandom
    {
        public int NextInt(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public float NextFloat()
        {
            return UnityEngine.Random.value;
        }

        public float NextFloat(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}