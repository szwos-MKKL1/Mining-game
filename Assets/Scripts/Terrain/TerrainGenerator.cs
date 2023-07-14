using System;
using System.Collections;
using System.Collections.Generic;
using Terrain.Blocks;
using Terrain.Generators;
using Terrain.Phases;
using UnityEngine;

namespace Terrain
{
    public class TerrainGenerator
    {

        //TODO can be async
        public TerrainData GenerateTerrain(GenerationData generationData)
        {
            TerrainData terrainData = new TerrainData(generationData.chunkSize);
            CheckForDependencyOrder(generationData.generationPhases);
            foreach (var vPhase in generationData.generationPhases)
            {
                Debug.Log("Started phase " + vPhase);
                var realtimeSinceStartup = Time.realtimeSinceStartup;
                vPhase.Generate(terrainData);
                Debug.Log($"Finished phase {vPhase} in {Time.realtimeSinceStartup-realtimeSinceStartup}s");
            }

            return terrainData;
        }

        private void CheckForDependencyOrder(List<IGenerationPhase> generationPhases)
        {
            HashSet<Type> before = new();
            foreach (var phase in generationPhases)
            {
                foreach (var phaseDependencyAttribute in PhaseDependencyAttribute.GetDependencies(phase))
                {
                    if (phaseDependencyAttribute.Order == DependencyOrder.Before)
                    {
                        if (!before.Contains(phaseDependencyAttribute.Type))
                            throw new Exception(
                                $"Incorrect order! {phase.GetType().FullName} requires {phaseDependencyAttribute.Type.FullName} before executing it");
                    }
                    else
                    {
                        //TODO implement after
                    }
                        
                }
                before.Add(phase.GetType());
            }
        }
    }
}