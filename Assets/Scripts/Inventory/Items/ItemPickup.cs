using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    //Make sure that there is a collider in the prefab!!
    //when there is a coin or something on the screen and you want to pick it up and add it to the inventory
    public ItemProperties itemProperties;
    void Pickup()
    {
        InventoryManager.instance.Add(new Item(itemProperties));
        Destroy(gameObject);
    }
    
    private void OnMouseDown()
    {
        Pickup();
    }
}

