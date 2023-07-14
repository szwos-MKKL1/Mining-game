using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using Terrain.PathGraph.Graphs.Points;
using UnityEngine;

namespace Terrain.PathGraph.Graphs
{
    /**
     * Builds graph by placing vertices(nodes) randomly(approximation of poisson distribution) in given area
     * and then connecting them using Delaunay triangulation
     */
    public class RandomGraph
    {
        private List<GraphNode> nodes = new List<GraphNode>();
        private readonly Delaunator delaunator;
        public RandomGraph(Vector2Int size, int nodeCount, int seed)
        {
            var realtimeSinceStartup = Time.realtimeSinceStartup;
            
            var pointDistribution = new BestCandidatePoints(size, 6, seed);
            
            Debug.Log($"pointDistribution in {Time.realtimeSinceStartup-realtimeSinceStartup}s");
            realtimeSinceStartup = Time.realtimeSinceStartup;
            
            delaunator = new Delaunator(pointDistribution.GetSamples(nodeCount).Select(vec => new Point(vec.x, vec.y)).Cast<IPoint>().ToArray());
            
            Debug.Log($"delaunator in {Time.realtimeSinceStartup-realtimeSinceStartup}s");
        }

        //TODO point distribution as parameter
        public RandomGraph(Graph graph)
        {
            var realtimeSinceStartup = Time.realtimeSinceStartup;

            var pointDistribution = new PointsAroundGraph(graph, new RangeInt(1, 4), 10);
            
            Debug.Log($"pointDistribution in {Time.realtimeSinceStartup-realtimeSinceStartup}s");
            realtimeSinceStartup = Time.realtimeSinceStartup;
            
            delaunator = new Delaunator(pointDistribution.GetSamples().Select(vec => new Point(vec.x, vec.y)).Cast<IPoint>().ToArray());
            
            Debug.Log($"delaunator2 in {Time.realtimeSinceStartup-realtimeSinceStartup}s");
        }

        public Graph GetGraph()
        {
            return new Graph(GetNodesSet());
        }

        public HashSet<GraphNode> GetNodesSet()
        {
            HashSet<GraphNode> hashSet = new HashSet<GraphNode>();
            foreach (var node in GetNodes())
            {
                hashSet.Add(node);
            }

            return hashSet;
        }

        public IEnumerable<GraphNode> GetNodes()
        {
            Dictionary<IPoint, GraphNode> pointDictionary = new Dictionary<IPoint, GraphNode>();
            foreach (var edge in delaunator.GetEdges())
            {
                IPoint p = edge.P;
                IPoint q = edge.Q;


                GraphNode pnode = GetOrAddNode(pointDictionary, p);
                GraphNode qnode = GetOrAddNode(pointDictionary, q);

                pnode.AddConnection(qnode);
                qnode.AddConnection(pnode);
            }

            return pointDictionary.Values;
        }

        //Exposes IEdge from DelaunatorSharp
        public IEnumerable<IEdge> GetEdges()
        {
            return delaunator.GetEdges();
        }

        private static GraphNode GetOrAddNode(IDictionary<IPoint, GraphNode> dict, IPoint point)
        {
            GraphNode node;
            if (!dict.ContainsKey(point))
            {
                node = new GraphNode(new Vector2((float)point.X, (float)point.Y));
                dict.Add(point, node);
            }
            else node = dict[point];

            return node;
        }

    }
    
    
    
}