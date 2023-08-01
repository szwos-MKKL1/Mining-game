using System.Collections;
using System.Collections.Generic;
using Terrain.Blocks;
using UnityEngine;

namespace Terrain
{
    public class BlockCollector : IEnumerable<BlockPos>
    {
        private readonly List<BlockPos> list;

        public BlockCollector()
        {
            list = new List<BlockPos>();
        }

        public void Add(BlockPos blockPos)
        {
            list.Add(blockPos);
        }
        
        public void Add(BlockBase blockBase, Vector2Int pos)
        {
            list.Add(new BlockPos(blockBase, pos));
        }

        public IEnumerator<BlockPos> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}