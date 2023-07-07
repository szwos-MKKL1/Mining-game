using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace Terrain.PathGraph
{
    /**
     * Class implementing planar graph "using half-edge data structure (vertices are doubly connected)"//TODO
     */
    public class Graph : IEnumerable<GraphNode>, ICollection<GraphNode>
    {
        private HashSet<GraphNode> nodes;

        public Graph(HashSet<GraphNode> nodes)
        {
            this.nodes = nodes;
        }

        public Graph(Path path)
        {
            nodes = new HashSet<GraphNode>();
            foreach (var n in path)
            {
                nodes.Add(n);
            }
        }

        /**
         * Combines multiple paths into one undirected graph
         * @param paths paths created from the same graph
         */
        public Graph(IEnumerable<Path> paths)
        {
            //key Old node, value new node
            IEnumerable<Path> enumerable = paths as Path[] ?? paths.ToArray();
            Dictionary<GraphNode, GraphNode> dictionary = new();

            //Get all edges and add them
            foreach (var path in enumerable)
            {
                foreach (var edge in path.GetEdges())
                {
                    if (!dictionary.TryGetValue(edge.P, out var newP))
                    {
                        newP = new GraphNode(edge.P.Pos);
                        dictionary.Add(edge.P, newP);
                    }
                    
                    if (!dictionary.TryGetValue(edge.Q, out var newQ))
                    {
                        newQ = new GraphNode(edge.Q.Pos);
                        dictionary.Add(edge.Q, newQ);
                    }
                    newP.ConnectedNodes.Add(newQ);
                    newQ.ConnectedNodes.Add(newP);
                }
            }

            nodes = new HashSet<GraphNode>();
            foreach (var n in dictionary.Values)
            {
                nodes.Add(n);
            }
        }

        public void Add(GraphNode item)
        {
            nodes.Add(item);
        }

        public void Clear()
        {
            nodes.Clear();
        }

        public bool Contains(GraphNode item)
        {
            return nodes.Contains(item);
        }

        public void CopyTo(GraphNode[] array, int arrayIndex)
        {
            nodes.CopyTo(array, arrayIndex);
        }

        public bool Remove(GraphNode node)
        {
            foreach (var childNode in node.ConnectedNodes)
            {
                childNode.ConnectedNodes.RemoveWhere(s => s == node);
            }
            return nodes.Remove(node);
        }

        public int Count => nodes.Count;
        public bool IsReadOnly => false;

        public int RemoveWhere([NotNull]Predicate<GraphNode> predicate)
        {
            List<GraphNode> toRemove = nodes.Where(node => predicate(node)).ToList();
            return toRemove.Sum(node => Remove(node) ? 1 : 0);
        }

        public IEnumerable<GraphEdge> GetEdges()
        {
            HashSet<GraphNode> ready = new();
            Queue<GraphNode> toCheck = new();
            List<GraphEdge> edges = new();
            using IEnumerator<GraphNode> a = this.GetEnumerator();
            a.MoveNext();
            toCheck.Enqueue(a.Current);
            while (toCheck.Count != 0)
            {
                List<GraphNode> _toCheck = new();
                while (toCheck.Count > 0)
                {
                    var current = toCheck.Dequeue();
                    foreach (var currentChild in current.ConnectedNodes.Where(currentChild => !ready.Contains(currentChild)))
                    {
                        _toCheck.Add(currentChild);
                        edges.Add(new GraphEdge(current, currentChild));
                    }
                    ready.Add(current);
                }

                foreach (var n in _toCheck) toCheck.Enqueue(n);
            }

            return edges;
        }

        public IEnumerator<GraphNode> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class GraphEdge
    {
        private GraphNode p;
        private GraphNode q;

        public GraphEdge(GraphNode p, GraphNode q)
        {
            this.p = p;
            this.q = q;
        }

        public GraphNode P => p;

        public GraphNode Q => q;
        
    }
    
    public class GraphNode
    {
        //positions right now are always integer so it could be changed to Vector2Int
        private Vector2 pos;
        private int cost = 0;
        private HashSet<GraphNode> connectedNodes = new HashSet<GraphNode>();

        public GraphNode(Vector2 pos)
        {
            this.pos = pos;
        }

        public bool AddConnection(GraphNode graphNode)
        {
            return connectedNodes.Add(graphNode);
        }

        public Vector2 Pos => pos;

        public int Cost
        {
            get => cost;
            set => cost = value;
        }

        public HashSet<GraphNode> ConnectedNodes
        {
            get => connectedNodes;
            set => connectedNodes = value;
        }

        public override string ToString()
        {
            return $"GraphNode(pos={pos.ToString()},cost={cost},connectedNodesCount={connectedNodes.Count})";
        }
    }
}