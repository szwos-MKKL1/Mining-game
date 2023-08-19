using System.Runtime.CompilerServices;
using Terrain.Blocks;
using Terrain.Outputs;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain
{
    public readonly struct BlockPos : IPosHolder
    {
        private readonly BlockBase blockBase;
        private readonly int2 pos;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos(BlockBase blockBase, int2 pos)
        {
            this.blockBase = blockBase;
            this.pos = pos;
        }

        public BlockBase BlockBase => blockBase;

        public int2 Pos => pos;
    }
}