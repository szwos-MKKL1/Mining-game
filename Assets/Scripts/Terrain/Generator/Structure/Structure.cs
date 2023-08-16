using System;
using Terrain.Outputs;

namespace Terrain.Generator.Structure
{
    public abstract class Structure
    {
        public abstract BlockCollector getStructureBlocks(Context context);

        public class Context
        {
            public Context(System.Random random)
            {
                Random = random;
            }

            public System.Random Random { get; }
        }
    }
}