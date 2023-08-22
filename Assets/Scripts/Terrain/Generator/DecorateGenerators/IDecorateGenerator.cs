using Terrain.Blocks;

namespace Terrain.Generator.DecorateGenerators
{
    public interface IDecorateGenerator
    {
        public BlockBase GetBlock(float x, float y);
    }
}