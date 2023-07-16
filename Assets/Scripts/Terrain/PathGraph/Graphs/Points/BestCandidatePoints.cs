using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Terrain.PathGraph.Graphs.Points
{
    //https://bost.ocks.org/mike/algorithms/ really good visualization
    public class BestCandidatePoints : IPointGenerator
    {
        private int numberOfCandidates;
        private Random random;
        private Vector2Int mapSize;
        private List<Vector2> points = new();


        public BestCandidatePoints(Vector2Int mapSize, int numberOfCandidates = 10, int seed = 0)
        {
            this.numberOfCandidates = numberOfCandidates;
            this.mapSize = mapSize;
            random = new Random(seed);
        }

        public int SampleCount { get; set; } = 200;

        public IEnumerable<Vector2> GetSamples()
        {
            return GetNextSamples(SampleCount);
        }

        public void Reset()
        {
            points = new();
        }

        public IEnumerable<Vector2> GetNextSamples(int sampleCount)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                points.Add(GetNextSample());
            }

            return points;
        }

        public Vector2 GetNextSample()
        {
            Vector2 bestCandidate = Vector2.zero;
            float bestDist = 0;
            for (int i = 0; i < numberOfCandidates; i++)
            {
                Vector2 c = RandomVector2();
                float dist = DistanceMethods.ManhattanDistance(FindClosest(c), c);
                if (dist > bestDist)
                {
                    bestCandidate = c;
                    bestDist = dist;
                }
            }

            return bestCandidate;
        }

        private Vector2 RandomVector2()
        {
            return new Vector2(random.Next(0, mapSize.x), random.Next(0, mapSize.y));
        }

        //TODO for now using bruteforce method of iterating all points and returning closest, but quad tree can be used instead
        private Vector2 FindClosest(Vector2 vector)
        {
            if (points.Count == 0) return RandomVector2();
            using IEnumerator<Vector2> enumerator = points.GetEnumerator();
            Vector2 closest = enumerator.Current;
            float minDist = DistanceMethods.ManhattanDistance(closest, vector);
            while (enumerator.MoveNext())
            {
                Vector2 checkedVector = enumerator.Current;
                float dist = DistanceMethods.ManhattanDistance(vector, checkedVector);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = checkedVector;
                }
            }

            return closest;
        }
    }
}