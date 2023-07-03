using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item
{
    public ItemProperties properties { get; }
    public int count;
    
    public Item(ItemProperties properties, int count = 1)
    {
        this.properties = properties;
        this.count = count;
    }
}
