using DungeonGame.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGame.Dungeon
{
    public class Dungeon : MonoBehaviour
    {
        public DungeonConfiguration config;

        public DungeonRoom startRoom;
         
        public List<DungeonSection> sections;

        public SeedableRandom random;

        public string seed;

        private static long a = 0;

        [ContextMenu("DungeonGenerator/TestGeneration")]
        public void TestGeneration()
        {
            GenerateDungeon(config, (seed == "" ? a++ : seed.MD5HashCode()));
        }

        public void GenerateDungeon(DungeonConfiguration config, SeedableRandom random)
        {
            // pretty slow and its not even done yet...

            Init(config, random);

            DestroyDungeon();

            List<DungeonRoom> independentRooms = new();

            CreateRooms(config.independentRooms, independentRooms);

            GenerateSections(independentRooms);
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
    }
}