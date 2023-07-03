using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Vector2 = System.Numerics.Vector2;

namespace Terrain.PathGraph
{
    public class CostApplicator
    {
        private CostApplicatorSettings settings;
        private float reversedSize;
        private readonly Dictionary<Vector2Int, byte> dict;
        private readonly Random mRandom;

        public CostApplicator(CostApplicatorSettings settings, Dictionary<Vector2Int, byte> dict, int seed = 0)
        {
            this.settings = settings;
            this.dict = dict;
            reversedSize = 1f / (settings.WorldSize.x * settings.WorldSize.y);
            mRandom = new Random(seed);
        }

        public void Apply(GraphNode node)
        {
            Vector2Int nodePos = new Vector2Int((int)node.Pos.x, (int)node.Pos.y);
            dict.TryGetValue(nodePos, out var bc);
            float barrierCost = bc * settings.BorderMultiplier;
            float distanceCost = DistanceMethods.SqEuclidianDistance(nodePos, settings.Destination) * reversedSize *
                                 settings.DistanceMultiplier;
            float randomCost = mRandom.Next(settings.RandomMin, settings.RandomMax);
            node.Cost += (int)(barrierCost + distanceCost + randomCost);
            Debug.Log($"{node} barrierCost:{barrierCost} distanceCost:{distanceCost} randomCost:{randomCost}");
        }

        public void Apply(IEnumerable<GraphNode> graph)
        {
            foreach (var node in graph)
            {
                Apply(node);
            }
        }
    }

    public class CostApplicatorSettings
    {
        private float borderMultiplier = 1f;
        private float distanceMultiplier = 1f;
        private int randomMin = 0;
        private int randomMax = 100;
        private Vector2Int worldSize;
        private Vector2Int destination;

        public CostApplicatorSettings(Vector2Int worldSize)
        {
            this.worldSize = worldSize;
        }

        public float BorderMultiplier
        {
            get => borderMultiplier;
            set => borderMultiplier = value;
        }

        public float DistanceMultiplier
        {
            get => distanceMultiplier;
            set => distanceMultiplier = value;
        }

        public int RandomMin
        {
            get => randomMin;
            set => randomMin = value;
        }

        public int RandomMax
        {
            get => randomMax;
            set => randomMax = value;
        }

        public Vector2Int WorldSize
        {
            get => worldSize;
        }

        public Vector2Int Destination
        {
            get => destination;
            set => destination = value;
        }
    }
}