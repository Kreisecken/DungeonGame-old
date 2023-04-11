using System.Collections;
using UnityEngine;

namespace DungeonGame.Dungeon
{
    [ExecuteInEditMode]
    public class ColliderTest : MonoBehaviour
    {
        public Collider2D colliderA;
        public Collider2D colliderB;

        [ContextMenu("Test")]
        private void Test()
        {
            if (colliderA == null || colliderB == null) return;

            Debug.Log(colliderA.Distance(colliderB).distance);

            colliderA.transform.position += Vector3.right * 5;

            Debug.Log(Physics2D.simulationMode);
            
            Physics2D.simulationMode = SimulationMode2D.Script;
            Physics2D.Simulate(0.01f);
            // Not Sure here
            Physics2D.simulationMode = SimulationMode2D.FixedUpdate; 

            Debug.Log(colliderA.Distance(colliderB).distance);
        }
    }
}