using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using QuikGraph;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.Generator.PathGraph.Graphs
{
    //Adapter class for delaunator to use unity and quikgraph methods
    public class DelaunatorGraph
    {
        private Delaunator delaunator;
        
        public DelaunatorGraph(IEnumerable<Vector2> points)
        {
            IPoint[] ipoints = points.Select(v => new Point(v.x, v.y)).Cast<IPoint>().ToArray();
            delaunator = new Delaunator(ipoints);
        }
        
        public DelaunatorGraph(IEnumerable<float2> points)
        {
            IPoint[] ipoints = points.Select(v => new Point(v.x, v.y)).Cast<IPoint>().ToArray();
            delaunator = new Delaunator(ipoints);
        }

        public IEnumerable<IEdge<Vector2>> GetEdges()
        {
            Dictionary<IPoint, Vector2> pointToVector = new();
            List<IEdge<Vector2>> edges = new();
            foreach (var edge in delaunator.GetEdges())
            {
                IPoint p = edge.P;
                IPoint q = edge.Q;
                if (!pointToVector.TryGetValue(p, out Vector2 vp))
                {
                    vp = new Vector2((float)p.X, (float)p.Y);
                    pointToVector.Add(p, vp);
                }
                if (!pointToVector.TryGetValue(q, out Vector2 vq))
                {
                    vq = new Vector2((float)q.X, (float)q.Y);
                    pointToVector.Add(q, vq);
                }
                edges.Add(new Edge<Vector2>(vp, vq));
            }

            return edges;
        }
    }
}