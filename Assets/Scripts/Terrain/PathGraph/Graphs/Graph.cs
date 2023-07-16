using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Terrain.PathGraph.Graphs
{
    /**
     * Class implementing planar graph "using half-edge data structure (vertices are doubly connected)"//TODO
     */
    public class Graph<T> : IEnumerable<T> where T : GraphNode
    {
        private HashSet<T> nodes;

        public Graph(HashSet<T> nodes)
        {
            this.nodes = nodes;
        }

        public Graph(Path<T> path)
        {
            nodes = new HashSet<T>();
            foreach (var n in path)
            {
                nodes.Add(n);
            }
        }

        /**
         * Combines multiple paths into one undirected graph
         * @param paths paths created from the same graph
         */
        public Graph(IEnumerable<Path<T>> paths)
        {
            //key Old node, value new node
            IEnumerable<Path<T>> enumerable = paths as Path<T>[] ?? paths.ToArray();
            Dictionary<T, T> dictionary = new();

            //Get all edges and add them
            foreach (var path in enumerable)
            {
                foreach (var edge in path.GetEdges())
                {
                    if (!dictionary.TryGetValue(edge.P, out var newP))
                    {
                        newP = edge.P.Clone() as T;
                        dictionary.Add(edge.P, newP);
                    }
                    
                    if (!dictionary.TryGetValue(edge.Q, out var newQ))
                    {
                        newQ = edge.Q.Clone() as T;
                        dictionary.Add(edge.Q, newQ);
                    }
                    newP.ConnectedNodes.Add(newQ);
                    newQ.ConnectedNodes.Add(newP);
                }
            }

            nodes = new HashSet<T>();
            foreach (var n in dictionary.Values)
            {
                nodes.Add(n);
            }
        }

        public void Add(T item)
        {
            nodes.Add(item);
        }

        public void Clear()
        {
            nodes.Clear();
        }

        public bool Contains(T item)
        {
            return nodes.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            nodes.CopyTo(array, arrayIndex);
        }

        public bool Remove(T node)
        {
            foreach (var childNode in node.ConnectedNodes)
            {
                childNode.ConnectedNodes.RemoveWhere(s => s == node);
            }
            return nodes.Remove(node);
        }

        public bool Remove(GraphEdge<T> graphEdge)
        {
            //TODO check always two
            if (!nodes.TryGetValue(graphEdge.P, out var nodeP) || 
                !nodes.TryGetValue(graphEdge.Q, out var nodeQ))
                return false;
            nodeP.ConnectedNodes.Remove(nodeQ);
            nodeQ.ConnectedNodes.Remove(nodeP);
            
            //Remove unconnected nodes
            if (nodeP.ConnectedNodes.Count == 0) Remove(nodeP);
            if (nodeQ.ConnectedNodes.Count == 0) Remove(nodeQ);
            return true;
        }

        public int Count => nodes.Count;
        public bool IsReadOnly => false;

        public int RemoveWhere([NotNull]Predicate<T> predicate)
        {
            List<T> toRemove = nodes.Where(node => predicate(node)).ToList();
            return toRemove.Sum(node => Remove(node) ? 1 : 0);
        }
        
        public int RemoveWhere([NotNull]Predicate<GraphEdge<T>> predicate)
        {
            List<GraphEdge<T>> toRemove = GetEdges().Where(edge => predicate(edge)).ToList();
            return toRemove.Sum(edge => Remove(edge) ? 1 : 0);
        }

        public IEnumerable<GraphEdge<T>> GetEdges()
        {
            HashSet<T> ready = new();
            Queue<T> toCheck = new();
            List<GraphEdge<T>> edges = new();
            using IEnumerator<T> a = this.GetEnumerator();
            a.MoveNext();
            toCheck.Enqueue(a.Current);
            while (toCheck.Count != 0)
            {
                List<T> _toCheck = new();
                while (toCheck.Count > 0)
                {
                    var current = toCheck.Dequeue();
                    foreach (var graphNode in current.ConnectedNodes.Where(currentChild => !ready.Contains(currentChild)))
                    {
                        var currentChild = (T)graphNode;
                        _toCheck.Add(currentChild);
                        edges.Add(new GraphEdge<T>(current, currentChild));
                    }
                    ready.Add(current);
                }

                foreach (var n in _toCheck) toCheck.Enqueue(n);
            }

            return edges;
        }
        

        public IEnumerator<T> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class GraphEdge<T> where T : GraphNode
    {
        private T p;
        private T q;

        public GraphEdge(T p, T q)
        {
            this.p = p;
            this.q = q;
        }

        public T P => p;

        public T Q => q;
        
    }

    public class GraphNode : ICloneable
    {
        private HashSet<GraphNode> connectedNodes = new HashSet<GraphNode>();

        public bool AddConnection(GraphNode graphNode)
        {
            return connectedNodes.Add(graphNode);
        }

        public HashSet<GraphNode> ConnectedNodes
        {
            get => connectedNodes;
            set => connectedNodes = value;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}