using System;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.Generator.Border
{
    public class CircleBorder : IBorderShape
    {
        private readonly int sqRadius;
        private readonly int centerX;
        private readonly int centerY;
        private readonly int top,bottom,left,right;
        public CircleBorder(float radius, Vector2Int center)
        {
            sqRadius = (int)(radius * radius);
            centerX = center.x;
            centerY = center.y;
            top = (int)math.ceil(centerY + radius);
            bottom = (int)math.ceil(centerY - radius);
            left = (int)math.ceil(centerX - radius);
            right = (int)math.ceil(centerX + radius);
        }

        public CircleBorder(Vector2Int mapSize, int offset)
        {
            centerX = mapSize.x / 2;
            centerY = mapSize.y / 2;
            float radius = Math.Min(centerX, centerY) - offset;
            sqRadius = (int)(radius * radius);
            top = (int)math.ceil(centerY + radius);
            bottom = (int)math.ceil(centerY - radius);
            left = (int)math.ceil(centerX - radius);
            right = (int)math.ceil(centerX + radius);
        }
        
        public bool IsInsideBorder(int posX, int posY)
        {
            if (posX < left || posX > right || posY > top || posY < bottom) 
                return false;
            return sq(posX - centerX) + sq(posY - centerY) < sqRadius;
        }

        private static int sq(int x)
        {
            return x * x;
        }
    }
}