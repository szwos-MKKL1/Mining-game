using System.CodeDom.Compiler;
using System.Collections.Generic;
using Terrain.Blocks;
using UnityEngine;

namespace Terrain.Generators
{
    /**
     * Uses top values of perlin noise to generate veins
     */
    public class VeinGenerator
    {
        private readonly IBlockProvider blockProvider;
        private FastNoiseLite fastNoiseLite;
        private float veinSize;
        private float frequency;

        public VeinGenerator(IBlockProvider blockProvider)
        {
            this.blockProvider = blockProvider;
        }

        public BlockBase GetBlock(float x, float y)
        {
            return fastNoiseLite.GetNoise(x, y) > (1 - veinSize) ? blockProvider.GetNextBlock() : null;
        }
        
        public BlockBase[,] Generate(Vector2Int size)
        {
            BlockBase[,] map = new BlockBase[size.x, size.y];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    map[x, y] = GetBlock(x, y);
                }
            }

            return map;
        }
    }


}