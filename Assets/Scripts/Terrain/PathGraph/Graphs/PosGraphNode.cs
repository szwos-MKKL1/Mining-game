
using UnityEngine;

namespace Terrain.PathGraph.Graphs
{
    //TODO better, mathematical name
    public class PosGraphNode : GraphNode
    {
        private Vector2 pos;

        public PosGraphNode(Vector2 pos)
        {
            this.pos = pos;
        }

        public Vector2 Pos => pos;
    }
}