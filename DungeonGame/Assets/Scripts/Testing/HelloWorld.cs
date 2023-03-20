using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DungeonGame 
{
    public class HelloWorld : MonoBehaviour
    {
        void Start()
        {
            
        }

        void Update()
        {
            Debug.Log("Hello World!");
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 1);
        }
    }
}