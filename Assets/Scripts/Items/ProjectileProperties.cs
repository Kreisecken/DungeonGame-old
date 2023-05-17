using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGame.Enemies;

namespace DungeonGame.Items
{
    [CreateAssetMenu(fileName = "NewProjectile", menuName = "DungeonGame/Projectile")]
    public class ProjectileProperties : ScriptableObject
    {
        public Sprite sprite;
        public Vector2 size;
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
        
        public GameObject createProjectile(Transform origin, Team originTeam)
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
            collider.size = size;
            Projectile projectileScript = g.AddComponent<Projectile>();
            projectileScript.properties = this;
            projectileScript.originTeam = originTeam;
            
            return null;
        }
    }
}
