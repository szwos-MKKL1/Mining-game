using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MineableObject : MonoBehaviour
{
    private Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Collision! " + other.tag);

        if (other != null) { 
            if(other.CompareTag("CausesDamage"))
            {
                Vector3Int cellPosition = tilemap.WorldToCell(other.transform.position);

                Debug.Log(cellPosition);

                tilemap.SetTile(cellPosition, null);
            }
        }
    }
}
