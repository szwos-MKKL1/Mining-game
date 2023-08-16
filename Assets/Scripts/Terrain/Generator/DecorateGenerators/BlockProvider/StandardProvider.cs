using Terrain.Blocks;

namespace Terrain.Generator.DecorateGenerators.BlockProvider
{
    public class StandardProvider : IBlockProvider, IContextBlockProvider
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

        public BlockBase GetNextBlock(IContextBlockProvider.Context context)
        {
            return GetNextBlock();
        }
    }
}