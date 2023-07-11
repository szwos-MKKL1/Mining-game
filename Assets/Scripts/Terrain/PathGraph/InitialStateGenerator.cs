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
        private LayerSettings[] layers;
        private GraphNode[] nodes;
        private int2 size;

        public InitialStateGenerator(Vector2Int size, IEnumerable<GraphNode> nodes, IEnumerable<LayerSettings> layerSettings)
        {
            this.size = new int2(size.x, size.y);
            this.nodes = nodes.ToArray();
            this.layers = layerSettings.ToArray();
        }

        public bool[] GetInitialMap(int seed=0)
        {
            var startTime = Time.realtimeSinceStartup;
            NativeArray<byte> layerMap = GenerateLayerMap();
            //ImageDebug.SaveImg(layerMap.Result.ToArray(), new Vector2Int(size.x, size.y), "layers.png", 10);
            System.Random random = new System.Random(seed);
            int sizexy = size.x * size.y;
            bool[] aliveMap = new bool[sizexy];
            for (int i = 0; i < sizexy; i++)
            {
                int layerId = layerMap[i]-1;
                if (layerId < 0) continue;
                byte perc = layers[layerId].percentChance;
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
        [BurstCompile]
        private NativeArray<byte> GenerateLayerMap()
        {
            int sizexy = size.x * size.y;
            int nodeCount = nodes.Length;
            
            NativeArray<byte> layerIdMap = new(sizexy, Allocator.Persistent);
            NativeArray<int2> nodesArray = new(nodeCount, Allocator.TempJob);
            for (int i = 0; i < nodeCount; i++)
            {
                Vector2 pos = nodes[i].Pos;
                nodesArray[i] = new int2((int)pos.x, (int)pos.y);
            }
            
            NativeArray<LayerSettings> layersArray = new(layers, Allocator.TempJob);
            FillLayerMapJob fillLayerMapJob = new FillLayerMapJob
            {
                layerIdMap = layerIdMap,
                layers = layersArray,
                layerCount = layers.Length,
                mapSize = size,
                mapSize1D = sizexy,
                nodes = nodesArray
            };
            fillLayerMapJob.Schedule(nodeCount, 8).Complete();
            nodesArray.Dispose();
            layersArray.Dispose();
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

    [BurstCompile]
    public struct FillLayerMapJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<byte> layerIdMap; // values 0 = no layer, 1 = layer with id 0, 2 = layer 1...
        
        [ReadOnly] public NativeArray<LayerSettings> layers;
        [ReadOnly] public int layerCount;
        [ReadOnly] public int2 mapSize;
        [ReadOnly] public int mapSize1D;
        [ReadOnly] public NativeArray<int2> nodes;
        public void Execute(int index)
        {
            int2 nodePos = nodes[index];
            //Get layers
            for (int layerId = layerCount-1; layerId >= 0; layerId--)
            {
                LayerSettings layer = layers[layerId];
                //Generate circle
                GenerateCircle(nodePos, layer.radius, (byte)(layerId+1)); //Layer id in layerMap should be 1 larger than it's value in layers array
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
    
    

    public struct LayerSettings
    {
        public int radius;
        public byte percentChance;

        public LayerSettings(int radius, byte percentChance)
        {
            this.radius = radius;
            this.percentChance = percentChance;
        }
    }
}