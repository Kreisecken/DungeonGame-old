using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    private Item item;
    
    public void Remove()
    {
        InventoryManager.instance.Remove(item);
    }
    
    public void UpdateItemSlot(Item item)
    {
        this.item = item;
        transform.GetChild(0).GetComponent<TMP_Text>().text = item.properties.itemName;
        transform.GetChild(1).GetComponent<Image>().sprite = item.properties.icon;
        transform.GetChild(3).GetChild(0).GetComponent<TMP_Text>().text = item.count.ToString();
    }
    
    public void UpdateDeleteButton(bool visible)
    {
        transform.GetChild(2).gameObject.SetActive(visible);
    }
    
    public void InventoryClick()
    {
        item.properties.InventoryClick(item);
    }
}
