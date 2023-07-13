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
            name = itemData.displayName;
            style.width = 50;
            style.height = 50;
            
            style.visibility = Visibility.Visible;

            VisualElement icon = new VisualElement
            {
                style = { backgroundImage = itemData.icon.texture, flexGrow = 1f}
            };
            Add(icon);

            // //TODO: what does this do?
            // icon.AddToClassList("visual-icon");
            // AddToClassList("visual-icon-container");

        }

        public void SetPosition(Vector2 pos)
        {
            style.left = pos.x;
            style.top = pos.y;
        }
    }
}


