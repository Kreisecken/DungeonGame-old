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
        private Team ownerTeam;
        private bool isHeld = false;
        private float fireCD = 0f; // fire cool down
        private Transform projectileOrigin;
        
        public Weapon(WeaponProperties properties)
        {
            this.properties = properties;
            isHeld = false;
        }
        
        public Weapon(WeaponProperties properties, Transform projectileOrigin, Team ownerTeam)
        {
            this.properties = properties;
            PickUp(projectileOrigin, ownerTeam);
        }
        
        public void PickUp(Transform projectileOrigin, Team ownerTeam)
        {
            this.projectileOrigin = projectileOrigin;
            this.ownerTeam = ownerTeam;
            isHeld = true;
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
