using System.Collections.Generic;
using System.Linq;
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
            if (closestNode == null) return null;
            
            float closestDist = DistanceMethods.SqEuclidianDistance(closestNode.Pos, pos);
            while (enumerator.MoveNext())
            {
                GraphNode node = enumerator.Current;
                if (node == null) continue;
                float distance = DistanceMethods.SqEuclidianDistance(node.Pos, pos);
                if (!(distance < closestDist)) continue;
                closestNode = node;
                closestDist = distance;
            }

            return closestNode;
        }

        //This algorithm should go like this:
        //1. Start from "startNode", "currentNode" = "startNode"
        //2. Go to node that has lowest cost and wasn't already visited. Set this node as "nextNode"
        // 2.1 If none available nodes were found set "nextNode" to null
        //3. Randomly choose if should branch.
        //  3.1 If node should branch, choose node that wasn't visited and that isn't "nextNode"
        //  3.2 Choose it's length randomly and add to list "branchingNodes"
        //4. "currentNode".next = "nextNode"
        //5. "currentNode" = "nextNode"
        //6. Go To step until "nextNode"==null
        //7. For each element of "branchingNodes" perform this algorithm from step 1\
        //TODO isn't this basically A*?
        
        //TODO Note to self, instead of saving branches as next nodes, save them as individual paths
        public Path NextRandomPath()
        {
            List<PathNode> branchingNodes = new();
            HashSet<PathNode> allPathNodes = new HashSet<PathNode>();
            GraphNode currentNode = startNode;
            PathNode rootNode = new PathNode(currentNode);
            PathNode currentPathNode = rootNode;
            allPathNodes.Add(rootNode);
            while (true)
            {
                if (currentNode.ConnectedNodes == null || currentNode.ConnectedNodes.Count == 0) break;
                GraphNode nextNode = currentNode.ConnectedNodes.Where(n => !allPathNodes.Contains(n)).MinBy(n => n.Cost);
                
                if (RndPercentage(0.5f))
                {
                    //Branch
                    branchingNodes.Add(new PathNode(currentNode.ConnectedNodes.Where(n => n != nextNode).MinBy(n => n.Cost), 1));
                }

                PathNode nextPathNode = new PathNode(nextNode);
                allPathNodes.Add(nextPathNode);
                currentPathNode.AddNextNode(nextPathNode);
                currentPathNode = nextPathNode;

            }
            return null;
        }

        private bool RndPercentage(float chance)
        {
            return mRandom.NextDouble() <= chance;
        }
    }
}