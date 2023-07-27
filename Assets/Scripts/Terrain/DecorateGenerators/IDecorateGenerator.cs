using Terrain.Blocks;
using UnityEngine;

namespace Terrain.DecorateGenerators
{
    public interface IDecorateGenerator
    {
        public BlockBase GetBlock(float x, float y);
    }
}