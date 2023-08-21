using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Terrain.Outputs
{
    public readonly struct PosPair<T> : IPosHolder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PosPair(T value, int2 pos)
        {
            Value = value;
            Pos = pos;
        }

        public T Value { get; }
        public int2 Pos { get; }
    }
}