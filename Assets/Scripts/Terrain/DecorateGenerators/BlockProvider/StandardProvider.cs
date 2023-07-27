using Terrain.Blocks;

namespace Terrain.DecorateGenerators.BlockProvider
{
    public class StandardProvider : IBlockProvider
    {
        private readonly BlockBase mBlockBase;

        public StandardProvider(BlockBase blockBase)
        {
            mBlockBase = blockBase;
        }

        public BlockBase GetNextBlock()
        {
            return mBlockBase;
        }
    }
}