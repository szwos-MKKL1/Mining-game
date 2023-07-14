using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Terrain.PathGraph.Graphs
{
    /**
     * Linear directed graph
     */
    public class Path : IEnumerable<GraphNode>
    {
        private readonly LinkedList<GraphNode> path;
        public Path(LinkedList<GraphNode> path)
        {
            this.path = path;
        }

        public IEnumerable<GraphEdge> GetEdges()
        {
            if (path.Count < 2) return Enumerable.Empty<GraphEdge>();
            List<GraphEdge> edges = new();
            using IEnumerator<GraphNode> pathEnumerator = path.GetEnumerator();
            pathEnumerator.MoveNext();
            GraphNode p = pathEnumerator.Current;
            while (pathEnumerator.MoveNext())
            {
                GraphNode q = pathEnumerator.Current;
                edges.Add(new GraphEdge(p,q));
                p = q;
            }

            return edges;
        }

        public Graph AsGraph()
        {
            //TODO
            throw new System.NotImplementedException();
        }

        public IEnumerator<GraphNode> GetEnumerator()
        {
            return path.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}