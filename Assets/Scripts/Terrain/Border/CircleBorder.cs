using System.Numerics;
using UnityEngine;

namespace Terrain
{
    //TODO add center of circle
    public class CircleBorder : IBorderShape
    {
        private readonly int sqRadius;
        private readonly int centerX;
        private readonly int centerY;
        public CircleBorder(float radius, Vector2Int center)
        {
            sqRadius = (int)(radius * radius);
            centerX = center.x;
            centerY = center.y;
        }
        
        public bool IsInsideBorder(int posX, int posY)
        {
            return sq(posX - centerX) + sq(posY - centerY) < sqRadius;
        }

        private static int sq(int x)
        {
            return x * x;
        }
    }
}