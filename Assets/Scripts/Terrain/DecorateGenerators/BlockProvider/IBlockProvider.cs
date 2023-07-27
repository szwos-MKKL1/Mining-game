using Terrain.Blocks;

namespace Terrain.DecorateGenerators.BlockProvider
{
    public interface IBlockProvider
    {
        BlockBase GetNextBlock();
    }
}