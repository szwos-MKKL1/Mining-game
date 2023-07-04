using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terrain.Blocks;
using Terrain.PathGraph;
using Terrain.Phases;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain
{
    /**
     * Stores all essential data used for terrain generation
     */
    [CreateAssetMenu(menuName = "Generation Data")]
    public class GenerationData : ScriptableObject
    {
        [SerializeField]
        public Vector2Int chunkSize = new Vector2Int(10,10);
        [SerializeField]
        public BorderType borderShapeType = BorderType.Circle;
        [SerializeField]
        public List<IGenerationPhase> generationPhases;
        [SerializeField]
        public byte borderWeight = 64;
        [SerializeField]
        public PathFindingSettings pathFindingSettings;

        private IBorderShape borderShape;

        public IBorderShape BorderShape
        {
            get => borderShape;
            set => borderShape = value;
        }

        public override string ToString()
        {
            return
                $"chunksize: {chunkSize} borderShapeType: {borderShapeType} generationPhases: {generationPhases} borderWeight: {borderWeight} PathFindingSettings: {pathFindingSettings}";
        }
    }
}