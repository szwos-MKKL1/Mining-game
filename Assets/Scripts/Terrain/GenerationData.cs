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
        public IBorderShape BorderShape = new CircleBorder(500, new Vector2Int(512, 512));
        public BlockBase blockBase;
    }
}