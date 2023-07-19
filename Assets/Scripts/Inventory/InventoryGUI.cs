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
