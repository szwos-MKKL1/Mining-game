using System;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using QuikGraph;
using UnityEngine;

namespace Terrain.Generator.PathGraph.Graphs
{
    //Adapter class for delaunator to use unity and quikgraph methods
    public class DelaunatorGraph<T>
    {
        private Delaunator delaunator;
        private readonly Dictionary<IPoint, T> pointTDict = new();

        public DelaunatorGraph(IEnumerable<T> points, Func<T, IPoint> toIPoint)
        {
            List<IPoint> ipoints = new();
            foreach (var t in points)
            {
                IPoint ipoint = toIPoint.Invoke(t);
                ipoints.Add(ipoint);
                pointTDict[ipoint] = t;
            }

            delaunator = new Delaunator(ipoints.ToArray());
        }

        public IEnumerable<QuikGraph.IEdge<T>> GetEdges()
        {
            return delaunator.GetEdges().Select(delEdge => new QuikGraph.Edge<T>(pointTDict[delEdge.P], pointTDict[delEdge.Q])).ToList();
        }
    }
}