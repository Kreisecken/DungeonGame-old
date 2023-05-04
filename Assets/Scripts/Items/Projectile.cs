using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Items
{
    public class Projectile : MonoBehaviour
    {
        public ProjectileType type { get; }
        private float lifeTime = 0f;
        
        void Start()
        {
            
        }

        void FixedUpdate()
        {
            // TODO: add collisions
            transform.position += transform.right * type.speed * Time.fixedDeltaTime;
            
            Debug.Log(type.speed);
            
            lifeTime += Time.fixedDeltaTime;
            if(lifeTime > type.maxLifeTime) Destroy(gameObject);
        }
        
        void OnCollisionStay2D(Collision2D collision)
        {
            // TODO: implement player / enemy damage
            // damage player / enemy (if one was hit)
            
            // aoe
            if(type.aoe)
            {
                Collider2D[] aoeCollisions = Physics2D.OverlapCircleAll(transform.position, type.aoeRadius);
                foreach(Collider2D c in aoeCollisions)
                {
                    if(c.gameObject != collision.gameObject /* && c.gameObject is player / enemy */)
                    {
                        // damage player / enemy (c)
                    }
                }
            }
            
            Destroy(gameObject);
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
