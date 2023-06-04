using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Utils
{
    public static class ListUtils
    {
        public static void Shuffle<T>(this IList<T> list, SeedableRandom random)
        {
            random.Shuffle(list);
        }

        public static T GetRandom<T>(this IList<T> list, SeedableRandom random)
        {
            if (list.Count == 0) return default;

            return list[random.Int32(0, list.Count - 1)];
        }

        public static T GetAndRemoveRandom<T>(this IList<T> list, SeedableRandom random)
        {
            if (list.Count == 0) return default;

            int index = random.Int32(0, list.Count - 1);

            T value = list[index];
            list.RemoveAt(index);

            return value;
        }
    }
}