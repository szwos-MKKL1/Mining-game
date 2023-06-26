using Terrain.Blocks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain
{
    public class TerrainGeneratorScript : MonoBehaviour
    {
        public Tilemap tilemap;
        void Start()
        {
            //Some generation settings
            GenerationData generationData = new GenerationData
            {
                AbstractBlock = ScriptableObject.CreateInstance<AirBlockBase>()
            };

            //Generate terrain
            TerrainGenerator terrainGenerator = new TerrainGenerator();
            TerrainData terrainData = terrainGenerator.GenerateTerrain(generationData);

            
            //Populate map with generated tiles
            byte[,] data = terrainData.TileMap;
            TilePalette tilePalette = terrainData.TilePalette;
            tilemap.ClearAllTiles();
            for (int x = 0; x < terrainData.sizeX ; x++)
            {
                for (int y = 0; y < terrainData.sizeY; y++)
                {
                    TileBase tile = tilePalette.GetTile(data[x, y]);
                    if(tile != null)
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        
        }
    

        void Update()
        {
        
        }
    }
}
