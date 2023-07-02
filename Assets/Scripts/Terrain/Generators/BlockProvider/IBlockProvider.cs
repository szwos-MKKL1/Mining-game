using Terrain.Blocks;

namespace Terrain.Generators
{
    public interface IBlockProvider
    {
        BlockBase GetNextBlock();
    }
}