using DungeonGame.Dungeon;
using DungeonGame.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace DungeonGame.Dungeon
{
    public class DungeonSection : MonoBehaviour
    {
        public Dungeon dungeon;
        public DungeonSectionConfiguration sectionConfig;
        public SeedableRandom random;

        public List<DungeonRoom> rooms;

        public void Generate(Dungeon dungeon, DungeonSectionConfiguration sectionConfig, List<DungeonRoom> independentRooms, bool useAll = false)
        {
            Init(dungeon, sectionConfig);

            dungeon.CreateRooms(sectionConfig.rooms, rooms, transform);
            UseIndependentRooms(independentRooms, useAll);

            PlaceRoomsRandomly();
            SetPositionsOfRooms();
            Triangulate();
            // SpanningTree();
            // Corridors(); 
            // etc.
        }

        public void Init(Dungeon dungeon, DungeonSectionConfiguration sectionConfig)
        {
            this.dungeon = dungeon;
            this.sectionConfig = sectionConfig;
            this.random = dungeon.random;

            rooms = new();
        }

        public void UseIndependentRooms(List<DungeonRoom> independentRooms, bool useAll)
        {
            if (useAll)
            {
                foreach (var room in independentRooms)
                {
                    rooms.Add(room);
                    room.transform.parent = transform;
                }

                independentRooms.Clear();

                return;
            }

            int count = random.Int32(independentRooms.Count - 1);

            while (count > 0)
            {
                var independentRoom = independentRooms[0];
                independentRooms.RemoveAt(0);

                independentRoom.transform.parent = transform;

                rooms.Add(independentRoom);
            }
        }

        public void PlaceRoomsRandomly()
        {
            foreach (DungeonRoom room in rooms)
                room.transform.position = random.PointInsideUnitCircle();
        }

        public void SetPositionsOfRooms()
        {
            rooms.Shuffle(random);

            Physics2D.simulationMode = SimulationMode2D.Script;

            List<DungeonRoom> positionatedRooms = new();

            while (rooms.Count > 0)
            {
                Vector3 direction = random.PointInsideUnitCircle();

                var room = rooms[0];

                room.transform.position = positionatedRooms.Count == 0 ? Vector3.zero : positionatedRooms.GetRandom(random).transform.position;

                List<DungeonRoom> intersectingRooms;

                do
                {
                    room.transform.position += 50 * random.Float() * direction;
                    Physics2D.Simulate(1f);

                    GetIntersectingRooms(room, positionatedRooms, out intersectingRooms);
                } while (intersectingRooms.Count > 0);

                rooms.RemoveAt(0);

                positionatedRooms.Add(room);
            }

            rooms = positionatedRooms;

            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        }

        public void GetIntersectingRooms(DungeonRoom roomA, List<DungeonRoom> rooms, out List<DungeonRoom> intersectingRooms)
        {
            intersectingRooms = new();

            foreach (var roomB in rooms)
            {
                if (roomA == roomB) continue;

                if (roomA.Intersects(roomB))
                    intersectingRooms.Add(roomB);
            }
        }

        List<Triangle<DungeonRoom>> triangles;

        public void Triangulate()
        {
            triangles = Triangulation.Triangulate(rooms, room => room.transform.position);

            foreach (var triangle in triangles)
            {
                foreach (var edge in triangle.edges)
                {
                    var roomA = edge.a.data;
                    var roomB = edge.b.data;

                    roomA.triangle = triangle;
                    roomB.triangle = triangle;

                    if (roomA.neighbours.Contains(roomB)) continue;

                    roomA.neighbours.Add(roomB);
                    roomB.neighbours.Add(roomA);
                }
            }
        }
    }
}