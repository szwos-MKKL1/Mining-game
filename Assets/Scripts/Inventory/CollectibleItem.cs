using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CollectibleItem : MonoBehaviour, ICollectible
{
    [SerializeField]
    public ItemData ItemData;

    public ItemData GetItemData() { return ItemData; }

    //Those events do nothing currently
    /*public event HandleItemCollected OnItemCollected;
    public delegate void HandleItemCollected(ItemData itemData);

    public ItemData ItemData;

    public void Start()
    {
        OnItemCollected += async () => await Collect();
    }


    private Task Collect()
    {
        Debug.Log(this);

        return new Task(() =>
        {
            Debug.Log("Zebrano item" + DateTime.Now);
            if (InventoryController.Instance.CollectItem(ItemData))
            {
                Debug.Log("Usuwam item" + DateTime.Now);
                Destroy(gameObject);
            }
        });
        
    }*/

}
