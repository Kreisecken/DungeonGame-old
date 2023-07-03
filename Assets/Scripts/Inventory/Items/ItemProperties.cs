using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewItem", menuName ="Item/Create New Item")]
public class ItemProperties : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite icon;
    public int maxCount = -1; // -1 for no limit
}
