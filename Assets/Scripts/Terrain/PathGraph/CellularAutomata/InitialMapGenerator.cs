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
    //TODO For now it works ok, but user has no way to expand uppon this algorithm with new layer shapes.
    //     One solution is to separate creation of layer map from generation of randomized map based on this layer map 
    //     This way user could provide layer map from any source
    //     To make this process easier we could create layer map in multiple passes, eg.:
    //      - Generation of circles around nodes
    //      - Generation of corridors between nodes
    public class InitialMapGenerator
    {
        private int2 size;
        private IEnumerable<GeneratorNode> nodes;
        private Layer[] layers;
        
        public InitialMapGenerator(Vector2Int size, IEnumerable<GeneratorNode> nodes, Layer[] layers)
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
            NativeArray<InitialMapJobs.NodePosAndLayerCount> nodesArray = new(nodeCount, Allocator.TempJob);
            
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
                
                nodesArray[i] = new InitialMapJobs.NodePosAndLayerCount(pos, (byte)nodeLayerCount);
                i++;
            }
            
            InitialMapJobs.FillLayerMapJob fillLayerMapJob = new InitialMapJobs.FillLayerMapJob
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

    public struct Layer
    {
        public byte aliveChance;

        public Layer(byte aliveChance)
        {
            this.aliveChance = aliveChance;
        }
    }
}