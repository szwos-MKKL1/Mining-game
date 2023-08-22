namespace Random
{
    public interface IRandom
    {
        public int NextInt(int min, int max);
        public float NextFloat();
        public float NextFloat(float min, float max);
    }
}