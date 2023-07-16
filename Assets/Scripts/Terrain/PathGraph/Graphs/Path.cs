using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Terrain.PathGraph.Graphs
{
    /**
     * Linear directed graph
     */
    public class Path<T> : IEnumerable<T> where T : GraphNode
    {
        private readonly LinkedList<T> path;
        public Path(LinkedList<T> path)
        {
            this.path = path;
        }

        public IEnumerable<GraphEdge<T>> GetEdges()
        {
            if (path.Count < 2) return Enumerable.Empty<GraphEdge<T>>();
            List<GraphEdge<T>> edges = new();
            using IEnumerator<T> pathEnumerator = path.GetEnumerator();
            pathEnumerator.MoveNext();
            T p = pathEnumerator.Current;
            while (pathEnumerator.MoveNext())
            {
                T q = pathEnumerator.Current;
                edges.Add(new GraphEdge<T>(p,q));
                p = q;
            }

            return edges;
        }

        public Graph<T> AsGraph()
        {
            //TODO
            throw new System.NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return path.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}