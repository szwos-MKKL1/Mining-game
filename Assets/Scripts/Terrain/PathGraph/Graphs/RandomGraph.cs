using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using Terrain.PathGraph.Graphs.Points;
using UnityEngine;

namespace Terrain.PathGraph.Graphs
{
    public delegate T CreateNodeValue<out T>(Vector2 pos);
    /**
     * Builds graph by placing vertices(nodes) randomly(approximation of poisson distribution) in given area
     * and then connecting them using Delaunay triangulation
     */
    public class RandomGraph<T>
    {
        private CreateNodeValue<T> constructor;
        private Delaunator delaunator;

        private RandomGraph()
        {
            
        }
        public static RandomGraph<T> CreateFromPoints(CreateNodeValue<T> constructor, Vector2Int size, int nodeCount, int seed = 0)
        {
            RandomGraph<T> randomGraph = new();
            randomGraph.constructor = constructor;
            
            var pointDistribution = new BestCandidatePoints(size, 6, seed);
            pointDistribution.SampleCount = nodeCount;
            
            randomGraph.delaunator = new Delaunator(pointDistribution.GetSamples().Select(vec => new Point(vec.x, vec.y)).Cast<IPoint>().ToArray());
            return randomGraph;
        }
        
        public static RandomGraph<T> CreateFromPoints(CreateNodeValue<T> constructor, IPointGenerator pointGenerator)
        {
            return new RandomGraph<T>
            {
                constructor = constructor,
                delaunator = new Delaunator(pointGenerator.GetSamples().Select(vec => new Point(vec.x, vec.y)).Cast<IPoint>().ToArray())
            };
        }
        
        //TODO point distribution as parameter
        public static RandomGraph<T> CreateAroundGraph<V>(Graph<V> graph, CreateNodeValue<T> constructor, int seed = 0) where V : PosGraphNode
        {
            return CreateFromPoints(constructor,new PointsAroundGraph<V>(graph, new RangeInt(1, 4), 10f, seed));
        }

        public Graph<T> GetGraph()
        {
            return new Graph<T>(GetNodesSet());
        }

        public HashSet<GraphNode<T>> GetNodesSet()
        {
            HashSet<GraphNode<T>> hashSet = new();
            foreach (GraphNode<T> node in GetNodes())
            {
                hashSet.Add(node);
            }

            return hashSet;
        }

        public IEnumerable<GraphNode<T>> GetNodes()
        {
            Dictionary<IPoint, GraphNode<T>> pointDictionary = new();
            foreach (var edge in delaunator.GetEdges())
            {
                IPoint p = edge.P;
                IPoint q = edge.Q;


                GraphNode<T> pnode = GetOrAddNode(pointDictionary, p);
                GraphNode<T> qnode = GetOrAddNode(pointDictionary, q);

                pnode.ConnectedNodes.Add(qnode);
                qnode.ConnectedNodes.Add(pnode);
            }

            return pointDictionary.Values;
        }

        //Exposes IEdge from DelaunatorSharp
        public IEnumerable<IEdge> GetEdges()
        {
            return delaunator.GetEdges();
        }

        private GraphNode<T> GetOrAddNode(IDictionary<IPoint, GraphNode<T>> dict, IPoint point)
        {
            GraphNode<T> node;
            if (!dict.ContainsKey(point))
            {
                node = new GraphNode<T>(constructor(new Vector2((float)point.X, (float)point.Y)));
                dict.Add(point, node);
            }
            else node = dict[point];

            return node;
        }

    }
    
    
    
}