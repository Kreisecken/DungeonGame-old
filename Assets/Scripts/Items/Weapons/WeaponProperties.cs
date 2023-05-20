using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Items
{
    // TODO: use the inventory system (extend form Item)
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "DungeonGame/Weapon")]
    public class WeaponProperties : ScriptableObject
    {
        public float fireDelay = 0.5f;
        public ProjectileProperties projectileType;
    }
}
