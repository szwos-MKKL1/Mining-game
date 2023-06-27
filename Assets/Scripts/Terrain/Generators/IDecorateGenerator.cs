using Terrain.Blocks;
using UnityEngine;

namespace Terrain.Generators
{
    public interface IDecorateGenerator
    {
        public BlockBase GetBlock(float x, float y);
    }
}