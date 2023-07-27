using Unity.Collections;
using Unity.Mathematics;

namespace Terrain.PathGraph.CellularAutomata
{
    public interface ILayerGenerator
    {
        public void GenerateLayer(NativeArray<byte> baseMap, Layer[] layers, int2 mapSize);
    }
}