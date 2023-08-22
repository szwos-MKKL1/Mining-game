using Terrain.Blocks;

namespace Terrain.Generator.DecorateGenerators.BlockProvider
{
    public interface IBlockProvider
    {
        BlockBase GetNextBlock();
    }
}