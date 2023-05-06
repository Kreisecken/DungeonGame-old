using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Player
{
    public class PlayerScript : MonoBehaviour
    {
        [Min(0)] public int maxHealth;
        
        private int health;
        
        void Start()
        {
            health = maxHealth;
        }
        
        public void Damage(int dmg)
        {
            // TODO: add more damage calculations here (damage type, resistance, ...)
            health -= dmg;
            
            if(health <= 0) Destroy(gameObject);
        }
    }
}
