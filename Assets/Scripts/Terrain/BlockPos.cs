using Terrain.Blocks;
using UnityEngine;

namespace Terrain
{
    public readonly struct BlockPos
    {
        private readonly BlockBase blockBase;
        private readonly Vector2Int pos;

        public BlockPos(BlockBase blockBase, Vector2Int pos)
        {
            this.blockBase = blockBase;
            this.pos = pos;
        }

        public BlockBase BlockBase => blockBase;

        public Vector2Int Pos => pos;
    }
}