using System;

namespace Terrain.Phases
{
    public interface IGenerationPhase
    {
        void Generate(TerrainData terrainData);
    }
}