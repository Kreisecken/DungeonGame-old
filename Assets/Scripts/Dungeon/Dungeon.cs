using DungeonGame.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Dungeon
{
    public class Dungeon : MonoBehaviour
    {
        public DungeonConfiguration configuration;

        public DungeonRoom startRoom;
         
        public List<DungeonRoom> independentRooms;
        public List<DungeonSection> sections;

        public SeedableRandom random;

        [ContextMenu("DungeonGenerator/TestGeneration")]
        public void TestGeneration()
        {
            GenerateDungeon(configuration, "Hello, World!");
        }

        public void GenerateDungeon(DungeonConfiguration config, SeedableRandom random)
        {
            DestroyDungeon();
        
            this.configuration = config;
            this.random        = random;

            CreateRooms(config.independentRooms, ref independentRooms);

            GenerateSections(independentRooms);
        }

        public void CreateRooms(List<DungeonRoomConfiguration> roomConfigs, ref List<DungeonRoom> list)
        {
            list ??= new();

            foreach (var roomConfig in roomConfigs)
                CreateRooms(list, roomConfig);
        }

        public void GenerateSections(List<DungeonRoom> independentRooms)
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

            foreach (Transform child in transform)
                DestroyImmediate(child.gameObject);
        }

        public void CreateRooms(List<DungeonRoom> list, DungeonRoomConfiguration config)
        {
            for (int i = 0; i < config.minCount; i++)
                list.Add(InstantiateRoom(config.room));

            for (int i = config.minCount; i < config.maxCount; i++)
                if (random.Bool(config.probability))
                    list.Add(InstantiateRoom(config.room));
        }

        private DungeonRoom InstantiateRoom(DungeonRoom room, Transform parent = null)
            => Instantiate(room.gameObject, parent != null ? parent : transform)
               .GetComponent<DungeonRoom>();
    }
}