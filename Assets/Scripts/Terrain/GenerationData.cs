using System.Diagnostics.CodeAnalysis;
using Terrain.Blocks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain
{
    /**
     * Stores all essential data used for terrain generation
     */
    public class GenerationData
    {
        [NotNull]
        public Vector2Int chunkSize;
        public IBorderShape BorderShape = new CircleBorder(new Vector2Int(1024, 1024), 10);
        public BlockBase blockBase;
    }
}