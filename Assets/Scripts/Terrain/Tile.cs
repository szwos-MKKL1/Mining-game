using UnityEngine.Tilemaps;

namespace Terrain
{
    public struct Tile
    {
        public byte id;
        public TileBase TileBase;

        public Tile(byte id, TileBase tileBase)
        {
            this.id = id;
            TileBase = tileBase;
        }
        
        public static implicit operator byte(Tile v)  {  return v.id;  }
        
        public static implicit operator TileBase(Tile v)  {  return v.TileBase;  }
    }
}