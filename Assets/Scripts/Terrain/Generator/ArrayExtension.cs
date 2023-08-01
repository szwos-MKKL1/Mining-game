namespace Terrain.Generator
{
    public static class ArrayExtension
    {
        public static T GetFrom2D<T>(this T[] data, int x, int y, int xSize)
        {
            return data[y * xSize + x];
        }
        
        public static void SetFrom2D<T>(this T[] data, int x, int y, int xSize, T toSet)
        {
            data[y * xSize + x] = toSet;
        }
    }
}