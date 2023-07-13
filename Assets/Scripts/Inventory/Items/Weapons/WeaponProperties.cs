using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Items
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "DungeonGame/Weapon")]
    public class WeaponProperties : ItemProperties
    {
        public float fireDelay = 0.5f;
        public ProjectileProperties projectileType;
        
        override public void InventoryClick(Item item)
        {
            InventoryManager.instance.equipInActiveSlot(ActiveSlot.WEAPON, item);
        }
    }
}
