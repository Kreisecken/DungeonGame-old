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
using UnityEngine.Tilemaps;

namespace DungeonGame.Dungeon
{
    public class DungeonSection : MonoBehaviour
    {
        public Dungeon dungeon;
        public DungeonSectionConfiguration sectionConfig;
        public SeedableRandom random;

        public List<DungeonRoom> rooms;

        [SerializeField]
        public Graph3<DungeonRoom> graph;

        public Tilemap tileMap;

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

            MergeTileMaps();

            CreateConnectionPoints();
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
            Triangulation.Triangulate(graph);
        }
    
        public void RemoveSomeEdges()
        {
            foreach (var edge in graph.edges)
            {
                edge.weight = (edge.a.position - edge.b.position).magnitude;
            }

            List<Edge3<DungeonRoom>> removedEdges = graph.MinimumSpanningTree().ToList();

            for (int i = 0, count = (int)(random.Float32() * sectionConfig.cycles * removedEdges.Count); i < count; i++)
                graph.AddEdge(removedEdges.GetAndRemoveRandom(random));
        }
    
        public void ApplyGraphToRooms()
        {
            foreach (var (vertex, edges) in graph.edgesMap)
            {
                foreach (var edge in edges)
                {
                    vertex.data.relations.Add(vertex.data == edge.a.data ? edge.b.data : edge.a.data);
                }
            }
        }
    
        public void MergeTileMaps()
        {
            GameObject tileMapGameObject = Instantiate(dungeon.config.tileMapPrefab, transform);

            tileMap = tileMapGameObject.GetComponentInChildren<Tilemap>();

            foreach (var dungeonRoom in rooms)
            {
                foreach (var position in dungeonRoom.tileMap.cellBounds.allPositionsWithin)
                {
                    if (!dungeonRoom.tileMap.HasTile(position)) continue;

                    Vector3 translatedPosition = position + dungeonRoom.transform.position;
                    Debug.Log(dungeonRoom.tileMap.GetTile(position));
                    tileMap.SetTile(new Vector3Int((int) translatedPosition.x, (int) translatedPosition.y, 0), dungeonRoom.tileMap.GetTile(position));
                }

                // TODO: change to Destroy()

                DestroyImmediate(dungeonRoom.tileMap.gameObject.transform.parent.gameObject);

                dungeonRoom.tileMap = tileMap;
            }
        }
 
        public void CreateConnectionPoints()
        {
            var remainingRooms = rooms.ToList();

            while (remainingRooms.Count() > 0)
            {
                var room = remainingRooms[0];

                Dictionary<DungeonRoom, List<Vector3>> neighbourHits = new();

                for (float i = 0; i < 360; i += 1)
                {
                    Vector3 directionRotated = Quaternion.Euler(0, 0, i) * Vector2.one;

                    RaycastHit2D[] rayCastHits = Physics2D.RaycastAll(room.transform.position, directionRotated);

                    if (rayCastHits.Length < 2) continue;

                    var hits = rayCastHits.Where((hit) => 
                        hit.collider != room.collider2d && 
                        room.Neighbours.Contains(hit.collider.gameObject.GetComponent<DungeonRoom>())
                    );

                    Debug.Log(hits.Count());

                    if (hits.Count() == 0) continue;

                    var hit = hits.ElementAt(0);

                    if (!hit.collider.TryGetComponent(out DungeonRoom neighbour)) continue;

                    if (!neighbourHits.ContainsKey(neighbour))
                        neighbourHits.Add(neighbour, new());

                    neighbourHits[neighbour].Add(hit.point);
                }

                foreach (var (neighbour, hits) in neighbourHits)
                {
                    Vector3 neighbourHitPoint = hits.GetRandom(random);
                    
                    List<Vector3> roomHits = new();

                    for (float i = 0; i < 360; i += 1)
                    {
                        Vector3 directionRotated = Quaternion.Euler(0, 0, i) * Vector2.one;

                        RaycastHit2D[] rayCastHits = Physics2D.RaycastAll(neighbourHitPoint, directionRotated);

                        if (rayCastHits.Length < 2) continue;

                        var rayCastHitsWithRoom = rayCastHits.Where((hit) => hit.collider == room.collider2d);

                        Debug.Log(rayCastHitsWithRoom.Count() + "abc");

                        if (rayCastHitsWithRoom.Count() == 0) continue;

                        roomHits.Add(rayCastHitsWithRoom.ElementAt(0).point);
                    }

                    Vector3 roomHitPoint = roomHits.GetRandom(random);

                    Debug.Log(roomHitPoint);
                    Debug.Log(room.tileMap.WorldToCell(roomHitPoint));

                    var connection = room.relations.connections.Find((connection) => connection.A == neighbour || connection.B == neighbour);

                    var roundedRoomHitPoint = new Vector3Int((int) Mathf.Round(roomHitPoint.x), (int) Mathf.Round(roomHitPoint.y), 0);
                    var roundedNeighbourHitPoint = new Vector3Int((int) Mathf.Round(neighbourHitPoint.x), (int) Mathf.Round(neighbourHitPoint.y), 0);

                    if (connection.A == room)
                    {
                        connection.AConnectionPoint = roomHitPoint;
                        connection.BConnectionPoint = neighbourHitPoint;
                    }
                    else
                    {
                        connection.AConnectionPoint = roomHitPoint;
                        connection.BConnectionPoint = neighbourHitPoint;
                    }
                }

                remainingRooms.RemoveAt(0);
            }
        }
    }
}