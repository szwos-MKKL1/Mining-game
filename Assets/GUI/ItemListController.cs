using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class ItemListController
{
    private ListView itemList;
    private VisualTreeAsset listEntryTemplate;

    private List<string> items = new List<string>(100);

    public void Init(VisualElement root, VisualTreeAsset listElementTemplate)
    {
        listEntryTemplate = listElementTemplate;

        itemList = root.Q<ListView>("ItemList");

        FillItemList();
    }

    public void FillItemList()
    {
        //TODO: take items from Inventory component instead
        //TODO: (or expose List, so that it will get dynamically updated, when Inventory component updates it's list)
        const int itemCount = 100;
        for (int i = 0; i < itemCount; i++)
        {
            //listView.Add(new Label(i.ToString()));
            //items[i] = i.ToString();
            items.Add(i.ToString());
        }


        itemList.makeItem = () => new Label("TEST");
        itemList.bindItem = (item, index) => { };

        itemList.fixedItemHeight = 22;

        itemList.itemsSource = items;
    }

    /*public void Start()
    { 
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        
        if (listView == null)
            listView = root.Q<ListView>("ItemList");

        Debug.Log(listView.ToString());


        //TODO: take items from Inventory component instead
        //TODO: (or expose List, so that it will get dynamically updated, when Inventory component updates it's list)
        const int itemCount = 100;
        var items = new List<string>(itemCount);
        for (int i = 0; i < itemCount; i++)
        {
            //listView.Add(new Label(i.ToString()));
            items[i] = i.ToString();
        }


        listView.bindItem = (item, index) => new Label(items[index]);

        //TODO: check if this is working
        listView.selectionType = SelectionType.Multiple;

        listView.itemsChosen += objects => Debug.Log(objects);
        listView.selectionChanged += objects => Debug.Log(objects);

    }*/
}



