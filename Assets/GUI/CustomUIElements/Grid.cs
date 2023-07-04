using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace CustomUIElements
{
    public class Grid : VisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<Grid, UxmlTraits> { }


        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlIntAttributeDescription columns = new UxmlIntAttributeDescription { name = "columns", defaultValue = 3 };
            UxmlIntAttributeDescription rows = new UxmlIntAttributeDescription { name = "rows", defaultValue = 3 };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as Grid;
                ate.columns = columns.GetValueFromBag(bag, cc);
                ate.rows = rows.GetValueFromBag(bag, cc);

                
                
            }

        }

        public int columns { get; set; }
        public int rows { get; set; }

        public Grid()
        {
            //generateVisualContent += OnDrawCanvas;
        }

        /*private void OnDrawCanvas(MeshGenerationContext ctx)
        {
            for (int i = 0; i < columns * rows; i++)
            {
                VisualElement cell = new VisualElement();
                //hierarchy.Add(cell);
                Add(cell);

            }
        }*/
        



    }

}


