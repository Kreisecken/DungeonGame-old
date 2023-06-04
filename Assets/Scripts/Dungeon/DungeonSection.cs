using DungeonGame.Dungeon;
using DungeonGame.Utils;
using DungeonGame.Utils.Graph;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DungeonGame.Dungeon
{
    public class DungeonSection : MonoBehaviour
    {
        public Dungeon dungeon;
        public DungeonSectionConfiguration sectionConfig;
        public SeedableRandom random;

        public List<DungeonRoom> rooms;

        [SerializeField]
        public Graph<DungeonRoom> graph;

        [ContextMenu("Update")]
        public void Refresh()
        {
            graph.Refresh();
        }

        public void Generate(Dungeon dungeon, DungeonSectionConfiguration sectionConfig, List<DungeonRoom> independentRooms, bool useAll = false)
        {
            graph = new();

            Init(dungeon, sectionConfig);

            dungeon.CreateRooms(sectionConfig.rooms, rooms, transform);
            UseIndependentRooms(independentRooms, useAll); // seems to take a while

            SetPositionsOfRooms();

            foreach (var room in rooms)
            {
                Vector3 floored = new ((int) room.transform.position.x, (int) room.transform.position.y);
                room.transform.position = floored;
                graph.AddVertex(new(room, floored));
            }

            Triangulate();
            RemoveSomeEdges();

            ApplyGraphToRooms();
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

        public void SetPositionsOfRooms() // slowest
        {
            rooms.Shuffle(random);

            Physics2D.simulationMode = SimulationMode2D.Script;

            List<DungeonRoom> positionatedRooms = new();

            while (rooms.Count > 0)
            {
                Vector3 direction = random.PointInsideUnitCircle();

                var room = rooms[0];

                room.transform.position = positionatedRooms.Count == 0 ? Vector3.zero : positionatedRooms.GetRandom(random).transform.position;

                do
                {
                    room.transform.position += 50 * random.Float32() * direction;
                    Vector3 floored = new((int)room.transform.position.x, (int)room.transform.position.y);
                    room.transform.position = floored;

                    Physics2D.Simulate(1f);
                } while (room.IntersectsWithAnyRoom(positionatedRooms));
                
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

        public void Triangulate()
        {
            graph.Triangulate();
        }
    
        public void RemoveSomeEdges()
        {
            foreach (var edge in graph.edges)
            {
                edge.weight = (edge.a.position - edge.b.position).magnitude;
            }

            List<Edge<DungeonRoom>> removedEdges = graph.MinimumSpanningTree().ToList();

            for (int i = 0, count = (int)(random.Float32() * sectionConfig.cycles * removedEdges.Count); i < count; i++)
                graph.AddEdge(removedEdges.GetAndRemoveRandom(random));
        }
    
        public void ApplyGraphToRooms()
        {
            foreach (var (vertex, edges) in graph.edgesMap)
            {
                foreach (var edge in edges)
                {
                    vertex.data.neighbours.Add(edge.a.data);
                    vertex.data.neighbours.Add(edge.b.data);
                }
            }
        }
    }
}