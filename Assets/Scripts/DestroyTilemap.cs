using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//TODO: this code could generally be optimized better,
//TODO: investigate significance of
//A) GetComponent at every OnTriggerEnter
//B) passing tiles to delete as list

public class DestroyTilemap : MonoBehaviour
{
    private Tilemap targetTilemap;

    [SerializeField]
    public int radius = 4;


    private void OnTriggerEnter2D(Collider2D other)
    {
        //TODO: this is also kind of resource heavy, but propably the only right way to work for multiple tilemaps and multiple scenes
        //TODO: also instead of accessing Tilemap component, it should be encapsulated in some kind of TileMapController component, that will handle tile states, drops etc
        targetTilemap = other.gameObject.GetComponent<Tilemap>();
        if(targetTilemap != null)
        {
            if (targetTilemap.CompareTag("DestructibleTilemap"))
            {
                List<Vector3Int> cellPositions = GetPositions();

                foreach (var cellPosition in cellPositions)
                {
                    targetTilemap.SetTile(cellPosition, null);
                }
            }
        }
    }

    private List<Vector3Int> GetPositions()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        Vector3Int rootPosition = targetTilemap.WorldToCell(transform.position);

        for(int i = 0 - radius; i < 0 + radius; i++)
        {
            for(int j = 0 - radius; j < 0 + radius; j++)
            {
                if(i * i + j * j < radius)
                {
                    positions.Add(rootPosition + new Vector3Int(i, j, 0));
                }
            }
        }

        return positions;
    }
}
