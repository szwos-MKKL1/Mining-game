using System;
using UnityEngine;

//TODO: instead of hard coding this, read items from json file, or store them as .asset scriptable object

public class ExampleCollectibleItem : MonoBehaviour, ICollectible
{
    public delegate void ItemCollectedCallback(bool status);

    public static event HandleItemCollected OnItemCollected;
    public delegate void HandleItemCollected(ItemData itemData, ItemCollectedCallback callback);
    public ItemData itemData;

    public void Collect()
    {
        OnItemCollected?.Invoke(itemData, HandleItemCollectedCallback);
    }

    private void HandleItemCollectedCallback(bool status)
    {
        if(status)
            Destroy(gameObject);
    }
}