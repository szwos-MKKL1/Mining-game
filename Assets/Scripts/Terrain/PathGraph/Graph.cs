using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace Terrain.PathGraph
{
    public class Graph : IEnumerable<GraphNode>, ICollection<GraphNode>
    {
        private HashSet<GraphNode> nodes;

        public Graph(HashSet<GraphNode> nodes)
        {
            this.nodes = nodes;
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

        public IEnumerator<GraphNode> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
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