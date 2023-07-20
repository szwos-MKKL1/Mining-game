using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using QuikGraph;
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
        private Delaunator delaunator;

        private RandomGraph()
        {
            
        }
        public static RandomGraph CreateFromPoints(Vector2Int size, int nodeCount, int seed = 0)
        {
            RandomGraph randomGraph = new();
            var pointDistribution = new BestCandidatePoints(size, 6, seed);
            pointDistribution.SampleCount = nodeCount;
            
            randomGraph.delaunator = new Delaunator(pointDistribution.GetSamples().Select(vec => new Point(vec.x, vec.y)).Cast<IPoint>().ToArray());
            return randomGraph;
        }
        
        public static RandomGraph CreateFromPoints(IPointGenerator pointGenerator)
        {
            return new RandomGraph
            {
                delaunator = new Delaunator(pointGenerator.GetSamples().Select(vec => new Point(vec.x, vec.y)).Cast<IPoint>().ToArray())
            };
        }
        
        //TODO point distribution as parameter
        public static RandomGraph CreateAroundEdges(IEnumerable<IEdge<Vector2>> edges, int seed = 0)
        {
            return CreateFromPoints(new PointsAroundGraph(edges, new RangeInt(1, 4), 10f, seed));
        }

        public BidirectionalGraph<Vector2, IEdge<Vector2>> GetGraph()
        {
            BidirectionalGraph<Vector2, IEdge<Vector2>> graph = new();
            graph.AddVerticesAndEdgeRange(GetEdges());
            return graph;
        }

        //Exposes IEdge from DelaunatorSharp
        public IEnumerable<IEdge<Vector2>> GetEdges()
        {
            List<IEdge<Vector2>> edges = new();
            foreach (var edge in delaunator.GetEdges())
            {
                IPoint p = edge.P;
                IPoint q = edge.Q;
                edges.Add(new Edge<Vector2>(new Vector2((float)p.X, (float)p.Y), new Vector2((float)q.X, (float)q.Y)));
            }

            return edges;
        }
    }
    
    
    
}