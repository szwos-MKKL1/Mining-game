using DelaunatorSharp;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.Generator
{
    public static class VectorExtensions
    {
        public static Vector2Int AsVectorInt(this Vector2 vector)
        {
            return new Vector2Int((int)vector.x, (int)vector.y);
        }
        
        public static Vector2 AsVector(this Vector2Int vector)
        {
            return new Vector2(vector.x, vector.y);
        }
        
        public static Vector2 AsVector(this float2 f)
        {
            return new Vector2(f.x, f.y);
        }
        
        public static int2 AsInt2(this Vector2Int vector)
        {
            return new int2(vector.x, vector.y);
        }
        public static DelaunatorSharp.IPoint AsIPoint(this Vector2 vector)
        {
            return new Point(vector.x, vector.y);
        }
    }
}