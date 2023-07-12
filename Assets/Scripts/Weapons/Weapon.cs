using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGame.Enemies;

namespace DungeonGame.Items
{
    public class Weapon
    {
        public WeaponProperties properties { get; }
        public bool trigger = false;
        private Entity owner;
        private bool isHeld = false;
        private float fireCD = 0f; // fire cool down
        private Transform projectileOrigin;
        
        public Weapon(WeaponProperties properties)
        {
            this.properties = properties;
            isHeld = false;
            owner = null;
        }
        
        public Weapon(WeaponProperties properties, Transform projectileOrigin, Entity owner)
        {
            this.properties = properties;
            PickUp(projectileOrigin, owner);
        }
        
        public void PickUp(Transform projectileOrigin, Entity owner)
        {
            this.projectileOrigin = projectileOrigin;
            this.owner = owner;
            isHeld = true;
        }
        
        public void WeaponUpdate(float deltaTime)
        {
            if(fireCD > 0f) fireCD -= deltaTime;
            
            if(trigger && fireCD <= 0f)
            {
                fireCD += properties.fireDelay;
                
                // fire
                Shoot();
            }
        }
        
        protected void Shoot()
        {
            properties.projectileType.CreateProjectile(projectileOrigin, owner);
        }
    }
}
