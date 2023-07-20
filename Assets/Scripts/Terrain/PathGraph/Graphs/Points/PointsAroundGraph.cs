using System.Collections.Generic;
using QuikGraph;
using UnityEngine;

namespace Terrain.PathGraph.Graphs.Points
{
    //Created in class so that it is easier to add more settings later
    public class PointsAroundGraph : IPointGenerator
    {
        private IEnumerable<IEdge<Vector2>> edges;
        private int edgeCountMin;
        private int edgeCountMax;
        private float maxOffset;
        private System.Random mRandom;

        public PointsAroundGraph(IEnumerable<IEdge<Vector2>> edges, RangeInt randomOnEdgeCount, float maxOffset, int seed = 0) 
        {
            this.edges = edges;
            edgeCountMin = randomOnEdgeCount.start;
            edgeCountMax = randomOnEdgeCount.end;
            this.maxOffset = maxOffset;
            mRandom = new System.Random(seed);
        }

        //TODO count of points should be affected by length of edge
        public IEnumerable<Vector2> GetSamples()
        {
            List<Vector2> sample = new();
            foreach (IEdge<Vector2> edge in edges)
            {
                Vector2 a = edge.Source;
                Vector2 b = edge.Target;
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