using Random;
using Unity.Mathematics;

namespace Terrain.Generator.Structure.Dungeon
{
    public interface IRandomRoomSize
    {
        int2 NextRandomSize();
    }

    public class BaseRandomRoomSize : IRandomRoomSize
    {
        private readonly IRandom random;
        private readonly int2 width;
        private readonly int2 height;

        public BaseRandomRoomSize(IRandom random, int widthMin, int widthMax, int heightMin, int heightMax)
        {
            this.random = random;
            width = new int2(widthMin, widthMax);
            height = new int2(heightMin, heightMax);
        }

        public int2 NextRandomSize()
        {
            return new int2(random.NextInt(width.x, width.y), random.NextInt(height.x, height.y));
        }
    }
}