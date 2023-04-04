using DungeonGame.Utils;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Dungeon
{
    [Serializable, CreateAssetMenu(menuName = "DungeonGame/DungeonConfiguration")]
    public class DungeonConfiguration : ScriptableObject
    {
        public string dungeonName;

        public List<DungeonRoomConfiguration> independentRooms;

        public List<DungeonSectionConfiguration> sections;
    }

    [Serializable]
    public class DungeonRoomConfiguration
    {
        public DungeonRoom room;

        public int minCount;
        public int maxCount;

        [Range(0, 1)]
        public float probability;

        public int minNeighbours;
        public int maxNeighbours;

        public bool isStartRoom;
        public bool isSectorConnection;

        public void CreateRooms(Transform parent, List<DungeonRoom> list, SeedableRandom random)
        {
            int count = minCount;

            for (int i = 0; i < maxCount - minCount; i++)
                if (random.Bool(probability))
                    count++;

            for (int i = 0; i < count; i++)
            {
                DungeonRoom dungeonRoom = DungeonRoom.Instantiate(room, parent);

                dungeonRoom.name = dungeonRoom.roomName;

                list.Add(dungeonRoom);
            }
        }
    }

    [Serializable]
    public class DungeonSectionConfiguration
    {
        public string dungeonSectionName;

        // maybe too much, but maybe define for this min, max and probability?

        public List<DungeonRoomConfiguration> rooms;
    }
}