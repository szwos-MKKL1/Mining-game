using UnityEngine;

namespace Terrain
{
    public class DistanceMethods
    {
        public static float SqEuclidianDistance(Vector2 a, Vector2 b)
        {
            float _x = a.x - b.x;
            float _y = a.y - b.y;
            return _x * _x + _y * _y;
        }
        
        public static float SqrtEuclidianDistance(Vector2 a, Vector2 b)
        {
            return Mathf.Sqrt(SqEuclidianDistance(a, b));
        }

        //Taxicab
        public static float ManhattanDistance(Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}