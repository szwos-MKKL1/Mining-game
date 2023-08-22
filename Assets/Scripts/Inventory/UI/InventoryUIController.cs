#nullable enable
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    private Canvas _Canvas;
    private Inventory _Inventory;

    [SerializeField]
    public Sprite SpriteTest;

    
    private Image? _Img = null!;

    // Start is called before the first frame update
    void Start()
    {
        _Canvas = GetComponent<Canvas>();
        _Inventory = GetComponent<Inventory>();

        //TODO: if _Canvas, _Inventory null, crash or something
    }

    private void OnGUI()
    {
        if(_Img == null)
        {
            _Img = _Canvas.AddComponent<Image>();
            _Img.sprite = SpriteTest;
        }
        
    }

}
