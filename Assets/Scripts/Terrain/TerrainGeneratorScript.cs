using System;
using System.Collections.Generic;
using InternalDebug;
using QuikGraph;
using Terrain.Blocks;
using Terrain.Generator;
using Terrain.Generator.Border;
using Terrain.Generator.DecorateGenerators;
using Terrain.Generator.DecorateGenerators.BlockProvider;
using Terrain.Generator.Phases;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain
{
    public class TerrainGeneratorScript : MonoBehaviour
    {
        public Tilemap tilemap;
        public GenerationData generationData;

        private IEdgeSet<Vector2, IEdge<Vector2>> paths;
        void Start()
        {
            //StartGeneration();
        }

        public void StartGeneration()
        {
            Debug.Log("Start generating");
            var realtimeSinceStartup = Time.realtimeSinceStartup;
            //Some generation settings
            //generationData.chunkSize = new Vector2Int(10, 10);
            switch (generationData.borderShapeType)
            {
                case BorderType.Circle: 
                    generationData.BorderShape = new CircleBorder(
                        new Vector2Int(generationData.chunkSize.x*TerrainChunk.ChunkSizeX, generationData.chunkSize.y*TerrainChunk.ChunkSizeY), 
                        10);
                    break;
                case BorderType.Rectangle:
                    throw new NotImplementedException("Rectangle not implemented");
            }
            
            //generationData.borderWeight = 64;
            generationData.generationPhases = new List<IGenerationPhase>()
            {
                new RawPhase(generationData),
                //  new FillRockPhase(generationData, new StandardProvider(BlockRegistry.ROCK)),
                //  new DecoratorPhase(generationData,
                //      new VeinGenerator(new StandardProvider(BlockRegistry.ORE), 0, 0.05f),
                //      new WormGenerator(new StandardProvider(BlockRegistry.AIR), 0, 0.005f, -0.9f)
                //  ),
                // new CavePathPhase(generationData, out paths),
                new StructurePhase(generationData)
            };
            
            //Generate terrain
            TerrainGenerator terrainGenerator = new TerrainGenerator();
            
            TerrainData terrainData = terrainGenerator.GenerateTerrain(generationData);
            Debug.Log($"Finished generating in {Time.realtimeSinceStartup-realtimeSinceStartup}s");

            //Populate map with generated tiles
            tilemap.ClearAllTiles();
            Debug.Log("Start rendering");
            foreach (KeyValuePair<Vector2Int, TerrainChunk> chunkPair in terrainData)
            {
                Vector2Int chunkPos = chunkPair.Key;
                TerrainChunk chunk = chunkPair.Value;
                int i = 0;
                TileBase[] tiles = new TileBase[128*128];
                Vector3Int[] positions = new Vector3Int[128 * 128];
                for (int x = 0; x < 128 ; x++)
                {
                    for (int y = 0; y < 128; y++)
                    {
                        int xInWorld = chunkPos.x * 128 + x;
                        int yInWorld = chunkPos.y * 128 + y;
            
                        var block = chunk.Blocks[x * 128 + y];
                        if (!block.GetType().IsInstanceOfType(typeof(AirBlock)))
                        {
                            tiles[i] = block.Texture;
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
            Debug.Log("Finished rendering");
        }

        private void OnDrawGizmosSelected()
        {
            if (paths == null) return;
            foreach (IEdge<Vector2> path in paths.Edges)
            {
                Gizmos.DrawLine(path.Source*0.16f,path.Target*0.16f);
            }
            
        }
    }

    [CustomEditor(typeof(TerrainGeneratorScript))]
    public class TerrainGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TerrainGeneratorScript generatorScript = (TerrainGeneratorScript)target;

            generatorScript.tilemap =
                (Tilemap)EditorGUILayout.ObjectField("Grid", generatorScript.tilemap, typeof(Tilemap), true);
            generatorScript.generationData = (GenerationData)EditorGUILayout.ObjectField("Generation Data",
                generatorScript.generationData, typeof(GenerationData), true);
            
            if(GUILayout.Button("Generate"))
            {
                generatorScript.generationData =
                    (GenerationData)AssetDatabase.LoadAssetAtPath("Assets/default.asset", typeof(GenerationData));
                Debug.Log(generatorScript.generationData);
                generatorScript.StartGeneration();
            }
        }
    }
}
