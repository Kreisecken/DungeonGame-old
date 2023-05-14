using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGame.Player;
using UnityEditor;

namespace DungeonGame.Items
{
    public class Projectile : MonoBehaviour
    {
        public ProjectileProperties properties;
        private float lifeTime = 0f;
        
        void Start()
        {
            
        }

        void FixedUpdate()
        {
            // TODO: add collisions
            transform.position += transform.right * properties.speed * Time.fixedDeltaTime;
            
            lifeTime += Time.fixedDeltaTime;
            if(lifeTime > properties.maxLifeTime) Destroy(gameObject);
        }
        
        void OnCollisionStay2D(Collision2D collision)
        {
            // TODO: weapons should be able to damage enemies (-> enum for teams ?)
            if(collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerScript>().Damage(properties.damage, properties.damageType);
                
                // aoe
                if(properties.aoe)
                {
                    DoAOEDamage(collision);
                }
            }
            
            Destroy(gameObject);
        }
        
        private void DoAOEDamage(Collision2D collision)
        {
            Collider2D[] aoeCollisions = Physics2D.OverlapCircleAll(transform.position, properties.aoeRadius);
            foreach(Collider2D c in aoeCollisions)
            {
                if(c.gameObject != collision.gameObject && c.gameObject.CompareTag("Player"))
                    c.gameObject.GetComponent<PlayerScript>().Damage(properties.aoeDamage, properties.aoeDamageType);
            }
        }
    }
}
