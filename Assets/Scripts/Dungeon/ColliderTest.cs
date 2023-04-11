using System.Collections;
using UnityEngine;

namespace DungeonGame.Dungeon
{
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
            Debug.Log(colliderA.Distance(colliderB).distance); // still same distance as before...
        }
    }
}