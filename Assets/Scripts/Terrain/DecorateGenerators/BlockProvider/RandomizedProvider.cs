using Terrain.Blocks;
using Terrain.Generators;

namespace Terrain.DecorateGenerators.BlockProvider
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