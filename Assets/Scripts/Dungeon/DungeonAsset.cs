using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Dungeon
{
    // prototyping the DungeonGenerator
    // the idea is the following:
    // create a DungeonAsset with all the informations for generating a Dungeon
    // to generate the Dungeon, we can pass a Generate() method the
    // asset and a seed to generate a Dungeon
    // giving the player the ability to use a seed
    // would be kinda op, so maybe the player could
    // spend money, clear the dungeon once or
    // clear the dungeon once on the hardest difficulty?

    [CreateAssetMenu(menuName = "DungeonGame/Dungeon Asset")]
    public class DungeonAsset : ScriptableObject
    {
        public List<DungeonRoomAsset> rooms;
    }

    [Serializable]
    public class DungeonRoomAsset
    {
        public DungeonRoom dungeonPrefab; // DungeonRoom or GameObject?

        public int minCount;
        public int maxCount;

        [Range(0, 1)]
        public float probability;

        public List<DungeonRoomAsset> additionalRequiredRooms;
    }
}