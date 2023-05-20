using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DungeonGame.Player;
using DungeonGame.Enemies;

namespace DungeonGame.Items
{
    public class Projectile : MonoBehaviour
    {
        public ProjectileProperties properties;
        public Team originTeam;
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
        
        void OnTriggerStay2D(Collider2D collider)
        {
            // check if an Entity was hit
            if(collider.gameObject.TryGetComponent<Entity>(out Entity entity))
            {
                if(entity.Damage(properties.damage, properties.damageType, originTeam))
                {
                    // aoe
                    if(properties.aoe) DoAOEDamage(collider);
                    
                    // remove projectile
                    Destroy(gameObject);
                }
            }
        }
        
        protected void DoAOEDamage(Collider2D collider)
        {
            Collider2D[] aoeCollisions = Physics2D.OverlapCircleAll(transform.position, properties.aoeRadius);
            foreach(Collider2D c in aoeCollisions)
            {
                // don't apply aoe damage to the directly hit Entity
                if(c.gameObject == collider.gameObject) continue;
                
                if(collider.gameObject.TryGetComponent<Entity>(out Entity entity))
                {
                    entity.Damage(properties.aoeDamage, properties.aoeDamageType, originTeam);
                }
            }
        }
    }
}
