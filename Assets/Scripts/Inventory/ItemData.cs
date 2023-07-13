using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    //TODO: {get; set;} ??
    public string ID = Guid.NewGuid().ToString();
    public string displayName;
    public string description;
    public int sellPrice; //TODO: this will propably be changed depending on established economy system
    public Sprite icon;
}
