using System;
using Random;
using Terrain.Outputs;
using UnityEngine;

namespace Terrain.Generator.Structure
{
    public abstract class Structure
    {
        public abstract BlockCollector getStructureBlocks(Context context);

        public class Context
        {
            public Context(IRandom random, Vector2 position)
            {
                Random = random;
                Position = position;
            }

            public Vector2 Position { get; }
            public IRandom Random { get; }
        }
    }
}