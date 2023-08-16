using Unity.Mathematics;

namespace Terrain.Outputs
{
    public interface IPositionHolder
    {
        public int2 Pos { get; }
    }
}