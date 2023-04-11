using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }
    
    [CreateAssetMenu(fileName = "NewProjectile", menuName = "DungeonGame/Projectile")]
    public class ProjectileType : ScriptableObject
    {
        [Min(0f)] public float speed = 5f;
        [Min(0f)] public float maxLifeTime = 5f;
        public Sprite sprite;
    }
}
