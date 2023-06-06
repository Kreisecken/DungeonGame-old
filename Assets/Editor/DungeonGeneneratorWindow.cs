using UnityEngine;
using UnityEditor;
using DungeonGame.Dungeons;
using DungeonGame.Utils;
using System.Linq;

namespace DungeonGame.Editor
{
    public class DungeonGeneratorWindow : EditorWindow
    {
        private Transform dungeonParent;

        private DungeonConfiguration dungeonConfiguration;
        private SeedableRandom random;

        private SeedType seedType;

        private long seed_long;
        private string seed_string;

        private Dungeon generatedDungeon;

        [MenuItem("Dungeons/Dungeon Generator")]
        public static void ShowWindow()
        {
            GetWindow<DungeonGeneratorWindow>("Dungeon Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Dungeon Generator", EditorStyles.boldLabel);

            GUILayoutUtils.GenericField("Parent", ref dungeonParent, true);

            GUILayoutUtils.GenericField("Configuration", ref dungeonConfiguration);

            GUILayout.Space(10);

            GUILayout.Label("Random", EditorStyles.boldLabel);

            GUILayoutUtils.EnumField("", ref seedType);


            if (seedType != SeedType.Random)
            {
                GUILayout.BeginHorizontal();
                
                if (seedType == SeedType.Long)
                {
                    seed_long = EditorGUILayout.LongField("", seed_long);
                    seed_string = seed_long + "";
                }
                else if (seedType == SeedType.String)
                {
                    seed_string = EditorGUILayout.TextField("", seed_string);
                    seed_long = seed_string.All(char.IsDigit) ? long.Parse(seed_string) : 0;
                }

                if (GUILayout.Button("Generate Seed"))
                {
                    seed_long = SeedableRandom.Instance.Next();
                    seed_string = SeedableRandom.Instance.String(20, StringUtils.AlphaNumeric);
                }
                
                GUILayout.EndHorizontal();
            }


            random = seedType switch
            {
                SeedType.Random => Seed.TimedSeed,
                SeedType.Long   => seed_long,
                SeedType.String => seed_string,
                _ => random // kinda redundant, but it's here to make the compiler happy
            };

            GUILayout.Space(30);

            if (GUILayout.Button("Generate Dungeon"))
            {
                foreach (Transform transform in dungeonParent)
                    if (transform.TryGetComponent<Dungeon>(out var dungeon))
                        DungeonGenerator.DestroyDungeon(dungeon);

                generatedDungeon = DungeonGenerator.GenerateDungeon(dungeonParent, dungeonConfiguration, random);
            }
        }
    }

    public enum SeedType
    {
        Random,
        Long,
        String
    }
}