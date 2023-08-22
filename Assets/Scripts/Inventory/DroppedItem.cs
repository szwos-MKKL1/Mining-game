using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [SerializeField]
    private ItemData _Data;
    public ItemData Data { get; set; }


    private void Start()
    {
        //setting default value if it has not been set in editor
        if(_Data == null)
        {
            _Data = new ItemData("None", null, null);
        }
        
    }

}
