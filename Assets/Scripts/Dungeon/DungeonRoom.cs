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
    }
}