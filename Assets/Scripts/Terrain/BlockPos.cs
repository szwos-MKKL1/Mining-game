using Terrain.Blocks;
using Terrain.Outputs;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain
{
    public readonly struct BlockPos : IPositionHolder
    {
        private readonly BlockBase blockBase;
        private readonly int2 pos;

        public BlockPos(BlockBase blockBase, int2 pos)
        {
            this.blockBase = blockBase;
            this.pos = pos;
        }

        public BlockBase BlockBase => blockBase;

        public int2 Pos => pos;
    }
}