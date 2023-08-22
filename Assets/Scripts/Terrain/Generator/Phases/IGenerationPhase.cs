namespace Terrain.Generator.Phases
{
    public interface IGenerationPhase
    {
        void Generate(TerrainData terrainData);
    }
}