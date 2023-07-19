using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemVisual : VisualElement
{
    private readonly ItemData itemData;

    public ItemVisual(ItemData itemData)
    {
        this.itemData = itemData;
        name = itemData.name;
        style.height = 150; //TODO: take height width from InventoryGUI
        style.width = 150;

        style.backgroundImage = itemData.icon.texture;

    }

    public Guid getID()
    {
        return itemData.ID;
    }
}
