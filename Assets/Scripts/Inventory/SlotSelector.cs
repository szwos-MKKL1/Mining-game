using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataStructures;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine.UIElements;
using System;

namespace Inventory
{
    public class SlotSelector
    {
        private BoundedQueue<ItemVisual> activeItems = new BoundedQueue<ItemVisual> (2);
        private ObservableCollection<ItemVisual> allItems;
      
        public SlotSelector(List<ItemVisual> items)
        {
            allItems = new ObservableCollection<ItemVisual>(items);
            allItems.CollectionChanged += updateItems;
            activeItems.CollectionChanged += updateItems;
        }

        public void Add(ItemVisual item)
        {
            item.clicked += () => Select(item);
            allItems.Add (item);
        }

        public void Remove(ItemVisual item)
        {
            item.clicked -= () => Select(item);
            allItems.Remove (item);
        }

        private void Select(ItemVisual sender)
        {
            if(sender.IsActive)
            {
                activeItems.Remove(sender);
            } else
            {
                activeItems.Enqueue(sender);
            }
        }

        //TODO: this could be shortened with LINQ perhaps
        public List<ItemVisual> GetActives()
        {
            List<ItemVisual> actives = new List<ItemVisual>();
            foreach (ItemVisual item in activeItems) 
            {
                actives.Add(item);
            }

            return actives;
        }

        private void updateItems(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (ItemVisual item in allItems)
            {
                if(activeItems.Contains(item))
                {
                    item.IsActive = true;
                } else
                {
                    item.IsActive = false;
                }
            }
        }

    }
}
