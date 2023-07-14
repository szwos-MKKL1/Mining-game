using System.Collections.Generic;
using System.Linq;
using Terrain.PathGraph;
using Terrain.PathGraph.Graphs;
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
        
        public static void DrawPath(Path path, Color color, float duration)
        {
            using IEnumerator<GraphNode> enumerator = path.GetEnumerator();
            enumerator.MoveNext();
            Vector2 p1 = enumerator.Current.Pos;
            while (enumerator.MoveNext())
            {
                var p2 = enumerator.Current.Pos;
                Debug.DrawLine(p1*0.16f,p2*0.16f,color,duration,false);
                p1 = p2;
            }
        }
    }
}