using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.PathGraph
{
    public class InitialStateGenerator
    {
        private byte[] layerIdMap;
        private LayerSettings[] layers;
        private Vector2Int size;

        public InitialStateGenerator(Vector2Int size, IEnumerable<GraphNode> nodes, IEnumerable<LayerSettings> layerSettings)
        {
            this.size = size;
            this.layers = layerSettings.ToArray();
        }

        public bool[] GetInitialMap()
        {
            
        }
        
        

    }

    [BurstCompile]
    struct FillLayerMapJob : IJobParallelFor
    {
        private byte[] layerIdMap;
        private LayerSettings[] layers;
        private Vector2Int size;
        public void Execute(int index)
        {
            int2 pos = IntToPos(index);
            byte val = 255;
            
            for (int i = layers.Length - 1; i >= 0; i--)
            {
                IBorderShape borderShape = layers[i].borderShape;//TODO fix wrong type in burst
                if (borderShape.IsInsideBorder(pos.x, pos.y)) val = (byte)i;
            }

            layerIdMap[index] = val;
        }
        
        //index = pos.x + pos.y * mapSize.x
        private int2 IntToPos(int index) => new int2(index % size.x, index / size.y);
        private int PosToInt(int2 pos) => pos.x + pos.y * size.x;
        private int PosToInt(int x, int y) => x + y * size.x;
    }

    public struct LayerSettings
    {
        public IBorderShape borderShape;
        public byte percentChance;
    }
}