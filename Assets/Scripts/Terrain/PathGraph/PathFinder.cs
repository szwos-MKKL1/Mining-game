using System;
using System.Collections.Generic;
using Terrain.PathGraph.Graphs;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace Terrain.PathGraph
{
    public class PathFinder
    {
        private readonly Graph<PathFinderNode> mGraph;
        private readonly GraphNode<PathFinderNode> startNode;
        private readonly GraphNode<PathFinderNode> destinationNode;
        private readonly Random mRandom;
        private readonly float reversedSizeSquared; // 1/przekątna prostokąta świata
        private readonly PathFindingSettings pathFindingSettings;

        public PathFinder(Graph<PathFinderNode> graph, Vector2Int startPos, Vector2Int destinationPos, Vector2Int size, PathFindingSettings pathFindingSettings, int seed = 0)
        {
            mGraph = graph;
            startNode = FindClosestNode(startPos);
            destinationNode = FindClosestNode(destinationPos);
            if (startNode == null || destinationNode == null) throw new Exception("One of the pathing nodes was null!");
            reversedSizeSquared = 1f / math.sqrt(size.x * size.x + size.y * size.y);
            mRandom = new Random(seed);
            this.pathFindingSettings = pathFindingSettings;
        }

        private GraphNode<PathFinderNode> FindClosestNode(Vector2Int pos)
        {
            using IEnumerator<GraphNode<PathFinderNode>> enumerator = mGraph.GetNodes();
            enumerator.MoveNext();
            GraphNode<PathFinderNode> closestNode = enumerator.Current;
            if (closestNode == null) return null;
            
            float closestDist = DistanceMethods.SqEuclidianDistance(closestNode.Value.Pos, pos);
            while (enumerator.MoveNext())
            {
                GraphNode<PathFinderNode> node = enumerator.Current;
                if (node == null) continue;
                float distance = DistanceMethods.SqEuclidianDistance(node.Value.Pos, pos);
                if (!(distance < closestDist)) continue;
                closestNode = node;
                closestDist = distance;
            }

            return closestNode;
        }
        
        public IEnumerable<GraphNode<PathFinderNode>> NextRandomPath()
        {
            HashSet<GraphNode<PathFinderNode>> visited = new();
            C5.IntervalHeap<PathNode> heap = new();
            
            heap.Add(new PathNode(startNode));
            visited.Add(startNode);
            
            PathNode current;
            while (!heap.IsEmpty)
            {
                current = heap.FindMin();
                heap.DeleteMin();
                foreach (var neighbour in current.currentNode.ConnectedNodes)
                {
                    if (visited.Contains(neighbour)) continue;
                    if (neighbour == destinationNode)
                    {
                        //Reconstruct path
                        LinkedList<GraphNode<PathFinderNode>> pathList = new();
                        pathList.AddFirst(destinationNode);
                        PathNode next = current;
                        while (next.currentNode != startNode)
                        {
                            pathList.AddFirst(next.currentNode);
                            next = next.cameFrom;
                        }

                        pathList.AddFirst(startNode);
                        return pathList;
                    }

                    visited.Add(neighbour);
                    
                    int randVal = mRandom.Next(pathFindingSettings.RandomCostMin, pathFindingSettings.RandomCostMax);
                    float randomh = calcH(neighbour.Value, destinationNode.Value);
                    
                    PathNode neighbourPathNode = new PathNode(neighbour, current.CostSoFar + randVal, (int)(randomh*100*pathFindingSettings.DistanceMultiplier));
                    neighbourPathNode.cameFrom = current;
                    
                    heap.Add(neighbourPathNode);
                }
            }
            return null;
        }

        private float calcH(PathFinderNode currentNode, PathFinderNode destNode)
        {
            return DistanceMethods.SqrtEuclidianDistance(currentNode.Pos.ToVectorInt(), destNode.Pos.ToVectorInt())*reversedSizeSquared;
        }

        private bool RndPercentage(float chance)
        {
            return mRandom.NextDouble() <= chance;
        }
    }

    class PathNode : IComparable<PathNode>
    {
        private int costSoFar;
        private int h;
        public GraphNode<PathFinderNode> currentNode;
        public PathNode cameFrom;
        public PathNode(GraphNode<PathFinderNode> currentNode, int costSoFar=0, int h=0)
        {
            this.currentNode = currentNode;
            this.costSoFar = costSoFar;
            this.h = h;
        }

        public int GraphCost => currentNode.Value.Weight;
        public int CostSoFar => costSoFar;
        public int H => h;
        public int Fcost => CostSoFar + GraphCost + h;

        public int CompareTo(PathNode other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var a = new PathFindingSettings();

            return Fcost.CompareTo(other.Fcost);
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