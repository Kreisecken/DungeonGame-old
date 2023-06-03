using DungeonGame.Utils;
using DungeonGame.Utils.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace DungeonGame.Dungeon
{
    public class DungeonRoom : MonoBehaviour
    {
        public string roomName;

        public DungeonConfiguration config;
        public Dungeon dungeon;

        //public List<DungeonRoom> neighbours;
        public DungeonRoomConnections connections;

        public Triangle<DungeonRoom> triangle;

        public Collider2D collider2d;

        public Tilemap tileMap;

        public void Init(Dungeon dungeon)
        {
            this.dungeon = dungeon;
            this.config = dungeon.config;
        }

        public bool Intersects(DungeonRoom room)
        {
            return collider2d.Distance(room.collider2d).distance < dungeon.config.roomSpacing;
        }

        public bool IntersectsWithAnyRoom(List<DungeonRoom> rooms)
        {
            foreach (var room in rooms)
            {
                if (Intersects(room))
                    return true;
            }

            return false;
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

            foreach (var neighbour in neighbours)
            {
                Gizmos.color = Color.red;

                Vector3 a = new(transform.position.x, neighbour.transform.position.y);

                Gizmos.DrawLine(transform.position, a);
                Gizmos.DrawLine(a, neighbour.transform.position);
            }


            for (float i = 0; i < 360; i += 2.5f)
            {
                Vector3 directionRotated = Quaternion.Euler(0, 0, i) * Vector2.one;

                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionRotated);

                if (hits.Length < 2) continue;

                var hit = hits.OrderBy((hit) => hit.distance).ElementAt(1);


                if (hit.collider != null)
                {
                    if (!neighbours.Contains(hit.collider.gameObject.GetComponent<DungeonRoom>())) continue;

                    Gizmos.DrawLine(transform.position, transform.position + directionRotated * hit.distance*0.7f);
                    Gizmos.DrawCube(new(hit.point.x, hit.point.y, 0), new(1, 1, 1)); 
                }
            }
        }
    }

    public struct DungeonRoomConnections
    {
        public DungeonRoom room;

        public List<DungeonRoom> neighbours;

        public List<DungeonRoomConnection> connections;

        public void Add(DungeonRoom neighbour)
        {
            if (room == neighbour) return;
            if (neighbours.Contains(neighbour)) return;

            neighbour.dungeonRoomConnections        
        }
    }

    public struct DungeonRoomConnection
    {
        public DungeonRoom A;
        public DungeonRoom B;

        public Vector3 tileA;
        public Vector3 tileB;
    }
}