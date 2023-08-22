using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//TODO: this is much wrong, not ItemVisual should implement Button, but some SlotVisual (both empty and populated slots should be able to be active/inactive)
public class ItemVisual : Button
{
    private readonly ItemData itemData;
    private bool isActive;
    public bool IsActive {
        get => isActive; 
        set 
        { 
            //TODO: simplify this code (apply styleSheet??)
            this.isActive = value;
            Color color = value ? Color.white : Color.black;
            this.style.borderBottomColor = color;
            this.style.borderRightColor = color;
            this.style.borderLeftColor = color;
            this.style.borderTopColor = color;
        } 
    }
    


    public ItemVisual(ItemData itemData)
    {
        this.isActive = false;
        this.itemData = itemData;
        name = itemData.name;
        style.height = 150; //TODO: take height width from InventoryGUI
        style.width = 150;
        style.borderBottomWidth = 2;

        style.backgroundImage = itemData.icon.texture;

    }


    public Guid getID()
    {
        return itemData.ID;
    }

   
}
