namespace Terrain
{
    //First phase of terrain generation where only basic caves/holes are generated
    public class RawPhase : IGenerationPhase
    {
        private readonly GenerationData generationData;
        private readonly FastNoiseLite fastNoiseLite;

        public RawPhase(GenerationData generationData)
        {
            this.generationData = generationData;
            fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            fastNoiseLite.SetFrequency(0.02f);
        }

        public void Generate(TerrainData terrainData)
        {
            
            TilePalette tilePalette = new TilePalette();
            byte baseTile = tilePalette.AddTile(generationData.baseTile);
            byte[,] tileMap = new byte[generationData.SizeX, generationData.SizeY];
            IBorderShape borderShape = generationData.BorderShape;
            for (int x = 0; x < generationData.SizeX; x++)
            {
                for (int y = 0; y < generationData.SizeY; y++)
                {
                    if (borderShape.IsInsideBorder(x, y))
                    {
                        tileMap[x, y] = fastNoiseLite.GetNoise(x, y) > 0 ? tilePalette.NullTile: baseTile;
                    }
                }
            }

            terrainData.TilePalette = tilePalette;
            terrainData.TileMap = tileMap;
        }
    }
}