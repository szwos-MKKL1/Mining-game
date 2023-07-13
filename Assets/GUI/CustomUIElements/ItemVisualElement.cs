using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUIElements
{
    public class ItemVisualElement : VisualElement
    {
        private readonly ItemData itemData;

        public ItemVisualElement(ItemData itemData)
        {
            this.itemData = itemData;
            name = $"{itemData.displayName}";
            style.height = PlayerInventory.slotHeight;
            style.width = PlayerInventory.slotWidth;
            style.visibility = Visibility.Hidden;

            VisualElement icon = new VisualElement
            {
                style = { backgroundImage = itemData.icon.texture }
            };
            Add(icon);

            //TODO: what does this do?
            icon.AddToClassList("visual-icon");
            AddToClassList("visual-icon-container");

        }

        public void SetPosition(Vector2 pos)
        {
            style.left = pos.x;
            style.top = pos.y;
        }
    }
}


