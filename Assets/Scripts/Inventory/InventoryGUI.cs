#nullable enable
using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryGUI : MonoBehaviour
{
    public static InventoryGUI Instance;

    private VisualElement root;
    private VisualElement grid;

    private SlotSelector slotSelector;

    //TODO: reconsider this Singleton implementation,why is it in Awake instead of ctor??
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            getVisualElements();
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    private void getVisualElements()
    {
        root = GetComponentInChildren<UIDocument>().rootVisualElement;
        grid = root.Q<VisualElement>("Grid");

        Debug.Log(grid.Children().First().style.width);

        slotSelector = new SlotSelector(slotsAsItemVisuals());
    }

    private List<ItemVisual> slotsAsItemVisuals()
    {
        //TODO: this will not work, because this function returns list of copies of grid.Children(), not references to them,
        //when they will be handled by SlotSelector like this, they will be completely different objects (which are not even present on screen)        
        List<ItemVisual> slots = new List<ItemVisual> ();
        foreach(var s in grid.Children())
        {
            slots.Add(s as ItemVisual);
        }

        return slots;
    }

    public bool Add(ItemData itemData)
    {
        foreach (var slot in grid.Children())
        {
            //TODO: this is a shady way to check if slot is empty
            if(slot.Children().Count() == 0)
            {
                ItemVisual newItem = new ItemVisual(itemData);
                slot.Add(newItem);
                return true;
            }
        }

        return false;
    }

    public bool Remove(ItemData itemData)
    {
        for(int i = 0; i < grid.Children().Count(); i++)
        {
            if ((grid.Children().ElementAt(i).Children().First() as ItemVisual).getID() == itemData.ID)
            {
                grid.RemoveAt(i);
                return true;
            }
        }

        return false;
    }
}
