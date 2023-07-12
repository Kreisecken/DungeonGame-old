using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DungeonGame.Items;

namespace DungeonGame.Enemies
{
    public class EnemyController : Entity
    {
        [Header("Player Detection")]
        public Transform orientation;
        [Min(0f)] public float hearRange = 5f;
        [Min(0f)] public float seeRange = 10f;
        [Range(0f, 360f)] public float seeAngle = 90f;
        [Min(0f)] public float detectionDelay = 1f; // delay between detections
        
        [Header("Movement")]
        public Rigidbody2D rb;
        public float speed = 3f;
        [Min(0f)] public float minPlayerDistance = 2f;
        [Min(0f)] public float maxPlayerDistance = 5f;
        
        [Header("Inventory")]
        public WeaponProperties weaponProperties;
        
        private Weapon weapon;
        public Transform target;
        private float detectionCD = 0f; // seconds since last detection attempt
        
        void OnDrawGizmosSelected()
        {
            if(orientation == null) return;
            
            float leftAngle = (orientation.rotation.eulerAngles.z + (seeAngle / 2f)) * Mathf.Deg2Rad;
            float rightAngle = (orientation.rotation.eulerAngles.z - (seeAngle / 2f)) * Mathf.Deg2Rad;
            Vector3 leftPoint = new Vector3(Mathf.Cos(leftAngle), Mathf.Sin(leftAngle), 0f);
            Vector3 rightPoint = new Vector3(Mathf.Cos(rightAngle), Mathf.Sin(rightAngle), 0f);
            Handles.DrawWireDisc(orientation.position, Vector3.back, hearRange);
            Handles.DrawWireArc(orientation.position, Vector3.back, leftPoint, seeAngle, seeRange);
            Handles.DrawLine(orientation.position, orientation.position + leftPoint * seeRange);
            Handles.DrawLine(orientation.position, orientation.position + rightPoint * seeRange);
        }
        
        new void Start()
        {
            base.Start();
            
            weapon = new Weapon(weaponProperties, orientation, this);
        }
        
        private void FixedUpdate()
        {
            if(target == null)
            {
                if(weapon != null) weapon.trigger = false;
                
                detectionCD += Time.fixedDeltaTime;
                if(detectionCD >= detectionDelay)
                {
                    detectionCD = 0f;
                    target = Detect();
                }
            }
            else
            {
                if(weapon != null) weapon.trigger = true;
                
                // TODO: implement better path finding
                // movement
                Vector3 delta = target.position - transform.position;
                orientation.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
                
                if(delta.magnitude > maxPlayerDistance)
                    rb.MovePosition(transform.position + delta.normalized * Time.fixedDeltaTime * speed);
                else if(delta.magnitude < minPlayerDistance)
                    rb.MovePosition(transform.position - delta.normalized * Time.fixedDeltaTime * speed);
            }
            
            if(weapon != null) weapon.WeaponUpdate(Time.fixedDeltaTime);
        }
        
        protected Transform Detect()
        {
            // detection by hearing
            Collider2D[] hearColliders = Physics2D.OverlapCircleAll(transform.position, hearRange);
            foreach(Collider2D c in hearColliders)
            {
                if(!c.gameObject.TryGetComponent<Entity>(out Entity e)) continue;
                if(e.team == team) continue;
                
                return e.transform;
            }
            
            // detection by seeing
            Collider2D[] seeColliders = Physics2D.OverlapCircleAll(transform.position, seeRange);
            foreach(Collider2D c in seeColliders)
            {
                if(!c.gameObject.TryGetComponent<Entity>(out Entity e)) continue;
                if(e.team == team) continue;
                
                // check angle to player
                Vector2 delta = c.transform.position - transform.position;
                float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
                if(Mathf.Abs(angle) > seeAngle) continue;
                
                return e.transform;
            }
            
            return null;
        }
        
        override protected void OnDamage(int amount, DamageType damageType, Entity originEntity)
        {
            target = originEntity.transform;
        }
        
    }
}
