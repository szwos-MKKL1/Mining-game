using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour, ICollectible
{
    //Those events do nothing currently
    public static event HandleItemCollected OnItemCollected;
    public delegate void HandleItemCollected(ItemData itemData);
    public ItemData ItemData;

    public void Collect()
    {
        OnItemCollected?.Invoke(ItemData);
        Destroy(gameObject);
    }

}
