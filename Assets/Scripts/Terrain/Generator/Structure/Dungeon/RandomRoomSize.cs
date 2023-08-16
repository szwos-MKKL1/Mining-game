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
        private readonly float2 heightM;

        public BaseRandomRoomSize(IRandom random, int widthMin, int widthMax, float heightDeviation)
        {
            this.random = random;
            width = new int2(widthMin, widthMax);
            heightM = new float2(1f-heightDeviation, 1f+heightDeviation);
        }

        public int2 NextRandomSize()
        {
            int w = random.NextInt(width.x, width.y);
            return new int2(w, (int)(w*random.NextFloat(0.8f, 1.2f)));
        }
    }
}