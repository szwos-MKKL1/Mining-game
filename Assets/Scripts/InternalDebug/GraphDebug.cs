using System.Collections.Generic;
using System.Linq;
using Terrain.PathGraph;
using Terrain.PathGraph.Graphs;
using UnityEngine;

namespace InternalDebug
{
    public class GraphDebug
    {
        //TODO for now leaving two methods for drawing graph and path, but when path is changed to subclass of graph, drawPath would be removed
        public static void DrawGraph(IEnumerable<IGraphEdge<PosGraphNode>> edges, Color color, float duration)
        {
            foreach (IGraphEdge<PosGraphNode> edge in edges)
            {
                Debug.DrawLine(edge.P.Pos*0.16f,edge.Q.Pos*0.16f,color,duration,false);
            }
        }
        
        public static void DrawPath(IEnumerable<PosGraphNode> path, Color color, float duration)
        {
            using IEnumerator<PosGraphNode> enumerator = path.GetEnumerator();
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