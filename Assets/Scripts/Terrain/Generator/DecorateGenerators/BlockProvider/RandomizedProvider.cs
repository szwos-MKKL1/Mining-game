using Terrain.Blocks;

namespace Terrain.Generator.DecorateGenerators.BlockProvider
{
    public class RandomizedProvider : IBlockProvider, IContextBlockProvider
    {
        private RandomFromList<BlockBase> randomBlockList;

        public RandomizedProvider(RandomFromList<BlockBase> randomBlockList)
        {
            this.randomBlockList = randomBlockList;
        }

        public BlockBase GetNextBlock()
        {
            return randomBlockList.GetRandom();
        }

        public BlockBase GetNextBlock(IContextBlockProvider.Context context)
        {
            return GetNextBlock();
        }
    }
}