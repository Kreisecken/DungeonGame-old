using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonGame.Enemies 
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Player Detection")]
        public Transform orientation;
        [Min(0f)] public float hearRange = 5;
        [Min(0f)] public float seeRange = 10;
        [Range(0f, 360f)] public float seeAngle = 90;
        
        private void OnDrawGizmos()
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
            
        }
        
        private void Update()
        {
            
        }
    }
}
