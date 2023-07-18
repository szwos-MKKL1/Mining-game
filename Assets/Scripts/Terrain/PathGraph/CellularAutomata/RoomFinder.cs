using System.Collections.Generic;
using Unity.Collections;

namespace Terrain.PathGraph.CellularAutomata
{
    public class RoomFinder
    {
        private NativeArray<bool> aliveMap;
        public RoomFinder(NativeArray<bool> aliveMap)
        {
            this.aliveMap = aliveMap;
        }
        //
        // public IEnumerable<>
    }
}