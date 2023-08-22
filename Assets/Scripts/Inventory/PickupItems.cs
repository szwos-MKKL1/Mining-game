#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class PickupItems : MonoBehaviour
{
    private Inventory inventory;

    private void Start()
    {
        this.inventory = transform.GetComponent<Inventory>()!;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DroppedItem? item = collision.GetComponent<DroppedItem>();

        if (item != null)
        {
            if (inventory.AddItem(item.Data))
            {
                Destroy(collision.gameObject);
            }
            else
            {
                Debug.Log("Inventory was full, item not added");
            }
        }
    }
}
