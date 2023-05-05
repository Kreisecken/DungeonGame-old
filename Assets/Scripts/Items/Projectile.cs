using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGame.Player;

namespace DungeonGame.Items
{
    public class Projectile : MonoBehaviour
    {
        public ProjectileType type;
        private float lifeTime = 0f;
        
        void Start()
        {
            
        }

        void FixedUpdate()
        {
            // TODO: add collisions
            transform.position += transform.right * type.speed * Time.fixedDeltaTime;
            
            lifeTime += Time.fixedDeltaTime;
            if(lifeTime > type.maxLifeTime) Destroy(gameObject);
        }
        
        void OnCollisionStay2D(Collision2D collision)
        {
            // TODO: implement player / enemy damage
            // damage player / enemy (if one was hit)
            
            // TODO: weapons should be able to damage enemies (-> enum for teams ?)
            if(collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerScript>().Damage(type.damage);
                
                // aoe
                if(type.aoe)
                {
                    DoAOEDamage(collision);
                }
            }
            
            Destroy(gameObject);
        }
        
        private void DoAOEDamage(Collision2D collision)
        {
            Collider2D[] aoeCollisions = Physics2D.OverlapCircleAll(transform.position, type.aoeRadius);
            foreach(Collider2D c in aoeCollisions)
            {
                if(c.gameObject != collision.gameObject && c.gameObject.CompareTag("Player"))
                    c.gameObject.GetComponent<PlayerScript>().Damage(type.aoeDamage);
            }
        }
    }
    
    [CreateAssetMenu(fileName = "NewProjectile", menuName = "DungeonGame/Projectile")]
    public class ProjectileType : ScriptableObject
    {
        public GameObject prefab;
        public int damage = 5; // TODO: data type of hp???0
        public bool aoe = false;
        public float aoeRadius = 2f;
        public int aoeDamage = 3;
        [Min(0f)] public float speed = 5f;
        [Min(0f)] public float maxLifeTime = 5f;
    }
}
