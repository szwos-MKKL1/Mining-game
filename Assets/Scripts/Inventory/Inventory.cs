using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<ItemData> items = new List<ItemData>();

    public bool AddItem(ItemData item)
    {
        
        //TODO: check if there is space for item, and return true/false accordingly
        items.Add(item);
        Debug.Log(items.Count);
        return true;

        
    }
}
