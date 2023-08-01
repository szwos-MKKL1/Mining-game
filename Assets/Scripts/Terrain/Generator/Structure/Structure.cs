using System;

namespace Terrain.Generator.Structure
{
    public abstract class Structure
    {
        public abstract BlockCollector getStructureBlocks(Context context);

        public class Context
        {
            public Context(Random random)
            {
                Random = random;
            }

            public Random Random { get; }
        }
    }
}