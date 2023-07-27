using UnityEngine;

namespace Terrain
{
    public abstract class DataChunk
    {
        protected DataChunk(Vector2Int chunkId)
        {
            ChunkId = chunkId;
        }

        public Vector2Int ChunkId { get; }
    }
}