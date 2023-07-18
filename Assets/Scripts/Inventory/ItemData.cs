using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public string ID = Guid.NewGuid().ToString();
    public string displayName;
    public string description;
    public int sellPrice;
    public Sprite icon;
}
