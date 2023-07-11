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
            int sizexy = size.x * size.y;
            NativeArray<byte> layerIdMap = new NativeArray<byte>(sizexy, Allocator.Persistent);
            //Fill array with default value of 255
            for (int i = 0; i < sizexy; i++) layerIdMap[i] = 255;

            int nodeCount = nodes.Length;
            NativeArray<int2> nodesArray = new(nodeCount, Allocator.TempJob);
            for (int i = 0; i < nodeCount; i++)
            {
                Vector2 pos = nodes[i].Pos;
                nodesArray[i] = new int2((int)pos.x, (int)pos.y);
            }

            NativeArray<LayerSettings> layersArray = new NativeArray<LayerSettings>(layers, Allocator.TempJob);

            FillLayerMapJob fillLayerMapJob = new FillLayerMapJob()
            {
                layerIdMap = layerIdMap,
                layers = layersArray,
                layerCount = layers.Length,
                mapSize = size,
                mapSize1D = sizexy,
                nodes = nodesArray
            };
            var startTime = Time.realtimeSinceStartup;
            fillLayerMapJob.Schedule(nodeCount, 64).Complete();
            Debug.Log($"Complete took {Time.realtimeSinceStartup-startTime}s");
            nodesArray.Dispose();
            layersArray.Dispose();
            
            
            ImageDebug.SaveImg(fillLayerMapJob.Result.ToArray(), new Vector2Int(size.x, size.y), "layers.png", 10);
            layerIdMap.Dispose();
            return null;
        }
    }

    public struct FillLayerMapJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<byte> layerIdMap; //default to 255
        
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
                GenerateCircle(nodePos, layer.radius, (byte)layerId);
            }
        }

        //TODO could be optimized by saving where layers in current node were already generated, starting from layer 0 to layer 1,2..
        private void GenerateCircle(int2 center, int radius, byte val)
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
                    if (val < oldval) //If there is already layer on current grid node with higher priority, then skip
                    {
                        layerIdMap[index] = val;
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