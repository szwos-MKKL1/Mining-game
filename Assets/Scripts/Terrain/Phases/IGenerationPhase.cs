namespace Terrain
{
    public interface IGenerationPhase
    {
        void Generate(TerrainData terrainData);
    }
}