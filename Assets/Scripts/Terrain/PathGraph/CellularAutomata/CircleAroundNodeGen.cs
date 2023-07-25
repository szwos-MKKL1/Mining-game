using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.PathGraph.CellularAutomata
{
    public class CircleAroundNodeGen : ILayerGenerator
    {
        private IEnumerable<GeneratorNode> nodes;

        public CircleAroundNodeGen(IEnumerable<GeneratorNode> nodes)
        {
            this.nodes = nodes;
        }

        public void GenerateLayer(NativeArray<byte> baseMap, Layer[] layers, int2 mapSize)
        {
            int sizexy = mapSize.x * mapSize.y;
            int nodeCount = nodes.Count();
            int layerCount = layers.Length;
            
            NativeArray<NodePosAndLayerCount> nodesArray = new(nodeCount, Allocator.TempJob);
            NativeArray<LayerGenerationSettings> layerGenArray = new(layerCount*nodeCount, Allocator.TempJob);
            //Saves generation layers for each node, access layer by (nodeIndex * layerCount + layerIndex)
            
            
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
                layerIdMap = baseMap,
                layerGenSettingsForNodes = layerGenArray,
                layerCount = layers.Length,
                mapSize = mapSize,
                mapSize1D = sizexy,
                nodes = nodesArray
            };
            fillLayerMapJob.Schedule(nodeCount, 8).Complete();
            nodesArray.Dispose();
            layerGenArray.Dispose();
        }
    }
    
    public struct GeneratorNode
    {
        private Vector2Int pos;
        private LayerGenerationSettings[] layers;
        public GeneratorNode(Vector2Int pos, [NotNull] LayerGenerationSettings[] layers)
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