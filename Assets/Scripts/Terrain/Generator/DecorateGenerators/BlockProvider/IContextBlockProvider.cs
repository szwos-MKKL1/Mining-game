using System.Numerics;
using Terrain.Blocks;

namespace Terrain.Generator.DecorateGenerators.BlockProvider
{
    public interface IContextBlockProvider
    {
        BlockBase GetNextBlock(Context context);

        public class Context
        {
            protected Context(Vector2 position)
            {
                Position = position;
            }

            public readonly Vector2 Position;
        }
    }
}