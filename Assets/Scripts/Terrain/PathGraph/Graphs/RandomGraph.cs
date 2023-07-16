using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using Terrain.PathGraph.Graphs.Points;
using UnityEngine;

namespace Terrain.PathGraph.Graphs
{
    public delegate T Create<T>(Vector2 pos) where T : GraphNode;
    /**
     * Builds graph by placing vertices(nodes) randomly(approximation of poisson distribution) in given area
     * and then connecting them using Delaunay triangulation
     */
    public class RandomGraph<T> where T : GraphNode
    {
        private List<T> nodes = new List<T>();
        private Create<T> constructor;
        private Delaunator delaunator;

        private RandomGraph()
        {
            
        }
        public static RandomGraph<T> CreateFromPoints(Create<T> constructor, Vector2Int size, int nodeCount, int seed = 0)
        {
            RandomGraph<T> randomGraph = new();
            randomGraph.constructor = constructor;
            
            var pointDistribution = new BestCandidatePoints(size, 6, seed);
            pointDistribution.SampleCount = nodeCount;
            
            randomGraph.delaunator = new Delaunator(pointDistribution.GetSamples().Select(vec => new Point(vec.x, vec.y)).Cast<IPoint>().ToArray());
            return randomGraph;
        }
        
        public static RandomGraph<T> CreateFromPoints(Create<T> constructor, IPointGenerator pointGenerator)
        {
            return new RandomGraph<T>
            {
                constructor = constructor,
                delaunator = new Delaunator(pointGenerator.GetSamples().Select(vec => new Point(vec.x, vec.y)).Cast<IPoint>().ToArray())
            };
        }
        
        //TODO point distribution as parameter
        public static RandomGraph<T> CreateAroundGraph<V>(Graph<V> graph, Create<T> constructor, int seed = 0) where V : PosGraphNode
        {
            return CreateFromPoints(constructor,new PointsAroundGraph<V>(graph, new RangeInt(1, 4), 10f, seed));
        }

        public Graph<T> GetGraph()
        {
            return new Graph<T>(GetNodesSet());
        }

        public HashSet<T> GetNodesSet()
        {
            HashSet<T> hashSet = new HashSet<T>();
            foreach (var node in GetNodes())
            {
                hashSet.Add(node);
            }

            return hashSet;
        }

        public IEnumerable<T> GetNodes()
        {
            Dictionary<IPoint, T> pointDictionary = new Dictionary<IPoint, T>();
            foreach (var edge in delaunator.GetEdges())
            {
                IPoint p = edge.P;
                IPoint q = edge.Q;


                T pnode = GetOrAddNode(pointDictionary, p);
                T qnode = GetOrAddNode(pointDictionary, q);

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

        private T GetOrAddNode(IDictionary<IPoint, T> dict, IPoint point)
        {
            T node;
            if (!dict.ContainsKey(point))
            {
                node = constructor(new Vector2((float)point.X, (float)point.Y));
                dict.Add(point, node);
            }
            else node = dict[point];

            return node;
        }

    }
    
    
    
}