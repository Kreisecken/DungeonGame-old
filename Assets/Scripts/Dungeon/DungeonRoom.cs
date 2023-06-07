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

namespace DungeonGame.Dungeons
{
    public class DungeonRoom : MonoBehaviour
    {
        // TODO: These need to be initialized somehow later trough the data in the DungeonPrefab object
        public DungeonRoomConfiguration roomConfig; 
        public DungeonConfiguration config;
        public Dungeon dungeon;

        public DungeonRoomRelations relations;

        public Collider2D collider2d;

        public string Name => roomConfig.roomName;

        public (Vector3Int[] positions, TileBase[] tilebases) TileMapData => DungeonRoomPrefab.TileMapData[config.name];

        public void Init(Dungeon dungeon, DungeonRoomConfiguration roomConfig, DungeonRoomPrefab roomPrefab)
        {
            this.dungeon = dungeon;
            this.roomConfig = roomConfig;
            this.config = dungeon.config;
            this.collider2d = roomPrefab.collider2d;

            relations = new(this);

            DestroyImmediate(roomPrefab.tileMap.transform.parent.gameObject);
        }
    
        public bool Intersects(DungeonRoom room)
        {
            return collider2d.Distance(room.collider2d).distance < dungeon.random.Float32(config.roomSpacingMin, config.rommSpacingMax);
        }

        public bool IntersectsWithAnyRoom(HashSet<DungeonRoom> rooms)
        {
            return rooms.Any(Intersects);
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(collider2d.bounds.center, collider2d.bounds.size);

            Gizmos.color = Color.green;

            foreach (var neighbour in relations.Neighbours)
            {
                Gizmos.DrawLine(collider2d.bounds.center, neighbour.collider2d.bounds.center);
            }

            //Debug.Log(relations.Neighbours.Count());
        }
    }

    public class DungeonRoomRelations
    {
        public DungeonRoom room;

        public BetterDictionary<DungeonRoom, DungeonRoomConnection> DungeonRoomConnections; // best of both worlds :D

        public IEnumerable<DungeonRoom>           Neighbours  => DungeonRoomConnections.Keys;
        public IEnumerable<DungeonRoomConnection> Connections => DungeonRoomConnections.Values;

        public DungeonRoomRelations(DungeonRoom room)
        {
            this.room = room;

            DungeonRoomConnections = new();
        }

        public void AddNeighbour(DungeonRoom neighbour)
        {
            DungeonRoomConnections[neighbour] ??= new(room, neighbour);
            //DungeonRoomConnections.Add(neighbour, new(room, neighbour)); // this does not work
        }

        public void RemoveNeighbour(DungeonRoom neighbour)
        {
            DungeonRoomConnections.Remove(neighbour);
        }
    }

    public class DungeonRoomConnection
    {
        [Header("References")]

        public DungeonRoom room;
        public DungeonRoom neighbor;

        [Header("Door Configuration")]

        public DungeonRoomDoor Door;

        public DungeonRoomDoor NeighborDoor => ConnectionFromNeighbor.Door;

        public DungeonRoomConnection ConnectionFromNeighbor => neighbor.relations.DungeonRoomConnections[room];

        public DungeonRoomConnection(DungeonRoom room, DungeonRoom neighbor)
        {
            this.room = room;
            this.neighbor = neighbor;
        }
    }
    
    public class DungeonRoomDoor // maybe this should be a struct?
    {
        public Vector3Int[] tiles;
        public Vector3Int center;

        public DungeonRoomDoor(Vector3Int[] tiles, Vector3Int center)
        {
            this.tiles = tiles;
            this.center = center;
        }
    }
}