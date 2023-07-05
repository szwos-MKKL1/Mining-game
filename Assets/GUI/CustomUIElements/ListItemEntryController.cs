using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUIElements
{
    public class ItemListEntryController
    {
        Label nameLabel;

        public void SetVisualElement(VisualElement visualElement)
        {
            nameLabel = visualElement.Q<Label>("item-display-name");
        }

        public void setItemData(ItemData itemData)
        {
            nameLabel.text = itemData.displayName;
        }


    }
}


