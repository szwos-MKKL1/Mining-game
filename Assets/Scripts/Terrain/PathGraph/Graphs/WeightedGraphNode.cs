using UnityEngine;

namespace Terrain.PathGraph.Graphs
{
    public class WeightedGraphNode : PosGraphNode
    {
        private int cost;
        
        public WeightedGraphNode(Vector2 pos) : base(pos)
        {
        }

        public int Cost => cost;
    }
}