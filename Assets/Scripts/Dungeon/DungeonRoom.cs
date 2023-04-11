using DungeonGame.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DungeonGame.Dungeon
{
    public class DungeonRoom : MonoBehaviour
    {
        public string roomName;

        public DungeonRoomConfiguration config;
        public Dungeon dungeon;

        public List<DungeonRoom> neighbours;

        public bool positionFixed;

        public List<Collider2D> colliders;

        public Vector3 direction;

        public Vector2 size;

        private void Awake()
        {
            colliders = new(GetComponents<Collider2D>());
        }

        public bool Intersects(DungeonRoom room)
        {
            return GetComponents<Collider2D>().Any((colliderA) =>
            {
                return room.GetComponents<Collider2D>().Any((colliderB) =>
                {
                    // Collider.Distance does not work here (see ColliderTest)
                    return colliderA.Distance(colliderB).distance < 0;
                });
            });
        }

        public void Move(float factor)
        {
            transform.position += direction * factor;
        }
    }
}