using Terrain.Blocks;

namespace Terrain.Generator.DecorateGenerators.BlockProvider
{
    public class BlockProviderFactory
    {
        public static IBlockProvider Standard(BlockBase blockBase)
        {
            return new StandardProvider(blockBase);
        }
        
        public static IBlockProvider Random(RandomFromList<BlockBase> randomFromList)
        {
            return new RandomizedProvider(randomFromList);
        }
    }
}