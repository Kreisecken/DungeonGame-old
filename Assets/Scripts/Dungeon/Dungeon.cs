using DungeonGame.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DungeonGame.Dungeon
{
    public class Dungeon : MonoBehaviour
    {
        public DungeonConfiguration config;
         
        public DungeonRoom startRoom;
         
        public List<DungeonSection> sections;

        public SeedableRandom random;

        public string seed;

        public Tilemap tileMap;

        private static long a;

        [ContextMenu("DungeonGenerator/TestGeneration")]
        public void TestGeneration()
        {
            GenerateDungeon(config, seed == "" ? a++ : seed.MD5HashCode());
        }

        public void GenerateDungeon(DungeonConfiguration config, SeedableRandom random)
        {
            // pretty slow and its not even done yet...

            Init(config, random);

            DestroyDungeon();

            List<DungeonRoom> independentRooms = new();

            CreateRooms(config.independentRooms, independentRooms);

            GenerateSections(independentRooms);

            MergeTilemaps();

            CreateCorridors();
        }

        public void Init(DungeonConfiguration config, SeedableRandom random)
        {
            this.config = config;
            this.random = random;
        }

        public void CreateRooms(List<DungeonRoomConfiguration> roomConfigs, List<DungeonRoom> list, Transform parent = null)
        {
            foreach (var roomConfig in roomConfigs)
                CreateRooms(roomConfig, list, parent);
        }

        public void GenerateSections(List<DungeonRoom> independentRooms)
        {
            for (int i = 0; i < config.sections.Count; i++)
            {
                GameObject sectionGameObject = new("section");

                sectionGameObject.transform.parent = transform;

                var section = sectionGameObject.AddComponent<DungeonSection>();

                section.Generate(this, config.sections[i], independentRooms, i == config.sections.Count - 1);

                independentRooms.Shuffle(random);

                sections.Add(section);
            }
        }

        public void DestroyDungeon()
        {
            sections.Clear();

            List<Transform> children = transform.Cast<Transform>().ToList();

            foreach (Transform child in children)
                DestroyImmediate(child.gameObject);
        }

        public void CreateRooms(DungeonRoomConfiguration config, List<DungeonRoom> list, Transform parent = null)
        {
            for (int i = 0; i < config.minCount; i++)
                list.Add(InstantiateRoom(config.prefab, parent));

            for (int i = config.minCount; i < config.maxCount; i++)
                if (random.Bool(config.probability))
                    list.Add(InstantiateRoom(config.prefab, parent));
        }

        private DungeonRoom InstantiateRoom(GameObject roomPrefab, Transform parent = null)
        {
            GameObject gameObject = Instantiate(roomPrefab, parent != null ? parent : transform);

            DungeonRoom room = gameObject.GetComponent<DungeonRoom>();

            room.Init(this);

            gameObject.name = room.name;

            return room;
        }
    
        public void MergeTilemaps()
        {
            GameObject tileMapGameObject = Instantiate(config.tileMapPrefab, transform);

            tileMap = tileMapGameObject.GetComponentInChildren<Tilemap>();

            foreach (var dungeonSection in sections)
            {
                foreach (var dungeonRoom in dungeonSection.rooms)
                {
                    foreach (var position in dungeonRoom.tileMap.cellBounds.allPositionsWithin)
                    {
                        if (!dungeonRoom.tileMap.HasTile(position)) continue;

                        Vector3 translatedPosition = position + dungeonRoom.transform.position;
                        Debug.Log(dungeonRoom.tileMap.GetTile(position));
                        tileMap.SetTile(new Vector3Int((int) translatedPosition.x, (int) translatedPosition.y, 0), dungeonRoom.tileMap.GetTile(position));
                    }

                    // TODO: remove TileMap Children
                }
            }
        }
     
        public void CreateCorridors()
        {
            foreach (var dungeonSection in sections)
            {
                foreach (var edge in dungeonSection.graph.edges)
                {
                    var a = edge.a.data;
                    var b = edge.b.data;

                    Vector3Int position = new((int)a.transform.position.x, (int)a.transform.position.y);
                    Vector3Int neighbourPosition = new((int)b.transform.position.x, (int)b.transform.position.y);

                    Vector3Int direction = new
                    (
                        -Math.Sign(a.transform.position.x - b.transform.position.x),
                        -Math.Sign(a.transform.position.y - b.transform.position.y)
                    );

                    Vector3Int length = new
                    (
                        Math.Abs(position.x - neighbourPosition.x),
                        Math.Abs(position.y - neighbourPosition.y)
                    );

                    for (int x = 0; x < length.x; x++)
                    {
                        position.x += direction.x;

                        tileMap.SetTile(position, config.tile);
                    }

                    for (int y = 0; y < length.y; y++)
                    {
                        position.y += direction.y;

                        tileMap.SetTile(position, config.tile);
                    }
                }
            }
        }
    }
}