using System;
using Terrain.Generator.Noise;
using UnityEngine;

namespace InternalDebug
{
    public class ImageDebug
    {
        public static void SaveImg(Vector2Int size, INoise noise, string name)
        {
            try
            {
                Texture2D texture = new Texture2D(size.x, size.y);
                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        float v = (noise.GetNoise(x, y) + 1) * 0.5f;
                        Color color = new Color(v, v, v);
                        texture.SetPixel(x, y, color);
                    }
                }

                texture.Apply();
                SaveTextureAsPNG(texture, name);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while trying to create image of noise " + name + ". " + Environment.NewLine + ex.ToString());
            }
        }
        
        public static void SaveImg(byte[,] byteMap, string name)
        {
            try
            {
                Texture2D texture = new Texture2D(byteMap.GetLength(0), byteMap.GetLength(1));
                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        float v = (float)byteMap[x, y]/byte.MaxValue;
                        Color color = new Color(v, v, v);
                        texture.SetPixel(x, y, color);
                    }
                }

                texture.Apply();
                SaveTextureAsPNG(texture, name);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while trying to create image of noise " + name + ". " + Environment.NewLine + ex.ToString());
            }
        }
        
        public static void SaveImg(byte[] byteMap,Vector2Int size, string name,byte maxval = byte.MaxValue)
        {
            try
            {
                Texture2D texture = new Texture2D(size.x, size.y);
                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        float v = (float)byteMap[x+y*texture.width]/maxval;
                        Color color = new Color(v, v, v);
                        texture.SetPixel(x, y, color);
                    }
                }

                texture.Apply();
                SaveTextureAsPNG(texture, name);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while trying to create image of noise " + name + ". " + Environment.NewLine + ex.ToString());
            }
        }
        
        public static void SaveImg(bool[,] boolMap, string name)
        {
            try
            {
                Texture2D texture = new Texture2D(boolMap.GetLength(0), boolMap.GetLength(1));
                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        float v = boolMap[x,y] ? 1f: 0f;
                        Color color = new Color(v, v, v);
                        texture.SetPixel(x, y, color);
                    }
                }

                texture.Apply();
                SaveTextureAsPNG(texture, name);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while trying to create image of noise " + name + ". " + Environment.NewLine + ex.ToString());
            }
        }
        
        public static void SaveImg(bool[] boolMap, Vector2Int size, string name)
        {
            try
            {
                Texture2D texture = new Texture2D(size.x, size.y);
                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        float v = boolMap[x+y*texture.width] ? 1f: 0f;
                        Color color = new Color(v, v, v);
                        texture.SetPixel(x, y, color);
                    }
                }

                texture.Apply();
                SaveTextureAsPNG(texture, name);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while trying to create image of noise " + name + ". " + Environment.NewLine + ex.ToString());
            }
        }
        
        public static void SaveImg(ushort[] distanceMap, Vector2Int size, string name)
        {
            try
            {
                Texture2D texture = new Texture2D(size.x, size.y);
                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        float v = distanceMap[x+y*texture.width]/500f;
                        Color color = new Color(v, v, v);
                        texture.SetPixel(x, y, color);
                    }
                }

                texture.Apply();
                SaveTextureAsPNG(texture, name);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while trying to create image of noise " + name + ". " + Environment.NewLine + ex.ToString());
            }
        }
        
        
        public static void SaveTextureAsPNG(Texture2D _texture, string _path)
        {
            byte[] _bytes =_texture.EncodeToPNG();
            System.IO.File.WriteAllBytes("Debug/Images/" + _path, _bytes);
            Debug.Log(_bytes.Length/1024  + "Kb was saved as: Debug/Images/" + _path);
        }
    }
}