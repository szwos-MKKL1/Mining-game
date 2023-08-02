using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace Terrain.Generator.Structure.Dungeon
{
    public interface IRandomRoomSize
    {
        int2 NextRandomSize();
    }

    public class BaseRandomRoomSize : IRandomRoomSize
    {
        private readonly Random random;
        private readonly int2 width;
        private readonly int2 height;

        public BaseRandomRoomSize(Random random, int widthMin, int widthMax, int heightMin, int heightMax)
        {
            this.random = random;
            width = new int2(widthMin, widthMax);
            height = new int2(heightMin, heightMax);
        }

        public int2 NextRandomSize()
        {
            return new int2(random.Next(width.x, width.y), random.Next(height.x, height.y));
        }
    }
}