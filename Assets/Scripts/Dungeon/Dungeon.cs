using DungeonGame.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DungeonGame.Dungeon
{
    public class Dungeon : MonoBehaviour
    {
        public DungeonConfiguration configuration;

        public DungeonRoom startRoom;
        public List<DungeonSection> sections;

        public SeedableRandom random;

        [ContextMenu("DungeonGenerator/TestGeneration")]
        public void TestGeneration()
        {
            GenerateDungeon(configuration, "Hello, World!");
        }

        public void GenerateDungeon(DungeonConfiguration configuration, SeedableRandom random)
        {
            DestroyDungeon();
        
            this.configuration = configuration;
            this.random        = random;

            CreateRooms();
            PlaceRoomsRandomly();
            // LayOutRooms()
            // DelaunayTriangulation()
            // GenerateMinimumSpanningTree()
            // AddSomeEdgesFromDelauneyBack(float percentage)
            // GenerateHallways()
            // Done() ?
        }

        public void CreateRooms()
        {
            foreach (var sectionConfiguration in configuration.sections)
            {
                GameObject gameObject = new(sectionConfiguration.dungeonSectionName);
                DungeonSection section = gameObject.AddComponent<DungeonSection>();

                section.transform.parent = transform;

                foreach (var roomConfiguration in sectionConfiguration.rooms)
                {
                    roomConfiguration.CreateRooms(section.transform, section.rooms, random);
                }

                sections.Add(section);
            }
        }

        public void PlaceRoomsRandomly()
        {
            foreach (DungeonSection section in sections)
            {
                section.transform.position = random.PointInsideUnitCircle();
            
                foreach (DungeonRoom room in section.rooms)
                {
                    room.transform.position = random.PointInsideUnitCircle();
                }
            }
        }

        public void DestroyDungeon()
        {
            sections = new List<DungeonSection>();

            foreach (Transform child in transform)
                GameObject.Destroy(child.gameObject);
        }
    }
}