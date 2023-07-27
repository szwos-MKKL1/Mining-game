namespace Terrain
{
    public static class RandomChance
    {
        public static bool GetWithChance(this System.Random random, float chance)
        {
            return random.NextDouble() <= chance;
        }
    }
}