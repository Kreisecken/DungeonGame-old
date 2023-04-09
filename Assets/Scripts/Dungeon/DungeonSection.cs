using DungeonGame.Dungeon;
using DungeonGame.Utils;
using System.Collections.Generic;
using System.Linq;
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

            dungeon.CreateRooms(sectionConfig.rooms, ref rooms, transform);
            UseIndependentRooms(independentRooms);

            PlaceRoomsRandomly();

            SetPositionsOfRooms();
            //Triangulate();
        }

        public void PlaceRoomsRandomly()
        {
            foreach (DungeonRoom room in rooms)
                room.transform.position = random.PointInsideUnitCircle();
        }

        public void UseIndependentRooms(List<DungeonRoom> independentRooms)
        {
            int count = random.Int32(independentRooms.Count - 1);

            for (int i = 0; i < count; i++)
            {
                var independentRoom = independentRooms[0];
                independentRooms.RemoveAt(0);

                independentRoom.transform.parent = transform;

                rooms.Add(independentRoom);
            }
        }

        public void SetPositionsOfRooms()
        {
            rooms.Shuffle(random);

            foreach (var room in rooms)
                room.direction = random.PointInsideUnitCircle();

            foreach (var room in rooms)
            {
                room.positionFixed = true;

                // maybe redefine direction to a value inside an arc which would not intersect with fixedRooms?
                // but on a second thought, this does not make sense

                GetIntersectingRooms(room, out var intersectingRooms);

                foreach (var intersectingRoom in intersectingRooms)
                {
                    int iterations = 0;
                    while (room.Intersects(intersectingRoom) && ++iterations < 1000)
                        intersectingRoom.Move(random.Float());

                    intersectingRoom.Move(intersectingRoom.size.magnitude * random.Float() * 1.5f);
                }
            }
        }

        public void GetIntersectingRooms(DungeonRoom roomA, out List<DungeonRoom> intersectingRooms)
        {
            intersectingRooms = new();

            foreach (var roomB in rooms)
            {
                if (roomA == roomB) continue;

                if (roomA.Intersects(roomB))
                    intersectingRooms.Add(roomB);
            }
        }
    }
}