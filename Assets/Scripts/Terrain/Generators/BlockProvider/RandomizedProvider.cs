using Terrain.Blocks;

namespace Terrain.Generators
{
    public class RandomizedProvider : IBlockProvider
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
    }
}