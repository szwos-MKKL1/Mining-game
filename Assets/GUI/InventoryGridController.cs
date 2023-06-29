using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryGridController : MonoBehaviour
{
    public int columns;
    public int rows;


    private VisualElement gridContainer;
    private UIDocument uIDocument;

    // Start is called before the first frame update
    void Start()
    {
        uIDocument = GetComponent<UIDocument>();

        gridContainer = uIDocument.rootVisualElement.Q<VisualElement>("Grid");
    

        //TEST
        gridContainer.Add(new Button());

        populateGrid();
    }

    private void populateGrid()
    {

        for(int i = 0; i < rows; i++)
        {
            VisualElement row = new VisualElement();
            row.style.backgroundColor = Color.white;
            for(int j = 0; j < columns; j++)
            {
                VisualElement cell = new VisualElement();

                //TODO: derive all of this from css
                cell.style.borderBottomColor = Color.black;
                cell.style.borderLeftColor = Color.black;
                cell.style.borderTopColor = Color.black;
                cell.style.borderRightColor = Color.black;
                cell.style.borderBottomWidth = 1;
                cell.style.borderTopWidth = 1;
                cell.style.borderRightWidth = 1;
                cell.style.borderLeftWidth = 1;
                row.Add(cell);
            }

        }
        


    }

}
