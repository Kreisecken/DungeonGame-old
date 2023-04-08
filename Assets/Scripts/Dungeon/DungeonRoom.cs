using DungeonGame.Utils;
using System.Collections;
using System.Collections.Generic;
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
    }
}