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
    public class InitialMapGenerator
    {
        private IEnumerable<ILayerGenerator> generators;
        private int2 size;
        private Layer[] layers;
        
        public InitialMapGenerator(Vector2Int size, Layer[] layers, IEnumerable<ILayerGenerator> generators)
        {
            this.size = new int2(size.x, size.y);
            this.layers = layers;
            this.generators = generators;
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
            NativeArray<byte> baseMap = new(size.x*size.y, Allocator.Persistent);
            foreach (var gen in generators)
            {
                gen.GenerateLayer(baseMap, layers, size);
            }

            return baseMap;
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