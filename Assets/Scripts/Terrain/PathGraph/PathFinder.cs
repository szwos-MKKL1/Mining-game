using System;
using System.Collections.Generic;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;
using Terrain.PathGraph.Graphs;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace Terrain.PathGraph
{
    public class PathFinder
    {
        private readonly BidirectionalGraph<Vector2, IEdge<Vector2>> graph;
        private readonly Func<Vector2, int> weightFunc;
        private readonly Vector2 startNode;
        private readonly Vector2 destinationNode;
        private readonly Random mRandom;
        private readonly float reversedSizeSquared; // 1/przekątna prostokąta świata
        private readonly PathFindingSettings pathFindingSettings;

        public PathFinder(
            BidirectionalGraph<Vector2, IEdge<Vector2>> graph, 
            Func<Vector2, int> weightFunc, 
            Vector2Int startPos, 
            Vector2Int destinationPos, 
            Vector2Int size, 
            PathFindingSettings pathFindingSettings, 
            int seed = 0)
        {
            this.graph = graph;
            this.weightFunc = weightFunc;
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
            List<IEdge<Vector2>> edges = new();
            IEnumerable<Vector2> nodes = NextRandomNodesPath();
            if (nodes == null) return null;
            using IEnumerator<Vector2> nodeEnumerator = nodes.GetEnumerator();
            if(!nodeEnumerator.MoveNext()) return null;
            Vector2 p1 = nodeEnumerator.Current;
            while (nodeEnumerator.MoveNext())
            {
                Vector2 p2 = nodeEnumerator.Current;
                edges.Add(new Edge<Vector2>(p1, p2));
                p1 = p2;
            }
            return edges;
        }
        
        public IEnumerable<Vector2> NextRandomNodesPath()
        {
            Dictionary<Vector2, PathNode> visited = new();
            C5.IntervalHeap<PathNode> openSet = new();
            
            PathNode currentNode = new PathNode(0, startNode, null);
            openSet.Add(currentNode);

            while (!openSet.IsEmpty)
            {
                currentNode = openSet.DeleteMin();
                visited.Add(currentNode.Current, currentNode);
                foreach (IEdge<Vector2> neighbourEdge in graph.OutEdges(currentNode.Current))
                {
                    Vector2 neighbour = neighbourEdge.Target;
                    if (visited.ContainsKey(neighbour)) continue;
                    if (neighbour == destinationNode)
                    {
                        //Reconstruct path
                        LinkedList<Vector2> pathList = new();
                        pathList.AddFirst(destinationNode);
                        PathNode next = currentNode;
                        while (next.Current != startNode)
                        {
                            pathList.AddFirst(next.Current);
                            next = next.CameFrom;
                        }
                        pathList.AddFirst(startNode);
                        return pathList;
                    }
                    int randVal = mRandom.Next(pathFindingSettings.RandomCostMin, pathFindingSettings.RandomCostMax);
                    float randomH = calcH(neighbour, destinationNode);
                    openSet.Add(new PathNode(
                        (int)(
                            currentNode.CostSoFar + 
                            randVal + 
                            randomH * 100 * pathFindingSettings.DistanceMultiplier + 
                            weightFunc(neighbour) * pathFindingSettings.weightMultiplier),
                        neighbour, 
                        currentNode));
                }
            }
            return null;
        }

        private float calcH(Vector2 currentNode, Vector2 destNode)
        {
            return DistanceMethods.SqrtEuclidianDistance(currentNode.ToVectorInt(), destNode.ToVectorInt())*reversedSizeSquared;
        }
    }

    internal class PathNode : IComparable<int>
    {
        public PathNode(int costSoFar, Vector2 current, PathNode cameFrom)
        {
            CostSoFar = costSoFar;
            Current = current;
            CameFrom = cameFrom;
        }

        public int CostSoFar { get; }
        public Vector2 Current { get; }
        public PathNode CameFrom { get; }
        public int CompareTo(int other)
        {
            return CostSoFar.CompareTo(other);
        }
    }

    [Serializable]
    public class PathFindingSettings
    {
        public int randomCostMin = 0;
        public int randomCostMax = 100;
        public float distanceMultiplier = 1f;
        public float weightMultiplier = 1f;

        public int RandomCostMin => randomCostMin;

        public int RandomCostMax => randomCostMax;

        public float DistanceMultiplier => distanceMultiplier;
        public float WeightMultiplier => weightMultiplier;
    }
}