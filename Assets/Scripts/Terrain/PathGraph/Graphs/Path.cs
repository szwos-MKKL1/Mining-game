using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Terrain.PathGraph.Graphs
{
    /**
     * Linear directed graph
     */
    public class Path<T> : IEnumerable<GraphNode<T>>
    {
        private readonly LinkedList<GraphNode<T>> path;

        public Path(IEnumerable<GraphNode<T>> path)
        {
            this.path = new LinkedList<GraphNode<T>>(path);
        }


        public IEnumerable<GraphEdge<T>> GetEdges()
        {
            if (path.Count < 2) return Enumerable.Empty<GraphEdge<T>>();
            List<GraphEdge<T>> edges = new();
            using IEnumerator<GraphNode<T>> pathEnumerator = path.GetEnumerator();
            pathEnumerator.MoveNext();
            GraphNode<T> p = pathEnumerator.Current;
            while (pathEnumerator.MoveNext())
            {
                GraphNode<T> q = pathEnumerator.Current;
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

        public IEnumerator<GraphNode<T>> GetEnumerator()
        {
            return path.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}