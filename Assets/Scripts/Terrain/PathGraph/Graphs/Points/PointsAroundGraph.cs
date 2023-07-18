using System.Collections.Generic;
using UnityEngine;

namespace Terrain.PathGraph.Graphs.Points
{
    //Created in class so that it is easier to add more settings later
    public class PointsAroundGraph<T> : IPointGenerator where T : IPosNode
    {
        private Graph<T> graph;
        private int edgeCountMin;
        private int edgeCountMax;
        private float maxOffset;
        private System.Random mRandom;

        public PointsAroundGraph(Graph<T> graph, RangeInt randomOnEdgeCount, float maxOffset, int seed = 0) 
        {
            this.graph = graph;
            edgeCountMin = randomOnEdgeCount.start;
            edgeCountMax = randomOnEdgeCount.end;
            this.maxOffset = maxOffset;
            mRandom = new System.Random(seed);
        }

        //TODO count of points should be affected by length of edge
        public IEnumerable<Vector2> GetSamples()
        {
            List<Vector2> sample = new();
            foreach (var edge in graph.GetEdges())
            {
                Vector2 a = edge.Q.Value.Pos;
                Vector2 b = edge.P.Value.Pos;
                int count = mRandom.Next(edgeCountMin, edgeCountMax);
                for (int i = 0; i < count; i++)
                {
                    float randXCenter = (float)(mRandom.NextDouble() * (a.x - b.x)) + b.x;
                    float randYCenter = (float)(mRandom.NextDouble() * (a.y - b.y)) + b.y;
                    float randX = (float)(mRandom.NextDouble() * 2 * maxOffset) + randXCenter;
                    float randY = (float)(mRandom.NextDouble() * 2 * maxOffset) + randYCenter;
                    sample.Add(new Vector2(randX, randY));
                }
            }

            return sample;
        }
    }
}