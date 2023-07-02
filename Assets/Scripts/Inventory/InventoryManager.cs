using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    
    public GameObject contentObject;
    public GameObject inventoryObject;
    public GameObject itemObjectPrefab;
    
    public List<Item> items = new List<Item>(); //This List includes every Item. 
    //The idea is that you can choose whether you want to see all Items together or sorted in groups.
    // We have to be careful that the items dont dublicate though.
    public List<Item> weapons = new List<Item>();
    public List<Item> food = new List<Item>();
    public List<Item> questItems = new List<Item>();
    public List<Item> rewards = new List<Item>();
    private void Awake(){
        instance=this;
    }

    public void Add(Item item){
        items.Add(item);
    }
public void addWeapons(Item item){
    items.Add(item);
    weapons.Add(item);
}
    
    public void addFood(Item item){
items.Add(item);
food.Add(item);
    }
    public void addQuestItems(Item item){
items.Add(item);
questItems.Add(item);
    }
public void addRewards(Item item){
items.Add(item);
rewards.Add(item);
    }

     public void Remove(Item item){
        items.Remove(item);
    }
public void RemoveWeapons(Item item){
    items.Remove(item);
    weapons.Remove(item);
}
    
    public void RemoveFood(Item item){
items.Remove(item);
food.Remove(item);
    }
    public void RemoveQuestItems(Item item){
items.Remove(item);
questItems.Remove(item);
    }
public void RemoveRewards(Item item){
items.Remove(item);
rewards.Remove(item);
    }

    public void ShowInventory() {
        if(inventoryObject.activeSelf) return;
        inventoryObject.SetActive(true);
        foreach(Item item in items) {
            GameObject itemObject = Instantiate(itemObjectPrefab);
            itemObject.transform.GetChild(0).GetComponent<TMP_Text>().text = item.properties.itemName;
            itemObject.transform.GetChild(1).GetComponent<Image>().sprite = item.properties.icon;
            itemObject.transform.SetParent(contentObject.transform);
        }
    }
    
    public void HideInventory() {
        inventoryObject.SetActive(false);
        foreach(Transform t in contentObject.transform) {
            GameObject.Destroy(t.gameObject);
        }
    }
    
}
