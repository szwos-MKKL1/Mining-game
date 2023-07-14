using System.Collections.Generic;
using UnityEngine;

namespace Terrain.PathGraph.Graphs.Points
{
    public interface IPointGenerator
    {
        public IEnumerable<Vector2> GetSamples();
    }
}