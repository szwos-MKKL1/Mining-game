using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain.Blocks
{
    public class BlockRegistry
    {
        public static readonly BlockBase AIR = new AirBlock();
        public static readonly BlockBase ROCK = new RockBlock((TileBase)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Tiles/piiiXL texture pack/Textures-16_7.asset", typeof(TileBase)));
    }
}