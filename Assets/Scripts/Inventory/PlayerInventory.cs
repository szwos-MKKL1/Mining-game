

using Cysharp.Threading.Tasks;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomUIElements;
using System.Net;

public sealed class PlayerInventory : MonoBehaviour
{
    //TODO: consider renaming to InventoryController
    //TODO: reconsider using Singleton
    public static PlayerInventory Instance;

    private VisualElement root;
    private VisualElement inventoryGrid;

    private static Label itemDetailHeader;
    private static Label itemDetailBody;
    private static Label itemDetailPrice;
    private bool isInventoryReady;

    public static int slotWidth { get; private set; }
    public static int slotHeight { get; private set; }

    
    public List<StoredItem> StoredItems = new List<StoredItem>();
    public int inventoryWidth;
    public int inventoryHeight;

    // private void OnEnable()
    // {
    //     ExampleCollectibleItem.OnItemCollected += AddItem;
    // }
    //
    // private void OnDisable()
    // {
    //     ExampleCollectibleItem.OnItemCollected -= AddItem;
    // }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            Configure();
        } else if(Instance != this)
        {
            Destroy(this);
        }
    }

    private async void Configure()
    {
        root = GetComponentInChildren<UIDocument>().rootVisualElement;
        inventoryGrid = root.Q<VisualElement>("Grid");

        VisualElement itemDetails = root.Q<VisualElement>("ItemDetails");

        itemDetailHeader = itemDetails.Q<Label>("Header");
        itemDetailBody = itemDetails.Q<Label>("Body");
        itemDetailPrice = itemDetails.Q<Label>("SellPrice");


        isInventoryReady = true;
    }
    
    private void ConfigureSlotDimensions()
    {
        VisualElement firstSlot = inventoryGrid.Children().First();
    
        slotWidth = Mathf.RoundToInt(firstSlot.worldBound.width);
        slotHeight = Mathf.RoundToInt(firstSlot.worldBound.height);
    }

    // private async Task<bool> GetPositionForItem(VisualElement newItem)
    // {
    //     for(int y = 0; y < inventoryWidth; y++)
    //     {
    //         for(int x = 0; x < inventoryHeight; x++)
    //         {
    //             SetItemPosition(newItem, new Vector2( slotWidth * x,slotHeight * y));
    //
    //             await UniTask.WaitForEndOfFrame();
    //
    //             StoredItem overlappingItem = StoredItems.FirstOrDefault(s =>
    //                 s.RootVisual != null &&
    //                 s.RootVisual.layout.Overlaps(newItem.layout));
    //
    //             if (overlappingItem == null) 
    //             {
    //                 return true;
    //             }
    //         }
    //     }
    //     return false;
    //}

    private static void SetItemPosition(VisualElement element, Vector2 vector)
    {
        element.style.left = vector.x;
        element.style.top = vector.y;
    }

    //TODO: why is it done like this? is it bcs LoadInventory is async?
    private void Start()
    {
        foreach (var child in inventoryGrid.Children())
        {
            Debug.Log($"aaa {child.name} {StoredItems[1].Details}");
            var label1 = new Label( "Hell-o-world" );
            //child.Add( label1 );
            child.Add(new ItemVisualElement(StoredItems[1].Details));
        }
        
        //VisualElement firstSlot = inventoryGrid.Children().First();
    }

    // private async void LoadInventory() 
    // {
    //     //TODO: how this works
    //     await UniTask.WaitUntil(() => isInventoryReady);
    //
    //     foreach (StoredItem loadedItem in StoredItems)
    //     {
    //         ItemVisualElement inventoryItemVisual = new ItemVisualElement(loadedItem.Details);
    //
    //         AddItemToInventoryGrid(inventoryItemVisual);
    //
    //         bool inventoryHasSpace = await GetPositionForItem(inventoryItemVisual);
    //
    //         if(!inventoryHasSpace)
    //         {
    //             //TODO: some indication for the user (propably somewhere else,
    //             //you don't want to indicate full inventory, when inventory is allready
    //             //full and player wants to pickup another item)
    //             Debug.Log("Inventory has no space!");
    //             RemoveItemFromInventoryGrid(inventoryItemVisual);
    //             continue;
    //         }
    //
    //         ConfigureInventoryItem(loadedItem, inventoryItemVisual);
    //     }
    // }
    //
    // public async void AddItem(ItemData newItem, ExampleCollectibleItem.ItemCollectedCallback callback)
    // {
    //     ItemVisualElement inventoryItemVisual = new ItemVisualElement(newItem);
    //
    //     StoredItem storedItem = new StoredItem();
    //     storedItem.Details = newItem;
    //     storedItem.RootVisual = inventoryItemVisual;
    //
    //     AddItemToInventoryGrid(inventoryItemVisual);
    //
    //     bool inventoryHasSpace = await GetPositionForItem(inventoryItemVisual);
    //
    //     if(!inventoryHasSpace)
    //     {
    //         Debug.Log("Inventory has no space!");
    //         RemoveItemFromInventoryGrid(inventoryItemVisual);
    //         callback?.Invoke(false);
    //         return;
    //     }
    //
    //     //TODO: this call could propably be simplified to take only 1st argument
    //     ConfigureInventoryItem(storedItem, inventoryItemVisual);
    //     callback?.Invoke(true);
    //     return;
    // }
    //
    // private void AddItemToInventoryGrid(VisualElement item) => inventoryGrid.Add(item);
    // private void RemoveItemFromInventoryGrid(VisualElement item) => inventoryGrid.Remove(item);
    //
    // private static void ConfigureInventoryItem(StoredItem item, ItemVisualElement visual)
    // {
    //     item.RootVisual = visual;
    //     visual.style.visibility = Visibility.Visible;
    // }

}