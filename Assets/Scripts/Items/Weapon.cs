using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Items
{
    public class Weapon
    {
        public WeaponType type { get; }
        public bool trigger = false;
        private float fireCD = 0f; // fire cool down
        private Transform projectileOrigin;
        
        public Weapon(WeaponType type, Transform projectileOrigin)
        {
            this.type = type;
            this.projectileOrigin = projectileOrigin;
        }
        
        public void WeaponUpdate(float deltaTime)
        {
            if(fireCD > 0f) fireCD -= deltaTime;
            
            if(trigger && fireCD <= 0f)
            {
                fireCD += type.fireDelay;
                
                // fire
                SpawnProjectile();
            }
        }
        
        private void SpawnProjectile()
        {
            // TODO: use a GameObject Queue for better performance (?)
            GameObject go = GameObject.Instantiate(type.projectileType.prefab);
            go.transform.position = projectileOrigin.position;
            go.transform.rotation = projectileOrigin.rotation;
        }
    }
    
    // TODO: use the inventory system (extend form Item)
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "DungeonGame/Weapon")]
    public class WeaponType : ScriptableObject
    {
        public float fireDelay = 0.5f;
        public ProjectileType projectileType;
    }
}
