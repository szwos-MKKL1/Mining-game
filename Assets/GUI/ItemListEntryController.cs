using UnityEngine.UIElements;

public class ItemListEntryController
{
    Label nameLabel;

    public void SetVisualElement(VisualElement visualElement)
    {
        nameLabel = visualElement.Q<Label>("item-display-name");
    }

    //TODO: rename to SetItemName or SetItemDisplayName
    public void SetItemData(ItemData itemData)
    {
        nameLabel.text = itemData.displayName;
    }
}