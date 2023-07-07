using UnityEngine;

namespace Terrain
{
    public class ChunkedData<T> where T : DataChunk
    {
        private Vector2Int chunkCount;
        private Vector2Int chunkSize;

        private T[,] chunks;//TODO save using some kind of dictionary/map
        
        public ChunkedData(Vector2Int chunkCount, Vector2Int chunkSize)
        {
            this.chunkCount = chunkCount;
            this.chunkSize = chunkSize;
            chunks = new T[chunkCount.x,chunkCount.y];
        }

        public T[,] Chunks => chunks;
        public T GetChunk(Vector2Int realPos) => chunks[realPos.x / chunkSize.x, realPos.y / chunkSize.y];
        public Vector2Int GetLocalPos(Vector2Int realPos) => new Vector2Int(realPos.x % chunkSize.x, realPos.y % chunkSize.y);
        
        public Vector2Int ChunkCount => chunkCount;
        public Vector2Int ChunkSize => chunkSize;

        
        public Vector2Int RealSize => chunkSize * chunkCount;
        
    }
}