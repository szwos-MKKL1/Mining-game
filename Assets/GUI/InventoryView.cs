using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryView : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset ListEntryTemplate;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        var itemListController = new ItemListController();
        itemListController.InitializeItemList(uiDocument.rootVisualElement, ListEntryTemplate);
    }
}
