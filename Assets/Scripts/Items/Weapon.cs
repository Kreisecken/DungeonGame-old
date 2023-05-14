using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Items
{
    public class Weapon
    {
        public WeaponProperties properties { get; }
        public bool trigger = false;
        private float fireCD = 0f; // fire cool down
        private Transform projectileOrigin;
        
        public Weapon(WeaponProperties properties, Transform projectileOrigin)
        {
            this.properties = properties;
            this.projectileOrigin = projectileOrigin;
        }
        
        public void WeaponUpdate(float deltaTime)
        {
            if(fireCD > 0f) fireCD -= deltaTime;
            
            if(trigger && fireCD <= 0f)
            {
                fireCD += properties.fireDelay;
                
                // fire
                SpawnProjectile();
            }
        }
        
        private void SpawnProjectile()
        {
            properties.projectileType.createProjectile(projectileOrigin);
        }
    }
}
