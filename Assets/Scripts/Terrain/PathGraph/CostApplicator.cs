using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Vector2 = System.Numerics.Vector2;

namespace Terrain.PathGraph
{
    public class CostApplicator
    {
        // private CostApplicatorSettings settings;
        // private float reversedSize;
        // private readonly Dictionary<Vector2Int, byte> dict;
        // private readonly Random mRandom;
        //
        // public CostApplicator(CostApplicatorSettings settings, Dictionary<Vector2Int, byte> dict, int seed = 0)
        // {
        //     this.settings = settings;
        //     this.dict = dict;
        //     reversedSize = 1f / (settings.WorldSize.x * settings.WorldSize.y);
        //     mRandom = new Random(seed);
        // }
        //
        // public void Apply(GraphNode node)
        // {
        //     Vector2Int nodePos = new Vector2Int((int)node.Pos.x, (int)node.Pos.y);
        //     dict.TryGetValue(nodePos, out var bc);
        //     float barrierCost = bc * settings.BorderMultiplier;
        //     float distanceCost = DistanceMethods.SqEuclidianDistance(nodePos, settings.Destination) * reversedSize *
        //                          settings.DistanceMultiplier;
        //     float randomCost = mRandom.Next(settings.RandomMin, settings.RandomMax);
        //     node.Cost += (int)(barrierCost + distanceCost + randomCost);
        //     Debug.Log($"{node} barrierCost:{barrierCost} distanceCost:{distanceCost} randomCost:{randomCost}");
        // }
        //
        // public void Apply(IEnumerable<GraphNode> graph)
        // {
        //     foreach (var node in graph)
        //     {
        //         Apply(node);
        //     }
        // }
    }
}