using DungeonGame.Utils;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DungeonGame.Dungeons
{
    [Serializable, CreateAssetMenu(menuName = "DungeonGame/Create DungeonConfiguration")]
    public class DungeonConfiguration : ScriptableObject
    {
        [Header("Data")]
        
        public string dungeonName;
        //public string dungeonDescription; etc...

        [Header("Generation Optiones")]

        public DungeonPrefab dungeonPrefab;

        [Min(0)]
        public float roomSpacingMin;
        [Min(0)]
        public float rommSpacingMax;

        [Range(0, 1)]
        public float cycles;

        [Header("Rooms")]

        public List<DungeonRoomConfiguration> rooms;
        
        [Header("Sub Dungeons")]

        public List<DungeonConfiguration> subDungeons;

        //public int orderOfCompletion;

        [Header("Tilemap Configuration")]

        public TileBase tile;
    }

    [Serializable]
    public class DungeonRoomConfiguration
    {
        [Header("Data")]

        public string roomName;

        public DungeonRoomPrefab roomPrefab;

        [Header("Generation Options")]

        public int minCount;
        public int maxCount;

        [Range(0, 1)]
        public float probability;

        [Header("Flags")]

        public bool isStartRoom;          // is this one of the possible starting rooms?
        public bool isDungeonConnection;  // is this room a connection to another dungeon? 
        //public bool isIndepentent;        // is this room independent from the rest of the dungeon? implement later if necessary

        public bool isNormalRoom => !isStartRoom && !isDungeonConnection;
    }
}