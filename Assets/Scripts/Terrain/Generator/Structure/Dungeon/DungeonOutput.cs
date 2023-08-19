using System.Collections.Generic;
using QuikGraph;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain.Generator.Structure.Dungeon
{
    //TODO name
    public class DungeonOutput<T> where T : IDungeonRoom
    {
        public DungeonOutput(List<T> rooms, UndirectedGraph<T,Hallway> hallways, Dictionary<T, float2> roomEntryPoints)
        {
            Rooms = rooms;
            Hallways = hallways;
            RoomEntryPoints = roomEntryPoints;
        }

        public List<T> Rooms { get; }
        public UndirectedGraph<T,Hallway> Hallways { get; }
        public Dictionary<T, float2> RoomEntryPoints { get; }

        public class Hallway : IEdge<T>
        {
            public Hallway(List<float2> points, T source, T target)
            {
                Points = points;
                Source = source;
                Target = target;
            }

            public bool IsValid => Points.Count >= 2;
            
            public bool IsSimple => Points.Count == 2;

            public List<float2> Points { get; }
            public T Source { get; }
            public T Target { get; }
        }
    }
}