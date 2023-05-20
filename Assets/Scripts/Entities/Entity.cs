using System.Collections;
using System.Collections.Generic;
using DungeonGame.Items;
using UnityEngine;

namespace DungeonGame.Enemies
{
    public class Entity : MonoBehaviour
    {
        [Min(0)] public int maxHealth;
        public Team team = Team.Enemies;
        
        protected int health;
        
        void Start()
        {
            health = maxHealth;
        }
        
        public void Damage(int amount, DamageType damageType)
        {
            // TODO: add more damage calculations here (damage type, resistance, ...)
            health -= amount;
            
            if(health <= 0) Destroy(gameObject);
        }
    }
}
