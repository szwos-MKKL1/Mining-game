using UnityEngine;

namespace Terrain.PathGraph.Graphs
{
    public class PathFinderNode : IPosNode, IWeightNode
    {
        public PathFinderNode(Vector2 pos, int weight)
        {
            Pos = pos;
            Weight = weight;
        }

        public Vector2 Pos { get; set; }
        public int Weight { get; set; }
    }
}