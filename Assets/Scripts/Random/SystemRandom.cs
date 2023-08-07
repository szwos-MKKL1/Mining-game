namespace Random
{
    public class SystemRandom : IRandom
    {
        private readonly System.Random random;

        public SystemRandom(System.Random random)
        {
            this.random = random;
        }

        public SystemRandom(int seed)
        {
            random = new System.Random(seed);
        }
        
        public SystemRandom()
        {
            random = new System.Random();
        }

        public int NextInt(int min, int max)
        {
            return random.Next(min, max);
        }

        public float NextFloat()
        {
            return (float)random.NextDouble();
        }

        public float NextFloat(float min, float max)
        {
            return min + NextFloat() * (max - min);
        }

        public System.Random AsRandom()
        {
            return random;
        }
    }
}