using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain.Blocks
{
    public class BlockRegistry
    {
        public static readonly BlockBase AIR = new AirBlock();
        public static readonly BlockBase ROCK = new RockBlock((TileBase)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Tiles/piiiXL texture pack/Textures-16_7.asset", typeof(TileBase)));
        public static readonly BlockBase ORE = new RockBlock((TileBase)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Tiles/piiiXL texture pack/Textures-16_7 1.asset", typeof(TileBase)));
        public static readonly BlockBase BEDROCK = new RockBlock((TileBase)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Tiles/piiiXL texture pack/Textures-16_9.asset", typeof(TileBase)));
        public static readonly BlockBase DUNGEONBLOCK = new RockBlock((TileBase)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Tiles/piiiXL texture pack/Textures-16_0.asset", typeof(TileBase)));
        
    }
}