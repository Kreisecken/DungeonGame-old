using DungeonGame.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using DungeonGame.Utils.Graph;

namespace DungeonGame.Dungeons
{
    public class DungeonGenerator
    {
        [Header("Dungeon Generation Configuration")] // TODO: Split DungeonGeneration to DungeonGenerator and DungeonGeneratorConfiguration
        public DungeonConfiguration config;

        public SeedableRandom random;

        public Dungeon dungeon;

        private DungeonGenerator(Dungeon dungeon, DungeonConfiguration config, SeedableRandom random)
        {
            this.config = config;
            this.random = random;
            this.dungeon = dungeon;
        }

        public static Dungeon GenerateDungeon(Transform dungeonParent, DungeonConfiguration config, SeedableRandom random)
        {
            var dungeonGameObject = GameObject.Instantiate(config.dungeonPrefab.gameObject, dungeonParent);

            var dungeon = dungeonGameObject.AddComponent<Dungeon>();
            var dungeonPrefab = dungeonGameObject.GetComponent<DungeonPrefab>();

            dungeon.Init(config, dungeonPrefab, random);

            GameObject.DestroyImmediate(dungeonPrefab);

            DungeonGenerator dungeonGenerator = new(dungeon, config, random);

            dungeonGenerator.GenerateDungeon();

            return dungeon;
        }

        // Destroys entire dungeon, even if it is a subdungeon
        public static void DestroyDungeon(Dungeon dungeon)
        {
            GameObject.DestroyImmediate(dungeon.gameObject);
        }

        public void GenerateDungeon()
        {
            InitializeSubDungeons(out var subDungeons);

            InitializeRooms(out var rooms);

            PlaceRooms(rooms);

            CreateGraph(out var graph);

            Triangulation.Triangulate(graph);

            InitializeEdges(graph);

            InitializeNeighbors(graph);

            MergeTileMaps();
        }

        public void InitializeSubDungeons(out HashSet<Dungeon> subDungeons)
        {
            subDungeons = new();

            foreach (DungeonConfiguration subDungeonConfig in config.subDungeons)
            {
                var subDungeon = GenerateDungeon(dungeon.transform, subDungeonConfig, random);

                subDungeons.Add(subDungeon);
            }
        }

        public void InitializeRooms(out List<DungeonRoom> rooms)
        {
            rooms = new();

            foreach (var roomConfig in config.rooms)
            {
                roomConfig.roomPrefab.Init(config.name);

                // TODO: I don't know if this is the right way to do this, looks like the for loops could be merged

                for (int i = 0; i < roomConfig.minCount; i++)
                    InstantiateRoom(roomConfig, rooms);

                for (int i = roomConfig.minCount; i < roomConfig.maxCount; i++)
                    if (random.Bool(roomConfig.probability))
                        InstantiateRoom(roomConfig, rooms);
            }
        }

        public void InstantiateRoom(DungeonRoomConfiguration roomConfig, List<DungeonRoom> rooms)
        {
            var roomGameObject = GameObject.Instantiate(roomConfig.roomPrefab.gameObject, dungeon.roomsContainerGameObject.transform);

            var room = roomGameObject.AddComponent<DungeonRoom>();

            var roomPrefab = roomGameObject.GetComponent<DungeonRoomPrefab>();

            room.Init(dungeon, roomConfig, roomPrefab);

            GameObject.DestroyImmediate(roomPrefab);
        
            rooms.Add(room);
        }

        public void PlaceRooms(List<DungeonRoom> rooms)
        {
            rooms.Shuffle(random);

            var normalRooms = rooms.FindAll((room) => room.roomConfig.isNormalRoom);

            Physics2D.simulationMode = SimulationMode2D.Script;

            foreach (var room in normalRooms)
            {
                PlaceRoom(room);
            }

            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        }

        public void PlaceRoom(DungeonRoom room)
        {
            Vector3 direction = random.PointInsideUnitCircle(); // optimizing this would be amazing

            int iterations = 0;

            do
            {
                room.transform.position += 50 * random.Float32() * direction;

                Vector3 floored = room.transform.position.Floored();

                room.transform.position = floored;

                Physics2D.Simulate(1f);

                if (iterations++ >= 100000) throw new Exception("Thats not good!");
            } 
            while (room.IntersectsWithAnyRoom(dungeon.rooms));

            dungeon.rooms.Add(room);
        }
    
        public void CreateGraph(out Graph3<DungeonRoom> graph)
        {
            graph = new();

            foreach (var room in dungeon.rooms)
            {
                //Vector3 floored = room.transform.position.Floored();

                //room.transform.position = floored;
                
                graph.AddVertex(new(room, room.transform.position));
            }
        }
    
        public void InitializeEdges(Graph3<DungeonRoom> graph)
        {
            graph.SetWeightsToDistance();

            graph.MinimumSpanningTree(out var removedEdges);

            for (int i = 0; i < removedEdges.Count; i++)
            {
                if (random.Bool(config.cycles))
                {
                    var edge = removedEdges.GetRandom(random);
                    removedEdges.Remove(edge);
                    graph.AddEdge(edge);
                }
            }
        }
    
        public void InitializeNeighbors(Graph3<DungeonRoom> graph)
        {
            foreach (var (vertex, edges) in graph.VerteciesAndEdges)
            {
                foreach (var edge in edges)
                {
                    vertex.data.relations.AddNeighbour(edge.Other(vertex).data);
                }
            }
        }
    
        public void MergeTileMaps()
        {
            var tileMap = dungeon.tileMap;

            foreach (var room in dungeon.rooms)
            {
                var tileMapData = room.TileMapData;
                Vector3Int roomPosition = new((int) room.transform.position.x, (int) room.transform.position.y);

                tileMap.SetTiles
                (
                    tileMapData.positions.Select((position) => position + roomPosition).ToArray(),
                    tileMapData.tilebases
                );
            }
        }
    }
}