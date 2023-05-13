using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGame.Items;

namespace DungeonGame.Player
{
    public class PlayerScript : MonoBehaviour
    {
        [Min(0)] public int maxHealth;
        public Transform weaponDirection;
        public WeaponType testWeaponTypeBecauseWeaponsCanNotBeCollectedYet;
        
        private int health;
        private Weapon weapon;
        
        void Start()
        {
            health = maxHealth;
            
            // TODO: collect weapons instead of just having them
            //weapon = new Weapon(testWeaponTypeBecauseWeaponsCanNotBeCollectedYet, weaponDirection);
        }
        
        void FixedUpdate()
        {
            if(weapon != null) weapon.WeaponUpdate(Time.fixedDeltaTime);
        }
        
        public void Damage(int amount, DamageType damageType)
        {
            // TODO: add more damage calculations here (damage type, resistance, ...)
            health -= amount;
            
            if(health <= 0) Destroy(gameObject);
        }
        
        public void SetWeaponTrigger(bool trigger)
        {
            if(weapon != null) weapon.trigger = trigger;
        }
        
        public void SetWeaponDirection(Vector2 direction)
        {
            weaponDirection.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
    }
}
