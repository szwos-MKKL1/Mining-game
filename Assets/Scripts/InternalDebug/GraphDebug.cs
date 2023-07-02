using System.Collections.Generic;
using System.Linq;
using Terrain;
using UnityEngine;

namespace InternalDebug
{
    public class GraphDebug
    {
        public static void DrawGraph(IEnumerable<GraphNode> graph, Color color, float duration)
        {
            HashSet<GraphNode> drawn = new HashSet<GraphNode>();
            Queue<GraphNode> toDraw = new Queue<GraphNode>();
            using IEnumerator<GraphNode> a = graph.GetEnumerator();
            a.MoveNext();
            toDraw.Enqueue(a.Current);
            while (toDraw.Count != 0)
            {
                List<GraphNode> _toDraw = new();
                while (toDraw.Count > 0)
                {
                    var current = toDraw.Dequeue();
                    foreach (var currentChild in current.ConnectedNodes.Where(currentChild => !drawn.Contains(currentChild)))
                    {
                        _toDraw.Add(currentChild);
                        Debug.DrawLine(current.Pos*0.16f, currentChild.Pos*0.16f, color, duration, false);
                    }
                    drawn.Add(current);
                }

                foreach (var n in _toDraw) toDraw.Enqueue(n);
            }
        }
    }
}