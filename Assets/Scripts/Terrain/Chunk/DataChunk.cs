using UnityEngine;

namespace Terrain
{
    public abstract class DataChunk
    {
        protected Vector2Int worldPos;

        public DataChunk(Vector2Int worldPos)
        {
            this.worldPos = worldPos;
        }

        public Vector2Int WorldPos => worldPos;
    }
}