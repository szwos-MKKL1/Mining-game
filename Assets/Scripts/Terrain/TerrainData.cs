using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain
{
    public class TerrainData
    {
        public int sizeX;
        public int sizeY;
        private byte[,] tileMap;

        public TerrainData(Vector2Int size)
        {
            sizeX = size.x;
            sizeY = size.y;
            tileMap = new byte[size.x, size.y];
        }

        public TilePalette TilePalette { get; set; }

        public byte[,] TileMap
        {
            get => tileMap;
            set => tileMap = value;
        }
    }
}