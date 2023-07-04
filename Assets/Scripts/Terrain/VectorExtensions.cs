using UnityEngine;

namespace Terrain
{
    public static class VectorExtensions
    {
        public static Vector2Int ToVectorInt(this Vector2 vector)
        {
            return new Vector2Int((int)vector.x, (int)vector.y);
        }
        
        public static Vector2 ToVector(this Vector2Int vector)
        {
            return new Vector2(vector.x, vector.y);
        }
    }
}