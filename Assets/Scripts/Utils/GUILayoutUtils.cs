using UnityEngine;
using UnityEditor;
using DungeonGame.Dungeons;
using DungeonGame.Utils;

namespace DungeonGame.Editor
{
    public static class GUILayoutUtils 
    {
        public static void GenericField<T>(string label, ref T value, bool allowSceneObjects = false) where T : Object
        {
            value = (T) EditorGUILayout.ObjectField(label, value, typeof(T), allowSceneObjects);
        }

        public static void EnumField<T>(string label, ref T value) where T : System.Enum
        {
            value = (T) EditorGUILayout.EnumPopup(label, value);
        }
    }
}