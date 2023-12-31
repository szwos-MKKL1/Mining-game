﻿using Random;
using Unity.Mathematics;

namespace Terrain.Generator.Structure.Dungeon
{
    public interface IRandomPointGenShape
    {
        float2 NextPoint();
        float2x2 Bounds();
    }
    
    public class RandomPointCircle : IRandomPointGenShape
    {
        private readonly IRandom random;
        private readonly float radius;
        private readonly float2 center;

        public RandomPointCircle(IRandom random, float2 center, float radius)
        {
            this.random = random;
            this.radius = radius;
            this.center = center;
        }

        public float2 NextPoint()
        {
            float r = radius * math.sqrt(random.NextFloat());
            float theta = random.NextFloat() * 2 * math.PI;
            float x = center.x + r * math.cos(theta);
            float y = center.y + r * math.sin(theta);
            return new float2(x, y);
        }

        public float2x2 Bounds()
        {
            return new float2x2(center - radius, center + radius);
        }
    }
    
    public class RandomPointEllipse : RandomPointCircle
    {
        private readonly float a2;
        private readonly float b2;
        public RandomPointEllipse(IRandom random, float2 center, float width, float height) : base(random, center, 1)
        {
            a2 = width/2;
            b2 = height/2;
        }

        public new float2 NextPoint()
        {
            float2 randomInCircle = base.NextPoint();
            return new float2(randomInCircle.x * a2, randomInCircle.y * b2);
        }

        //TODO
        // public float2x2 Bounds()
        // {
        //     return new float2x2(center + radius, center - radius);
        // }
    }
}