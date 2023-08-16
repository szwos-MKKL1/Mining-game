using System.Collections;
using System.Collections.Generic;
using Terrain.Blocks;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.Outputs
{
    public struct BlockCollector : IPositionCollector<BlockPos>
    {
        private readonly PositionCollector<BlockPos> positionCollector;

        public BlockCollector(int2 min, int2 max, Allocator allocator)
        {
            positionCollector = new PositionCollector<BlockPos>(min, max, allocator);
        }
        

        public IEnumerator<ValuePosition<BlockPos>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int2 Min => positionCollector.Min;
        public int2 Max => positionCollector.Max;
        public void Set(BlockPos t, int2 pos)
        {
            throw new System.NotImplementedException();
        }

        public BlockPos At(int2 pos)
        {
            throw new System.NotImplementedException();
        }
    }
}