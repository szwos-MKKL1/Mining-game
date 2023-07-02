using Terrain.Blocks;

namespace Terrain.Generators
{
    public class StandardProvider : IBlockProvider
    {
        private readonly BlockBase BlockBase;

        public StandardProvider(BlockBase blockBase)
        {
            BlockBase = blockBase;
        }

        public BlockBase GetNextBlock()
        {
            return BlockBase;
        }
    }
}