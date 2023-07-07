using System.Collections.Generic;
using UnityEngine;

namespace Terrain.PathGraph
{
    public interface IPointGenerator
    {
        public IEnumerable<Vector2> GetSamples();
    }
}