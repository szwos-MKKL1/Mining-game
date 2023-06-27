using System.Collections.Generic;
using Terrain.Blocks;
using Terrain.Generators;
using Terrain.Phases;
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
            GenerationData generationData = new GenerationData();
            generationData.chunkSize = new Vector2Int(2, 2);
            generationData.BorderShape = new CircleBorder(new Vector2Int(1024, 1024), 10);
            generationData.generationPhases = new List<IGenerationPhase>()
            {
                new RawPhase(generationData),
                new FillRockPhase(generationData, new StandardProvider(BlockRegistry.ROCK)),
                new DecoratorPhase(generationData,
                    new VeinGenerator(new StandardProvider(BlockRegistry.ORE), 0, 0.05f),
                    new WormCaveGenerator(new StandardProvider(BlockRegistry.AIR), 0, 0.005f, 0.3f, 0.4f)
                )
            };

            //Generate terrain
            TerrainGenerator terrainGenerator = new TerrainGenerator();
            TerrainData terrainData = terrainGenerator.GenerateTerrain(generationData);

            
            //Populate map with generated tiles
            tilemap.ClearAllTiles();
            for (int chunkx = 0; chunkx < generationData.chunkSize.x; chunkx++)
            {
                for (int chunky = 0; chunky < generationData.chunkSize.y; chunky++)
                {
                    TerrainChunk chunk = terrainData.terrainChunks[chunkx, chunky];
                    Debug.Log(chunk);
                    for (int x = 0; x < 512 ; x++)
                    {
                        for (int y = 0; y < 512; y++)
                        {
                            int xInWorld = chunkx * 512 + x;
                            int yInWorld = chunky * 512 + y;

                            var tileBase = chunk.Blocks[x, y].Texture;
                            if(tileBase != null)
                                tilemap.SetTile(new Vector3Int(xInWorld, yInWorld, 0), tileBase);
                        }
                    }
                }
            }
            
        
        }
    

        void Update()
        {
        
        }
    }
}
