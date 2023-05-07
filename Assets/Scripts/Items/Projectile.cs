using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGame.Player;
using UnityEditor;

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
            // TODO: weapons should be able to damage enemies (-> enum for teams ?)
            if(collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerScript>().Damage(type.damage, type.damageType);
                
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
                    c.gameObject.GetComponent<PlayerScript>().Damage(type.aoeDamage, type.aoeDamageType);
            }
        }
    }
    
    [CreateAssetMenu(fileName = "NewProjectile", menuName = "DungeonGame/Projectile")]
    public class ProjectileType : ScriptableObject
    {
        public Sprite sprite;
        [Min(0f)] public float speed = 5f;
        [Min(0f)] public float maxLifeTime = 5f;
        
        [Header("Damage")]
        public int damage = 5;
        public DamageType damageType = DamageType.Magic;
        
        [Header("AOE")]
        public bool aoe = false;
        public float aoeRadius = 2f;
        public int aoeDamage = 3;
        public DamageType aoeDamageType = DamageType.Explosion;
        
        public GameObject createProjectile(Transform origin)
        {
            // TODO: use a GameObject Queue for better performance (?)
            // add Projectile GameObject
            GameObject g = new GameObject("Projectile");
            g.transform.position = origin.position;
            g.transform.rotation = origin.rotation;
            
            // add components
            SpriteRenderer spriteRenderer = g.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            BoxCollider2D collider = g.AddComponent<BoxCollider2D>();
            Projectile projectileScript = g.AddComponent<Projectile>();
            projectileScript.type = this;
            
            return null;
        }
    }
}
