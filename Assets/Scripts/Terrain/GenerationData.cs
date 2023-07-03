﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terrain.Blocks;
using Terrain.Phases;
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
        [NotNull]
        public IBorderShape BorderShape;
        [NotNull]
        public List<IGenerationPhase> generationPhases;

        public byte borderWeight;
        public float borderMultiplier = 1f;
        public float distanceMultiplier = 1f;
        public int randomMin = 0;
        public int randomMax = 100;
    }
}