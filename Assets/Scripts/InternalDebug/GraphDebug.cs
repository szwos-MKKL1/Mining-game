using System.Collections.Generic;
using System.Linq;
using QuikGraph;
using Terrain.PathGraph;
using Terrain.PathGraph.Graphs;
using UnityEngine;

namespace InternalDebug
{
    public static class GraphDebug
    {
        public static void UnityDraw(this IEdgeSet<Vector2, IEdge<Vector2>> graph, Color color, float duration)
        {
            foreach (IEdge<Vector2> edge in graph.Edges)
            {
                Debug.DrawLine(edge.Source*0.16f, edge.Target*0.16f, color, duration, false);
            }
        }
    }
}