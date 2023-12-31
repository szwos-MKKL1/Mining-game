﻿using System;
using System.Collections.Generic;
using System.Linq;
using QuikGraph;
using Random;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace Terrain.Generator.PathGraph
{
    public class PathFinder
    {
        private readonly UndirectedGraph<Vector2, IEdge<Vector2>> graph;
        private readonly Func<Vector2, int> weightFunc;
        private readonly Vector2 startNode;
        private readonly Vector2 destinationNode;
        private readonly IRandom mRandom;
        private readonly float reversedSizeSquared; // 1/przekątna prostokąta świata
        private readonly PathFindingSettings pathFindingSettings;

        public PathFinder(
            UndirectedGraph<Vector2, IEdge<Vector2>> graph, 
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
            if(startNode == null || destinationNode == null) throw new ArgumentException("One of the pathing nodes was null!");
            if(!this.graph.ContainsVertex(startNode)) throw new ArgumentException("Graph doesn't contain start node");
            if(!this.graph.ContainsVertex(destinationNode)) throw new ArgumentException("Graph doesn't contain destination node");
            
            //TODO not sure if it should throw exception,
            //if startNode is disconnected from main graph then there is not path, but if I don't throw exception then there is no way to know that
            if(!this.graph.AdjacentVertices(startNode).Any()) throw new ArgumentException("Start node has no out edges");
            if(!this.graph.AdjacentVertices(destinationNode).Any()) throw new ArgumentException("Destination node has no out edges");
            
            reversedSizeSquared = 1f / math.sqrt(size.x * size.x + size.y * size.y);
            mRandom = new SystemRandom(seed);//TODO replace with random from terrain generator
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
        
        //TODO save found paths to add cost based on them
        public IEnumerable<Vector2> NextRandomNodesPath()
        {
            Dictionary<Vector2, PathNode> visited = new();
            C5.IntervalHeap<PathNode> openSet = new();
            
            PathNode currentNode = new PathNode(0, startNode, null);
            openSet.Add(currentNode);

            while (!openSet.IsEmpty)
            {
                currentNode = openSet.DeleteMin();
                visited[currentNode.Current] = currentNode;
                foreach (Vector2 neighbour in graph.AdjacentVertices(currentNode.Current))
                {
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
                    int randVal = mRandom.NextInt(pathFindingSettings.RandomCostMin, pathFindingSettings.RandomCostMax);
                    float randomH = calcH(neighbour, destinationNode);

                    openSet.Add(new PathNode(
                        (int)(
                            currentNode.CostSoFar +
                            randVal +
                            math.pow(randomH,2) * 100 * pathFindingSettings.DistanceMultiplier +
                            weightFunc(neighbour) * pathFindingSettings.weightMultiplier),
                        neighbour,
                        currentNode));
                }
            }
            return null;
        }

        private float calcH(Vector2 currentNode, Vector2 destNode)
        {
            return DistanceMethods.SqrtEuclidianDistance(currentNode.AsVectorInt(), destNode.AsVectorInt())*reversedSizeSquared;
        }
    }

    internal class PathNode : IComparable<PathNode>
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
        public int CompareTo(PathNode other)
        {
            return CostSoFar.CompareTo(other.CostSoFar);
        }

        public override string ToString()
        {
            return $"PathNode CostSoFar:{CostSoFar} Current:{Current} CameFrom:{CameFrom}";
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