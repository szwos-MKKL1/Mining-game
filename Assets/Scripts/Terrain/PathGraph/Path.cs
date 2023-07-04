using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Terrain.PathGraph
{
    //TODO use list
    public class Path : IEnumerable<GraphNode>
    {
        private readonly LinkedList<GraphNode> path;
        public Path(LinkedList<GraphNode> path)
        {
            this.path = path;
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