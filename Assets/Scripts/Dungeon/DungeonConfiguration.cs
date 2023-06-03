using DungeonGame.Utils;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DungeonGame.Dungeon
{
    [Serializable, CreateAssetMenu(menuName = "DungeonGame/DungeonConfiguration")]
    public class DungeonConfiguration : ScriptableObject
    {
        public string dungeonName;

        [Min(0)]
        public float roomSpacing;

        public List<DungeonRoomConfiguration> independentRooms;

        public List<DungeonSectionConfiguration> sections;

        public GameObject tileMapPrefab;

        public TileBase tile;
    }

    [Serializable]
    public class DungeonRoomConfiguration
    {
        public GameObject prefab;

        public int minCount;
        public int maxCount;

        [Range(0, 1)]
        public float probability;

        public bool isStartRoom;
        public bool isSectorConnection;
    }

    [Serializable]
    public class DungeonSectionConfiguration
    {
        // maybe too much, but maybe define for this min, max and probability?
        public string dungeonSectionName;

        [Range(0, 1)]
        public float cycles;

        public List<DungeonRoomConfiguration> rooms;
    }
}