using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemData ItemData;
    public int stackSize; //TODO: consider renaming to somthing that indicates, that this is CURRENT stack size, not e.g. stack max size

    public InventoryItem(ItemData item)
    {
        ItemData = item;
        AddToStack();
    }

    //TODO: consider renaming 
    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }
}
