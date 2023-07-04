using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryView : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset ListEntryTemplate; //TODO: what is that???

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        var itemListController = new ItemListController();
        itemListController.Init(uiDocument.rootVisualElement, ListEntryTemplate);
    }
}