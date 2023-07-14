using System;
using System.Collections.Generic;
using System.Linq;
using InternalDebug;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.PathGraph
{
    public class InitialStateGenerator
    {
        private int2 size;
        private IEnumerable<GeneratorNode> nodes;
        private Layer[] layers;
        
        public InitialStateGenerator(Vector2Int size, IEnumerable<GeneratorNode> nodes, Layer[] layers)
        {
            this.size = new int2(size.x, size.y);
            this.nodes = nodes;
            this.layers = layers;
        }

        public bool[] GetInitialMap(int seed=0)
        {
            NativeArray<byte> layerMap = GenerateLayerMap();
            System.Random random = new System.Random(seed);
            int sizexy = size.x * size.y;
            bool[] aliveMap = new bool[sizexy];
            for (int i = 0; i < sizexy; i++)
            {
                int layerId = layerMap[i]-1;
                if (layerId < 0) continue;
                byte perc = layers[layerId].aliveChance;
                aliveMap[i] = perc switch
                {
                    0 => false,
                    >= 100 => true,
                    _ => random.Next(0, 100) <= perc
                };

            }

            layerMap.Dispose();
            return aliveMap;
        }
        
        private NativeArray<byte> GenerateLayerMap()
        {
            int sizexy = size.x * size.y;
            int nodeCount = nodes.Count();
            int layerCount = layers.Length;
            
            NativeArray<byte> layerIdMap = new(sizexy, Allocator.Persistent);
            NativeArray<NodePosAndLayerCount> nodesArray = new(nodeCount, Allocator.TempJob);
            
            //Saves generation layers for each node, access layer by (nodeIndex * layerCount + layerIndex)
            NativeArray<LayerGenerationSettings> layerGenArray = new(layerCount*nodeCount, Allocator.TempJob);
            
            int i = 0;
            foreach (var p in nodes)
            {
                int2 pos = new int2(p.Pos.x, p.Pos.y);
                LayerGenerationSettings[] nodeLayers = p.Layers;
                int nodeLayerCount = nodeLayers.Length;
                
                if (nodeLayerCount > layerCount)
                    throw new Exception("GeneratorNode cannot have more layers than defined");
                
                for (int j = 0; j < nodeLayerCount; j++)
                {
                    layerGenArray[i * layerCount + j] = nodeLayers[j];
                }
                
                nodesArray[i] = new NodePosAndLayerCount(pos, (byte)nodeLayerCount);
                i++;
            }
            
            FillLayerMapJob fillLayerMapJob = new FillLayerMapJob
            {
                layerIdMap = layerIdMap,
                layerGenSettingsForNodes = layerGenArray,
                layerCount = layers.Length,
                mapSize = size,
                mapSize1D = sizexy,
                nodes = nodesArray
            };
            fillLayerMapJob.Schedule(nodeCount, 8).Complete();
            nodesArray.Dispose();
            layerGenArray.Dispose();
            return layerIdMap;
        }
    }

    //TODO works but not used here
    // public struct FillNativeArray<T> : IJobParallelFor where T : struct
    // {
    //     private NativeArray<T> array;
    //     private T value;
    //
    //     public FillNativeArray(NativeArray<T> array, T value)
    //     {
    //         this.array = array;
    //         this.value = value;
    //     }
    //
    //     public void Execute(int index)
    //     {
    //         array[index] = value;
    //     }
    // }


    public struct GeneratorNode
    {
        private Vector2Int pos;
        private LayerGenerationSettings[] layers;
        public GeneratorNode(Vector2Int pos, LayerGenerationSettings[] layers)
        {
            this.pos = pos;
            this.layers = layers;
        }
        public Vector2Int Pos => pos;
        public LayerGenerationSettings[] Layers => layers;
    }
    
    public struct LayerGenerationSettings
    {
        public short radius;
        public byte layerID;//Could also be pointer to Layer struct, but pointer takes 8 bytes while this solution takes only 1 byte

        public LayerGenerationSettings(short radius, byte layerID)
        {
            this.radius = radius;
            this.layerID = layerID;
        }
    }

    public struct Layer
    {
        public byte aliveChance;

        public Layer(byte aliveChance)
        {
            this.aliveChance = aliveChance;
        }
    }
    
    //
    //
    // BURST
    //
    //

    //Used to save how many layers each nodes has to generate
    //It is required because node can have less generation layers than other nodes
    internal struct NodePosAndLayerCount
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
    internal struct FillLayerMapJob : IJobParallelFor
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