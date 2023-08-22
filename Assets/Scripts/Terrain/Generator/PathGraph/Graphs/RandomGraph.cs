using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using QuikGraph;
using Terrain.Generator.PathGraph.Graphs.Points;
using UnityEngine;

namespace Terrain.Generator.PathGraph.Graphs
{
    /**
     * Builds graph by placing vertices(nodes) randomly(approximation of poisson distribution) in given area
     * and then connecting them using Delaunay triangulation
     */
    public class RandomGraph
    {
        private DelaunatorGraph<Vector2> delaunatorGraph;

        private RandomGraph()
        {
            
        }
        public static RandomGraph CreateFromPoints(Vector2Int size, int nodeCount, int seed = 0)
        {
            RandomGraph randomGraph = new();
            var pointDistribution = new BestCandidatePoints(size, 6, seed);
            pointDistribution.SampleCount = nodeCount;

            randomGraph.delaunatorGraph = new DelaunatorGraph<Vector2>(pointDistribution.GetSamples(), vector2 => vector2.AsIPoint());
            return randomGraph;
        }
        
        public static RandomGraph CreateFromPoints(IPointGenerator pointGenerator)
        {
            return new RandomGraph
            {
                delaunatorGraph = new DelaunatorGraph<Vector2>(pointGenerator.GetSamples(),vector2 => vector2.AsIPoint())
            };
        }
        
        //TODO point distribution as parameter
        public static RandomGraph CreateAroundEdges(IEnumerable<IEdge<Vector2>> edges, int seed = 0)
        {
            return CreateFromPoints(new PointsAroundGraph(edges, new RangeInt(1, 4), 1f, seed));
        }

        public UndirectedGraph<Vector2, IEdge<Vector2>> GetGraph()
        {
            return GetEdges().ToUndirectedGraph<Vector2, IEdge<Vector2>>();
        }
        
        public IEnumerable<IEdge<Vector2>> GetEdges()
        {
            return delaunatorGraph.GetEdges();
        }
    }
    
    
    
}