using DungeonGame.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGame.Dungeon
{
    public class Dungeon : MonoBehaviour
    {
        public DungeonConfiguration configuration;

        public DungeonRoom startRoom;
         
        public List<DungeonSection> sections;

        public SeedableRandom random;

        public string seed;

        [ContextMenu("DungeonGenerator/TestGeneration")]
        public void TestGeneration()
        {
            GenerateDungeon(configuration, seed);
        }

        [ContextMenu("DungeonGenerator/TestRandom")]
        public void TestRandom()
        {
            random ??= "Hello World";

            Debug.Log(random.Int32(5));

            for (int i = 0; i < 10; i++)
            {
                Debug.Log(random.PointInsideUnitCircle());
            }
        }

        public void GenerateDungeon(DungeonConfiguration config, SeedableRandom random)
        {
            DestroyDungeon();
        
            this.configuration = config;
            this.random        = random;

            List<DungeonRoom> independentRooms = new();

            CreateRooms(config.independentRooms, ref independentRooms);

            GenerateSections(ref independentRooms);
        }

        public void CreateRooms(List<DungeonRoomConfiguration> roomConfigs, ref List<DungeonRoom> list, Transform parent = null)
        {
            list ??= new();

            foreach (var roomConfig in roomConfigs)
                CreateRooms(roomConfig, list, parent);
        }

        public void GenerateSections(ref List<DungeonRoom> independentRooms)
        {
            foreach (var sectionConfiguration in configuration.sections)
            {
                GameObject sectionGameObject = new("section");
                sectionGameObject.transform.parent = transform;

                DungeonSection section = sectionGameObject.AddComponent<DungeonSection>();

                section.Generate(this, independentRooms, sectionConfiguration);

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
                list.Add(InstantiateRoom(config.room, parent));

            for (int i = config.minCount; i < config.maxCount; i++)
                if (random.Bool(config.probability))
                    list.Add(InstantiateRoom(config.room, parent));
        }

        private DungeonRoom InstantiateRoom(DungeonRoom room, Transform parent = null)
        {
            GameObject gameObject = Instantiate(room.gameObject, parent != null ? parent : transform);

            gameObject.name = room.name;

            return gameObject.GetComponent<DungeonRoom>();
        }
    }
}