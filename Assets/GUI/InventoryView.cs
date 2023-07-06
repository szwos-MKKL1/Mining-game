using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryView : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset ListEntryTemplate;

    [SerializeField]
    Inventory inventory;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        inventory = FindObjectOfType<Inventory>();

        var itemListController = new ItemListController();
        itemListController.InitializeItemList(uiDocument.rootVisualElement, ListEntryTemplate, inventory);
    }
}
