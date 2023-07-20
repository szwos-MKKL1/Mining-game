using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Terrain.PathGraph.Graphs
{
    public class Graph<T> : IReadOnlyCollection<IGraphNode<T>>
    {
        private HashSet<GraphNode<T>> nodes;

        public Graph(HashSet<GraphNode<T>> nodes)
        {
            this.nodes = nodes;
        }

        public Graph(Path<T> path)
        {
            nodes = new HashSet<GraphNode<T>>();
            foreach (GraphNode<T> n in path)
            {
                nodes.Add(n);
            }
        }

        //TODO path normally should be derived from graph, but in this case both are stored in completely different ways
        //      instead path could simply be graph that doesn't branch, and this method would be replaced with something
        //      like Graph#Optimize(), which would remove repeating connections between nodes
        //      we would also require operation which would allow for combining multiple graphs into one
        /**
         * Combines multiple paths into one undirected graph
         * @param paths paths created from the same graph
         */
        public Graph(IEnumerable<Path<T>> paths)
        {
            //key Old node, value new node
            IEnumerable<Path<T>> enumerable = paths as Path<T>[] ?? paths.ToArray();
            Dictionary<GraphNode<T>, GraphNode<T>> dictionary = new();

            //Get all edges and add them
            foreach (var path in enumerable)
            {
                foreach (var edge in path.GetEdges())
                {
                    if (!dictionary.TryGetValue(edge.P, out var newP))
                    {
                        newP = edge.P.Clone() as GraphNode<T>;
                        dictionary.Add(edge.P, newP);
                    }
                    
                    if (!dictionary.TryGetValue(edge.Q, out var newQ))
                    {
                        newQ = edge.Q.Clone() as GraphNode<T>;
                        dictionary.Add(edge.Q, newQ);
                    }
                    newP.ConnectedNodes.Add(newQ);
                    newQ.ConnectedNodes.Add(newP);
                }
            }

            nodes = new HashSet<GraphNode<T>>();
            foreach (var n in dictionary.Values)
            {
                nodes.Add(n);
            }
        }

        public bool AddConnection(IGraphNode<T> target, IGraphNode<T> value)
        {
            if (target is not GraphNode<T> ntarget || value is not GraphNode<T> nvalue) 
                throw new InvalidCastException("target or value are not instance of GraphNode");
            return ntarget.ConnectedNodesSet.Add(nvalue);
        }

        public bool Contains(GraphNode<T> item)
        {
            return nodes.Contains(item);
        }

        public bool Remove(GraphNode<T> node)
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

        public int RemoveWhereNode([NotNull]Predicate<GraphNode<T>> predicate)
        {
            List<GraphNode<T>> toRemove = nodes.Where(node => predicate(node)).ToList();
            return toRemove.Sum(node => Remove(node) ? 1 : 0);
        }
        
        public int RemoveWhereEdge([NotNull]Predicate<GraphEdge<T>> predicate)
        {
            List<GraphEdge<T>> toRemove = GetEdges().Where(edge => predicate(edge)).ToList();
            return toRemove.Sum(edge => Remove(edge) ? 1 : 0);
        }

        public IEnumerable<GraphEdge<T>> GetEdges()
        {
            HashSet<IGraphNode<T>> ready = new();
            Queue<IGraphNode<T>> toCheck = new();
            List<GraphEdge<T>> edges = new();
            using IEnumerator<IGraphNode<T>> a = GetNodes();
            a.MoveNext();
            toCheck.Enqueue(a.Current);
            while (toCheck.Count != 0)
            {
                List<GraphNode<T>> _toCheck = new();
                while (toCheck.Count > 0)
                {
                    var current = toCheck.Dequeue();
                    foreach (var graphNode in current.ConnectedNodes.Where(currentChild => !ready.Contains(currentChild)))
                    {
                        _toCheck.Add(graphNode);
                        edges.Add(new GraphEdge<T>(current, graphNode));
                    }
                    ready.Add(current);
                }

                foreach (var n in _toCheck) toCheck.Enqueue(n);
            }

            return edges;
        }

        public IEnumerator<IGraphNode<T>> GetNodes()
        {
            return nodes.GetEnumerator();
        }


        public IEnumerator<IGraphNode<T>> GetEnumerator()
        {
            return GetNodes();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class GraphEdge<T> : IGraphEdge<T>
    {
        private readonly GraphNode<T> p;
        private readonly GraphNode<T> q;
        public GraphEdge(GraphNode<T> p, GraphNode<T> q)
        {
            this.p = p;
            this.q = q;
        }

        public IGraphNode<T> P => p;
        public IGraphNode<T> Q => q;
    }
    
    public interface IGraphEdge<out T>
    {
        public IGraphNode<T> P { get; }
        public IGraphNode<T> Q { get; }
    }

    public sealed class GraphNode<T> : ICloneable, IGraphNode<T>
    {
        private readonly HashSet<GraphNode<T>> connectedNodes = new();
        public GraphNode(T value)
        {
            Value = value;
        }
        public T Value { get; }
        public IReadOnlyCollection<IGraphNode<T>> ConnectedNodes => connectedNodes;
        public HashSet<GraphNode<T>> ConnectedNodesSet => connectedNodes;
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public interface IGraphNode<out T>
    {
        public T Value { get; }
        public IReadOnlyCollection<IGraphNode<T>> ConnectedNodes { get; }
    }

}