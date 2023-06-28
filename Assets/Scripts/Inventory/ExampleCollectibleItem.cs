using System;
using UnityEngine;

//TODO: instead of hard coding this, read items from json file, or store them as .asset scriptable object

public class ExampleCollectibleItem : MonoBehaviour, ICollectible
{
    public static event HandleItemCollected OnItemCollected;
    public delegate void HandleItemCollected(ItemData itemData);
    public ItemData itemData;

    public void Collect()
    {
        Destroy(gameObject);
        OnItemCollected?.Invoke(itemData);
    }
}
