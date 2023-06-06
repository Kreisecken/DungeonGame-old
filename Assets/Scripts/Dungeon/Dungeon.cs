using DungeonGame.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DungeonGame.Utils.Graph;

namespace DungeonGame.Dungeons
{
    public class Dungeon : MonoBehaviour // Create a Prefab containing this script // chnage of plans, use DungeonPrefab instead
    {
        [Header("Dungeon Configuration")]
        public DungeonConfiguration config;

        public SeedableRandom random;

        [Header("Dungeon Rooms")]
        public DungeonRoom entranceRoom;
        
        public HashSet<DungeonRoom> rooms; // No order needed, so HashSet could be better

        [Header("Dungeon Relations")]

        public DungeonRelations relations;

        [Header("Dungeon Objects")]

        public Tilemap tileMap;

        public Transform roomsContainerGameObject;

        public void Init(DungeonConfiguration config, DungeonPrefab dungeonPrefab, SeedableRandom random)
        {
            this.config = config;
            this.random = random;

            rooms = new();
            relations = new(this);

            tileMap = dungeonPrefab.tileMap;
            roomsContainerGameObject = dungeonPrefab.roomsContainerGameObject;
        }
    }

    public class DungeonRelations
    {
        public Dungeon dungeon;

        public Dictionary<Dungeon, DungeonConnection> DungeonConnectionMap; // best of both worlds :D

        public IEnumerable<Dungeon>           Neighbours  => DungeonConnectionMap.Keys;
        public IEnumerable<DungeonConnection> Connections => DungeonConnectionMap.Values;
        
        public DungeonRelations(Dungeon dungeon)
        {
            this.dungeon = dungeon;

            DungeonConnectionMap = new();
        }

        public void AddNeighbour(DungeonConnection connection)
        {
            DungeonConnectionMap.Add(connection.neighbour, connection);
        }

        public void RemoveNeighbour(Dungeon neighbour)
        {
            DungeonConnectionMap.Remove(neighbour);
        }
    }

    public class DungeonConnection
    {
        public Dungeon dungeon;
        public Dungeon neighbour;

        public DungeonRoomConnection connection;

        public DungeonConnection ConnectionFromNeighbour => neighbour.relations.DungeonConnectionMap[dungeon];

        public DungeonConnection(Dungeon dungeon, Dungeon neighbour, DungeonRoomConnection connection)
        {
            this.dungeon = dungeon;
            this.neighbour = neighbour;

            this.connection = connection;
        }
    }
}