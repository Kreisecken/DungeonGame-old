using DungeonGame.Dungeon;
using DungeonGame.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Dungeon
{
    public class DungeonSection : MonoBehaviour
    {
        public Dungeon dungeon;
        public DungeonSectionConfiguration sectionConfig;
        public SeedableRandom random;

        public List<DungeonRoom> rooms;


        public void Generate(Dungeon dungeon, List<DungeonRoom> independentRooms, DungeonSectionConfiguration sectionConfig)
        {
            random = dungeon.random;
            this.sectionConfig = sectionConfig;

            dungeon.CreateRooms(sectionConfig.rooms, ref rooms);

            PlaceRoomsRandomly();
            UseIndependentRooms(independentRooms);
            dungeon.CreateRooms(sectionConfig.rooms, ref rooms);

            SetPositionsOfRooms();
        }

        public void PlaceRoomsRandomly()
        {
            foreach (DungeonRoom room in rooms)
                room.transform.position = random.PointInsideUnitCircle();
        }

        public void UseIndependentRooms(List<DungeonRoom> independentRooms)
        {
            int count = random.Int32(independentRooms.Count);

            for (int i = 0; i < count; i++)
            {
                rooms.Add(independentRooms[i]);
                independentRooms.RemoveAt(i);
            }
        }

        public void SetPositionsOfRooms()
        {
            rooms.Shuffle(random);

            /*
             * Pseudo Code
             * 
             * list rooms;
             * 
             * foreach (var room in rooms)
             * {
             *     room.done = true;
             *     
             *     while (rooms collide with rooms[0])
             *     {
             *         move colliding rooms until all collisions are resolved
             *     }
             * }
             */
        }
    }
}