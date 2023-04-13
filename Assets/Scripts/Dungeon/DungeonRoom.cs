using DungeonGame.Utils;
using System;
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

        public DungeonConfiguration config;
        public Dungeon dungeon;

        public List<DungeonRoom> neighbours;
        [SerializeField]
        public Triangle<DungeonRoom> triangle;

        public Collider2D collider2d;

        public void Init(Dungeon dungeon)
        {
            this.dungeon = dungeon;
            this.config = dungeon.config;
        }

        public bool Intersects(DungeonRoom room)
        {
            return collider2d.Distance(room.collider2d).distance < dungeon.config.roomSpacing;
        }

        private void OnDrawGizmos()
        {
            foreach (var neighbour in neighbours)
            {
                Gizmos.DrawLine(transform.position, neighbour.transform.position);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (triangle == null) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(triangle.circumCircle.center, triangle.circumCircle.radius);
        }
    }
}