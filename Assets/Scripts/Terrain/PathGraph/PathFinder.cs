using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Terrain.PathGraph
{
    public class PathFinder
    {
        private Graph mGraph;
        private GraphNode startNode;
        private GraphNode destinationNode;
        private Random mRandom;

        public PathFinder(Graph graph, Vector2Int startPos, Vector2Int destinationPos, int seed = 0)
        {
            mGraph = graph;
            startNode = FindClosestNode(startPos);
            destinationNode = FindClosestNode(destinationPos);
            mRandom = new Random(seed);
        }

        private GraphNode FindClosestNode(Vector2Int pos)
        {
            using IEnumerator<GraphNode> enumerator = mGraph.GetEnumerator();
            enumerator.MoveNext();
            GraphNode closestNode = enumerator.Current;
            float closestDist = DistanceMethods.SqEuclidianDistance(closestNode.Pos, pos);
            while (enumerator.MoveNext())
            {
                GraphNode node = enumerator.Current;
                float distance = DistanceMethods.SqEuclidianDistance(closestNode.Pos, pos);
                if (!(distance < closestDist)) continue;
                closestNode = node;
                closestDist = distance;
            }

            return closestNode;
        }

        public Path NextRandomPath()
        {
            return null;
        }
    }
}