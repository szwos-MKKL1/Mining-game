using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Terrain.PathGraph.CellularAutomata
{
    public class InitialMapJobs
    {
        //Used to save how many layers each nodes has to generate
        //It is required because node can have less generation layers than other nodes
        public struct NodePosAndLayerCount
        {
            public int2 pos;
            public byte layerCount;

            public NodePosAndLayerCount(int2 pos, byte layerCount)
            {
                this.pos = pos;
                this.layerCount = layerCount;
            }
        }

        [BurstCompile]
        public struct FillLayerMapJob : IJobParallelFor
        {
            [NativeDisableParallelForRestriction]
            public NativeArray<byte> layerIdMap; // values 0 = no layer, 1 = layer with id 0, 2 = layer 1...
            
            //Get generation layer for node by nodeIndex*layerCount+layerIndex
            [ReadOnly] public NativeArray<LayerGenerationSettings> layerGenSettingsForNodes;
            [ReadOnly] public NativeArray<NodePosAndLayerCount> nodes;
            [ReadOnly] public int2 mapSize;
            [ReadOnly] public int mapSize1D;
            [ReadOnly] public int layerCount;
            public void Execute(int index)
            {
                NodePosAndLayerCount n = nodes[index];
                int2 nodePos = n.pos;
                int baseLayer = layerCount * index;
                //Get generation layers bottom-up
                for (int layerOffset = n.layerCount-1; layerOffset >= 0 ; layerOffset--)
                {
                    LayerGenerationSettings layer = layerGenSettingsForNodes[baseLayer + layerOffset];
                    //Generate circle
                    GenerateCircle(nodePos, layer.radius, (byte)(layer.layerID+1)); //Layer id in layerMap should be 1 larger than it's value in layers array
                }
            }

            //TODO could be optimized by saving where layers in current node were already generated, starting from layer 0 to layer 1,2..
            private void GenerateCircle(int2 center, int radius, byte layerId)
            {
                int top = center.y + radius;
                int bottom = center.y - radius;
                int left = center.x - radius;
                int right = center.x + radius;
                int sqradius = radius * radius;
                //TODO could be further optimized using better circle generator
                //Check all nodes inside square bounds that contain circle
                for (int x = left; x <= right; x++)
                {
                    for (int y = bottom; y <= top; y++)
                    {
                        int index = y * mapSize.x + x;
                        if(index < 0 || index > mapSize1D) continue; //Out of bounds
                        if (squared(x - center.x) + squared(y - center.y) > sqradius) continue;
                        //Is inside circle
                        
                        byte oldval = layerIdMap[index];
                        if (oldval == 0 || layerId < oldval) //If there is already layer on current grid node with higher priority, then skip
                        {
                            layerIdMap[index] = layerId;
                        }
                    }
                }
            }

            private int squared(int v) => v * v;

            public NativeArray<byte> Result => layerIdMap;
        }
    }
}