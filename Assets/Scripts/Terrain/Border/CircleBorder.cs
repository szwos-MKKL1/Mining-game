using System;
using System.Numerics;
using UnityEngine;

namespace Terrain
{
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

        public CircleBorder(Vector2Int mapSize, int offset)
        {
            centerX = mapSize.x / 2;
            centerY = mapSize.y / 2;
            float radius = Math.Min(centerX, centerY) - offset;
            sqRadius = (int)(radius * radius);
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