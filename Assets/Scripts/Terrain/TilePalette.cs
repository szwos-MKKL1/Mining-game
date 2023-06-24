using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace Terrain
{
    /**
     * Class that provides tools for storing and using TileBases with their corresponding ids. 
     */
    public class TilePalette
    {
        private byte index = 0;
        private readonly Dictionary<byte, Tile> tiles = new Dictionary<byte, Tile>();

        public Tile NullTile = new Tile(0, null);

        public TilePalette()
        {
            tiles[0] = new Tile(0, null); //Air
        }

        public byte AddTile(TileBase tileBase)
        {
            byte id = ++index;
            tiles[id] = new Tile(id, tileBase);
            return id;
        }

        public void AddId(byte id, TileBase tileBase)
        {
            if (tiles.ContainsKey(id)) throw new ArgumentException("Already in palette");
            tiles[id] = new Tile(id, tileBase);
        }

        public Tile GetTile(byte id)
        {
            return tiles[id];
        }
        
    }
}