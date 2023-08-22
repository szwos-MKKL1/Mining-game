using System.Collections.Generic;
using System.Linq;
using QuikGraph;
using UnityEngine;

namespace InternalDebug
{
    public static class GraphDebug
    {
        public static void UnityDraw(this IEdgeSet<Vector2, IEdge<Vector2>> graph, Color color, float duration)
        {
            graph.Edges.UnityDraw(color, duration);
        }
        
        public static void UnityDraw(this IEnumerable<IEdge<Vector2>> edges, Color color, float duration)
        {
            foreach (IEdge<Vector2> edge in edges)
            {
                Debug.DrawLine(edge.Source*0.16f, edge.Target*0.16f, color, duration, false);
            }
        }
    }
}