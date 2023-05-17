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
        
        void OnCollisionStay2D(Collision2D collision)
        {
            // TODO: use a seperate class for bein in a team for both players and enemies (?)
            
            if(collision.gameObject.TryGetComponent<PlayerScript>(out PlayerScript ps))
            {
                if(ps.team == originTeam) return;
                
                ps.Damage(properties.damage, properties.damageType);
                
                // aoe
                if(properties.aoe) DoAOEDamage(collision);
            }
            else if(collision.gameObject.TryGetComponent<EnemyController>(out EnemyController ec))
            {
                if(ec.team == originTeam) return;
                
                // TODO: enemy damage
                //ec.Damage(properties.damage, properties.damageType);
                
                // aoe
                if(properties.aoe) DoAOEDamage(collision);
            }
            
            Destroy(gameObject);
        }
        
        private void DoAOEDamage(Collision2D collision)
        {
            Collider2D[] aoeCollisions = Physics2D.OverlapCircleAll(transform.position, properties.aoeRadius);
            foreach(Collider2D c in aoeCollisions)
            {
                if(c.gameObject == collision.gameObject) continue;
                
                if(c.gameObject.TryGetComponent<PlayerScript>(out PlayerScript ps))
                {
                    if(ps.team != originTeam) ps.Damage(properties.aoeDamage, properties.aoeDamageType);
                }
                
                /*else if(c.gameObject.TryGetComponent<EnemyController>(out EnemyController ec))
                {
                    if(ec.team != originTeam) ec.Damage(properties.aoeDamage, properties.aoeDamageType);
                }*/
            }
        }
    }
}
