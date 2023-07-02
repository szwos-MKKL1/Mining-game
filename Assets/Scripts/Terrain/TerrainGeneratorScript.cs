using System;
using System.Collections.Generic;
using InternalDebug;
using Terrain.Blocks;
using Terrain.Generators;
using Terrain.Noise;
using Terrain.Phases;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;

namespace Terrain
{
    public class TerrainGeneratorScript : MonoBehaviour
    {
        public Tilemap tilemap;
        void Start()
        {
            // DistanceMap distanceMap = new DistanceMap(new[] { new Vector2Int(100, 100), new Vector2Int(105, 105) },
            //     pos => pos.x > 0 && pos.x < 200 && pos.y > 0 && pos.y < 200);
            // ImageDebug.SaveImg(distanceMap.Generate(new Vector2Int(200,200), 15),"weighmap.png");
            
            Debug.Log("Start generating");
            //Some generation settings
            GenerationData generationData = new GenerationData();
            generationData.chunkSize = new Vector2Int(10, 10);
            generationData.BorderShape = new CircleBorder(new Vector2Int(generationData.chunkSize.x*TerrainChunk.ChunkSizeX, generationData.chunkSize.y*TerrainChunk.ChunkSizeY), 10);
            generationData.borderWeight = 64;
            
            
            FastNoiseLite fastNoiseLite = new FastNoiseLite();
            fastNoiseLite.SetSeed(1);
            fastNoiseLite.SetFractalOctaves(3);
            fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            fastNoiseLite.SetFrequency(0.002f);
            fastNoiseLite.SetFractalType(FastNoiseLite.FractalType.FBm);
            fastNoiseLite.SetFractalLacunarity(2);
            generationData.generationPhases = new List<IGenerationPhase>()
            {
                new RawPhase(generationData),
                // new FillRockPhase(generationData, new StandardProvider(BlockRegistry.ROCK)),
                // new DecoratorPhase(generationData,
                //     new VeinGenerator(new StandardProvider(BlockRegistry.ORE), 0, 0.05f),
                //     new WormGenerator(new StandardProvider(BlockRegistry.AIR), 0, 0.005f, -0.9f),
                //     new WormGenerator(new StandardProvider(BlockRegistry.ORE), new FastNoiseAsINoise(fastNoiseLite), -0.9f)
                // ),
                new CavePathPhase(generationData)
            };
            
            //Generate terrain
            TerrainGenerator terrainGenerator = new TerrainGenerator();
            
            TerrainData terrainData = terrainGenerator.GenerateTerrain(generationData);
            Debug.Log("Finished generating");
            
            //Populate map with generated tiles
            tilemap.ClearAllTiles();
            Debug.Log("Start rendering");
            for (int chunkx = 0; chunkx < generationData.chunkSize.x; chunkx++)
            {
                for (int chunky = 0; chunky < generationData.chunkSize.y; chunky++)
                {
                    TerrainChunk chunk = terrainData.Chunks[chunkx, chunky];
                    int i = 0;
                    TileBase[] tiles = new TileBase[128*128];
                    Vector3Int[] positions = new Vector3Int[128 * 128];
                    for (int x = 0; x < 128 ; x++)
                    {
                        for (int y = 0; y < 128; y++)
                        {
                            int xInWorld = chunkx * 128 + x;
                            int yInWorld = chunky * 128 + y;
            
                            var tileBase = chunk.Blocks[x, y].Texture;
                            if (tileBase != null)
                            {
                                tiles[i] = tileBase;
                                positions[i] = new Vector3Int(xInWorld, yInWorld, 0);
                                i++;
                            }
                                //tilemap.SetTile(new Vector3Int(xInWorld, yInWorld, 0), tileBase);
                                
                            
            
                        }
                    }
                    Array.Resize(ref tiles, i);
                    Array.Resize(ref positions, i);
                    tilemap.SetTiles(positions, tiles);
                    //tilemap.SetTilesBlock(new BoundsInt(chunkx*128, chunky*128,0,128,128,0), tiles);
                }
            }
            Debug.Log("Finished rendering");
            
        }
    

        void Update()
        {
        
        }
    }
}
