using UnityEngine;

namespace Terrain.Generator
{
    public class DistanceMethods
    {
        public static float SqEuclidianDistance(Vector2 a, Vector2 b)
        {
            float _x = a.x - b.x;
            float _y = a.y - b.y;
            return _x * _x + _y * _y;
        }
        
        public static int SqEuclidianDistance(Vector2Int a, Vector2Int b)
        {
            int _x = a.x - b.x;
            int _y = a.y - b.y;
            return _x * _x + _y * _y;
        }
        
        public static float SqrtEuclidianDistance(Vector2 a, Vector2 b)
        {
            return Mathf.Sqrt(SqEuclidianDistance(a, b));
        }
        
        public static int SqrtEuclidianDistance(Vector2Int a, Vector2Int b)
        {
            return (int)Mathf.Sqrt(SqEuclidianDistance(a, b));
        }

        //Taxicab
        public static float ManhattanDistance(Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
        
        //Taxicab
        public static int ManhattanDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}