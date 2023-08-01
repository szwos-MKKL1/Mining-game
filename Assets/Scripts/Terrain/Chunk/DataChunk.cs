using UnityEngine;

namespace Terrain.Chunk
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