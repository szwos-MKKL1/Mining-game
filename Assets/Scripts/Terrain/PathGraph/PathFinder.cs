using System;
using System.Collections.Generic;
using QuikGraph;
using QuikGraph.Algorithms;
using Terrain.PathGraph.Graphs;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace Terrain.PathGraph
{
    public class PathFinder
    {
        private readonly BidirectionalGraph<Vector2, IEdge<Vector2>> graph;
        private readonly Dictionary<Vector2, float> weight;
        private readonly Vector2 startNode;
        private readonly Vector2 destinationNode;
        private readonly Random mRandom;
        private readonly float reversedSizeSquared; // 1/przekątna prostokąta świata
        private readonly PathFindingSettings pathFindingSettings;

        public PathFinder(
            BidirectionalGraph<Vector2, IEdge<Vector2>> graph, 
            Dictionary<Vector2, float> weight, 
            Vector2Int startPos, 
            Vector2Int destinationPos, 
            Vector2Int size, 
            PathFindingSettings pathFindingSettings, 
            int seed = 0)
        {
            this.graph = graph;
            this.weight = weight;
            startNode = FindClosestNode(startPos);
            destinationNode = FindClosestNode(destinationPos);
            if (startNode == null || destinationNode == null) throw new Exception("One of the pathing nodes was null!");
            reversedSizeSquared = 1f / math.sqrt(size.x * size.x + size.y * size.y);
            mRandom = new Random(seed);
            this.pathFindingSettings = pathFindingSettings;
        }

        private Vector2 FindClosestNode(Vector2Int pos)
        {
            using IEnumerator<Vector2> enumerator = graph.Vertices.GetEnumerator();
            enumerator.MoveNext();
            Vector2 closestNode = enumerator.Current;

            float closestDist = DistanceMethods.SqEuclidianDistance(closestNode, pos);
            while (enumerator.MoveNext())
            {
                Vector2 node = enumerator.Current;
                float distance = DistanceMethods.SqEuclidianDistance(node, pos);
                if (!(distance < closestDist)) continue;
                closestNode = node;
                closestDist = distance;
            }

            return closestNode;
        }
        
        public IEnumerable<IEdge<Vector2>> NextRandomPath()
        {
            //TODO using built-in method for pathfinding which may not be most optimal
            graph.ShortestPathsAStar(
                edge => weight[edge.Source] + weight[edge.Target], 
                pos =>  + calcH(pos, destinationNode)*100*pathFindingSettings.DistanceMultiplier + mRandom.Next(pathFindingSettings.RandomCostMin, pathFindingSettings.RandomCostMax), 
                startNode)(destinationNode, out IEnumerable<IEdge<Vector2>> result);
            return result;
        }

        private float calcH(Vector2 currentNode, Vector2 destNode)
        {
            return DistanceMethods.SqrtEuclidianDistance(currentNode.ToVectorInt(), destNode.ToVectorInt())*reversedSizeSquared;
        }
    }

    [Serializable]
    public class PathFindingSettings
    {
        public int randomCostMin = 0;
        public int randomCostMax = 100;
        public float distanceMultiplier = 1f;

        public int RandomCostMin => randomCostMin;

        public int RandomCostMax => randomCostMax;

        public float DistanceMultiplier => distanceMultiplier;

        public override string ToString()
        {
            return
                $"randomCostMin: {randomCostMin} randomCostMax: {randomCostMax} distanceMultiplier:{distanceMultiplier}";
        }
    }
}