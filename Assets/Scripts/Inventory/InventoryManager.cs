using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    
    public Transform contentTransform;
    public GameObject inventoryObject;
    public GameObject itemObjectPrefab;
    public Toggle enableDeleteButton;
    
    public List<Item> items = new List<Item>(); //This List includes every Item. 
    //The idea is that you can choose whether you want to see all Items together or sorted in groups.
    // We have to be careful that the items dont dublicate though.
    public List<Item> weapons = new List<Item>();
    public List<Item> food = new List<Item>();
    public List<Item> questItems = new List<Item>();
    public List<Item> rewards = new List<Item>();
    private void Awake()
    {
        instance = this;
    }
    
    private void AddToList(List<Item> list, Item item)
    {
        // try to add the items to an existing slot
        foreach (Item i in list)
        {
            ItemProperties p = item.properties;
            if (i.properties == p && (p.maxCount < 0 || i.count + item.count <= p.maxCount))
            {
                i.count += item.count;
                return;
            }
        }
        
        // else create a new slot
        list.Add(item);
    }
    public void Add(Item item)
    {
        AddToList(items, item);
        
        // update inventory if visible
        if(inventoryObject.activeSelf) UpdateContent(); // TODO: make this more efficient
    }
    public void addWeapons(Item item)
    {
        Add(item);
        AddToList(weapons, item);
    }
    public void addFood(Item item)
    {
        Add(item);
        AddToList(food, item);
    }
    public void addQuestItems(Item item)
    {
        Add(item);
        AddToList(questItems, item);
    }
    public void addRewards(Item item)
    {
        Add(item);
        AddToList(rewards, item);
    }
    
    public void Remove(Item item)
    {
        items.Remove(item);
        
        // update inventory if visible
        if(inventoryObject.activeSelf) UpdateContent(); // TODO: make this more efficient
    }
    public void RemoveWeapons(Item item)
    {
        Remove(item);
        weapons.Remove(item);
    }
    public void RemoveFood(Item item)
    {
        Remove(item);
        food.Remove(item);
    }
    public void RemoveQuestItems(Item item)
    {
        Remove(item);
        questItems.Remove(item);
    }
    public void RemoveRewards(Item item)
    {
        Remove(item);
        rewards.Remove(item);
    }
    
    public void SetInventoryVisible(bool visible)
    {
        if(visible) UpdateContent();
        inventoryObject.SetActive(visible);
    }
    
    private void UpdateContent()
    {
        int contentLength = contentTransform.childCount;
        int itemsLength = items.Count;
        // keep existing item objects
        for (int i = 0; i < Mathf.Min(contentLength, itemsLength); i++)
        {
            UpdateItemSlot(items[i], contentTransform.GetChild(i).gameObject);
        }
        // remove excess objects if necessary
        if (contentLength > itemsLength)
        {
            for (int i = itemsLength; i < contentLength; i++)
            {
                GameObject.Destroy(contentTransform.GetChild(i).gameObject);
            }
        }
        // add new objects if necessary
        if (itemsLength > contentLength)
        {
            for (int i = contentLength; i < itemsLength; i++)
            {
                CreateItemSlot(items[i]);
            }
        }
    }
    
    public void CreateItemSlot(Item item)
    {
        GameObject itemObject = Instantiate(itemObjectPrefab);
        UpdateItemSlot(item, itemObject);
        itemObject.transform.SetParent(contentTransform);
    }
    
    private void UpdateItemSlot(Item item, GameObject itemObject)
    {
        InventoryItem inventoryItem = itemObject.GetComponent<InventoryItem>();
        inventoryItem.UpdateItemSlot(item);
        inventoryItem.UpdateDeleteButton(enableDeleteButton.isOn);
    }
    
    public void UpdateDeleteButtons()
    {
        bool visible = enableDeleteButton.isOn;
        foreach(Transform t in contentTransform)
        {
            t.GetComponent<InventoryItem>().UpdateDeleteButton(visible);
        }
    }
    
}
