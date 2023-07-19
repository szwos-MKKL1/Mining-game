using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This class controlls other inventory related systems like
/// InventoryContainer, InventoryGUI etc.
/// </summary>
public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;


    //TODO: reconsider this Singleton implementation,why is it in Awake instead of ctor??
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else if(Instance != this)
        {
            Destroy(this);
        }
    }
    

    private bool collectItem(ItemData item)
    {

        if (InventoryGUI.Instance.Add(item))
        {
            Debug.Log("Dodano item do eq" + DateTime.Now);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CollectItem(ItemData itemData)
    {
        
        var status = InventoryGUI.Instance.Add(itemData);

        return status;
    }


}
