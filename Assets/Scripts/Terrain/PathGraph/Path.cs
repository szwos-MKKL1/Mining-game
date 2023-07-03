using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Terrain.PathGraph
{
    public class Path : IEnumerable<PathNodeEnumeratorElement>
    {
        private readonly PathNode firstNode;
        
        public Path(PathNode firstNode)
        {
            this.firstNode = firstNode;
        }
        
        public IEnumerator<PathNodeEnumeratorElement> GetEnumerator()
        {
            return new PathEnumerator(firstNode);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
    public class PathEnumerator : IEnumerator<PathNodeEnumeratorElement>
    {
        private PathNode firstNode;
        private PathNodeEnumeratorElement current;
        private PathNodeEnumeratorElement next;

        public PathEnumerator(PathNode firstNode)
        {
            this.firstNode = firstNode;
            next = new PathNodeEnumeratorElement(firstNode, null, 0);
        }

        public bool MoveNext()
        {
            if (next == null) return false;
            current = next;
            PathNode[] nextElems = current.rootElement.GetNextNodes();
            if (nextElems == null || nextElems.Length == 0)
                next = null;
            else if(nextElems.Length == 1)
                next = new PathNodeEnumeratorElement(nextElems[0], null, 0);
            else next = new PathNodeEnumeratorElement(nextElems[0], nextElems.Skip(1), nextElems.Length-1);
            return true;
        }

        public void Reset()
        {
            current = null;
            next = new PathNodeEnumeratorElement(firstNode, null, 0);
        }

        public PathNodeEnumeratorElement Current => current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }

    public class PathNodeEnumeratorElement
    {
        public readonly PathNode rootElement;
        public readonly IEnumerable<PathNode> branches;
        public readonly int branchCount;
        
        public PathNodeEnumeratorElement(PathNode rootElement, IEnumerable<PathNode> branches, int branchCount)
        {
            this.rootElement = rootElement;
            this.branches = branches;
            this.branchCount = branchCount;
        }
    }
    public class PathNode
    {
        private readonly byte branch;
        private readonly GraphNode mGraphNode;
        private readonly List<PathNode> nextNodes;

        public PathNode(GraphNode graphNode, byte branch = 0)
        {
            this.branch = branch;
            mGraphNode = graphNode;
            nextNodes = new List<PathNode>();
        }

        public byte Branch => branch;

        public GraphNode GraphNode => mGraphNode;

        public IEnumerator<PathNode> getNextNodesEnumerator()
        {
            return nextNodes.GetEnumerator();
        }

        public PathNode[] GetNextNodes()
        {
            return nextNodes.ToArray();
        }

        public void AddNextNode(PathNode pathNode)
        {
            nextNodes.Add(pathNode);
        }

        public bool RemoveNextNode(PathNode pathNode)
        {
            return nextNodes.Remove(pathNode);
        }

        public bool Contains(PathNode pathNode)
        {
            return nextNodes.Contains(pathNode);
        }

        public int NextNodesCount => nextNodes.Count;

    }
}