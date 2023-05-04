using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [Min(0)] public int maxHealth;
        
        private int health;
        
        void Start()
        {
            health = maxHealth;
        }
        
        public void damage(int dmg)
        {
            // TODO: add more damage calculations here (damage type, resistance, ...)
            health -= dmg;
            
            // TODO: add death animation here
            if(health <= 0) Destroy(gameObject);
        }
    }
}
