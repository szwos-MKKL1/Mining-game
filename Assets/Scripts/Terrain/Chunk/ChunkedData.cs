using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class ChunkedData<T> : IEnumerable<KeyValuePair<Vector2Int, T>> where T : DataChunk
    {
        private Vector2Int chunkCount;
        private Vector2Int chunkSize;

        protected readonly Dictionary<Vector2Int, T> Chunks;

        public ChunkedData(Vector2Int chunkCount, Vector2Int chunkSize) : this(chunkCount, chunkSize, new Dictionary<Vector2Int, T>()) { }
        
        public ChunkedData(Vector2Int chunkCount, Vector2Int chunkSize, Dictionary<Vector2Int, T> chunkDictionary)
        {
            this.chunkCount = chunkCount;
            this.chunkSize = chunkSize;
            Chunks = chunkDictionary;
        }

        public void SetChunk(Vector2Int chunkId, T chunk)
        {
            Chunks[chunkId] = chunk;
        }

        public T GetChunk(Vector2Int realPos) => Chunks[new Vector2Int(realPos.x / chunkSize.x, realPos.y / chunkSize.y)];
        public Vector2Int GetLocalPos(Vector2Int realPos) => new Vector2Int(realPos.x % chunkSize.x, realPos.y % chunkSize.y);
        
        public Vector2Int ChunkCount => chunkCount;
        public Vector2Int ChunkSize => chunkSize;

        
        public Vector2Int RealSize => chunkSize * chunkCount;

        public IEnumerator<KeyValuePair<Vector2Int, T>> GetEnumerator()
        {
            return Chunks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}