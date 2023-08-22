#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string Name;
    public string? Description;
    public Texture2D? Icon;


    public ItemData(string name, string? description, Texture2D? icon)
    {
        this.Name = name;
        this.Description = description;
        this.Icon = icon;

    }
}
