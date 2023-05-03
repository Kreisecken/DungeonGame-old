using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DungeonGame.Items;

namespace DungeonGame.Enemies
{
    public class EnemyController : MonoBehaviour
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
        public WeaponType weaponType;
        
        private Weapon weapon;
        public Transform target;
        private float detectionCD = 0f; // seconds since last detection attempt
        
        private void OnDrawGizmosSelected()
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
        
        private void Start()
        {
            weapon = new Weapon(weaponType, orientation);
        }
        
        private void FixedUpdate()
        {
            if(target == null)
            {
                weapon.trigger = false;
                // TODO: implement player detection
            }
            else
            {
                weapon.trigger = true;
                
                // TODO: implement better path finding
                // movement
                Vector3 delta = target.position - transform.position;
                orientation.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
                
                if(delta.magnitude > maxPlayerDistance)
                    rb.MovePosition(transform.position + delta.normalized * Time.fixedDeltaTime * speed);
                else if(delta.magnitude < minPlayerDistance)
                    rb.MovePosition(transform.position - delta.normalized * Time.fixedDeltaTime * speed);
            }
            
            weapon.weaponUpdate(Time.fixedDeltaTime);
        }
    }
}
