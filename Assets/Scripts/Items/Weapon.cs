using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Items
{
    // TODO: use the inventory system (extend form Item)
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "DungeonGame/Weapon")]
    public class Weapon : ScriptableObject
    {
        public float fireDelay = 0.5f;
        public ProjectileType projectileType;
        
        public void spawnProjectile(Transform startTransform)
        {
            // TODO: use a GameObject Queue for better performance (?)
            GameObject go = new GameObject("Projectile");
            go.transform.position = startTransform.position;
            go.transform.rotation = startTransform.rotation;
            go.AddComponent<SpriteRenderer>();
            go.GetComponent<SpriteRenderer>().sprite = projectileType.sprite;
            go.AddComponent<Projectile>();
            go.GetComponent<Projectile>().type = projectileType;
        }
    }
}
