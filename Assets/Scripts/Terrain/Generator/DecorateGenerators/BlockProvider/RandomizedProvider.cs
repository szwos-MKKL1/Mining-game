using Terrain.Blocks;

namespace Terrain.Generator.DecorateGenerators.BlockProvider
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