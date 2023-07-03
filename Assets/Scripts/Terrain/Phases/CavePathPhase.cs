using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using InternalDebug;
using UnityEngine;
namespace Terrain.Phases
{
    public class CavePathPhase : IGenerationPhase
    {
        private readonly GenerationData generationData;

        public CavePathPhase(GenerationData generationData)
        {
            this.generationData = generationData;
        }

        public void Generate(TerrainData terrainData)
        {
            // Vector2Int realSize = terrainData.RealSize;
            // byte[,] bytes = new byte[realSize.x, realSize.y];
            Dictionary<Vector2Int, byte> dict = DistanceMapUtils.DistanceMapNotBuildable(terrainData, generationData.borderWeight);
            // foreach (var pair in dict)
            // {
            //     Vector2Int v = pair.Key;
            //     if(v.x >= 0 && v.y >= 0 && v.x < realSize.x && v.y < realSize.y)
            //         bytes[pair.Key.x, pair.Key.y] = pair.Value;
            // }
            // ImageDebug.SaveImg(bytes, "weightmap.png");

            Vector2Int startPoint = new Vector2Int(100, 100);
            
            RandomGraph randomGraph = new RandomGraph(terrainData.RealSize, 50, 68);
            Graph graph = randomGraph.GetGraph();
            GraphDebug.DrawGraph(graph, Color.red, 200);
            graph.RemoveWhere(s => !terrainData.GetBuildPermission(new Vector2Int((int)s.Pos.x, (int)s.Pos.y)));
            GraphDebug.DrawGraph(graph, Color.white, 200);

            const int seed = 69;
            System.Random random = new System.Random(seed);
            float borderMultiplier = generationData.borderMultiplier;
            float distanceMultiplier = generationData.distanceMultiplier;
            float reversedSize = 1f / (terrainData.RealSize.x * terrainData.RealSize.y);
            int randomMin = generationData.randomMin;
            int randomMax = generationData.randomMax;
            foreach (var node in graph)
            {
                Vector2Int nodePos = new Vector2Int((int)node.Pos.x, (int)node.Pos.y);
                dict.TryGetValue(nodePos, out var bc);
                float barrierCost = bc * borderMultiplier;
                float distanceCost = DistanceMethods.SqEuclidianDistance(nodePos, startPoint) * reversedSize *
                                     distanceMultiplier;
                float randomCost = random.Next(randomMin, randomMax);
                node.Cost += (int)(barrierCost + distanceCost + randomCost);
                Debug.Log($"{node} barrierCost:{barrierCost} distanceCost:{distanceCost} randomCost:{randomCost}");
            }
        }
    }
}