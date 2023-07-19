using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    //TODO: getters, setters
    public Guid ID = Guid.NewGuid();
    public string displayName;
    public string description;
    public int sellPrice;
    public Sprite icon;
}
