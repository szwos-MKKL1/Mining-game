using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CollectItems : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ICollectible collectible = collision.GetComponent<ICollectible>();

        InventoryController.Instance.CollectItem(collectible.GetItemData());


        //bool itemCollected = InventoryController.Instance.CollectItem?.Invoke(collectible.GetItemData());
        //InventoryController.Instance.InvokeCollectItem(collectible.GetItemData());




        /*if(collectible != null)
        {
            Task.Run(() => { collectible.Collect(); } );
        }*/
    }
}
