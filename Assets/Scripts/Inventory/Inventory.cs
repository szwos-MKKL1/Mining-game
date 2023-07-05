using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //TODO: two separate containers is a bad idea
    public List<InventoryItem> inventory = new List<InventoryItem>();
    private Dictionary<ItemData, InventoryItem> itemDict = new Dictionary<ItemData, InventoryItem>();

    private void OnEnable()
    {
        ExampleCollectibleItem.OnItemCollected += Add;
    }

    private void OnDisable()
    {
        ExampleCollectibleItem.OnItemCollected -= Add;
    }

    public void Add(ItemData itemData)
    {
       
        if(itemDict.TryGetValue(itemData, out InventoryItem item)) 
        {
            item.AddToStack();
            Debug.Log(item.stackSize.ToString() + ": " + item.ItemData.displayName);
        } else
        {
            InventoryItem newItem = new InventoryItem(itemData);
            inventory.Add(newItem);
            itemDict.Add(itemData, newItem);
        }
    }

    public void Remove(ItemData itemData)
    {
        if(itemDict.TryGetValue(itemData, out InventoryItem item))
        {
            item.RemoveFromStack();
            if(item.stackSize == 0)
            {
                inventory.Remove(item);
                itemDict.Remove(itemData);
            }
        }

    }

    public List<ItemData> GetAllItems()
    {
        List<ItemData> items = new List<ItemData>();
        foreach(var i in inventory)
        {
            items.Add(i.ItemData);
        }

        return items;
    }
}
