using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGame.Utils
{
    public static class ListUtils
    {
        public static void Shuffle<T>(this IList<T> list, SeedableRandom random)
        {
            random.Shuffle(list);
        }

        public static T GetRandom<T>(this IEnumerable<T> enumerable, SeedableRandom random)
        {
            if (enumerable.Count() == 0) return default;

            return enumerable.ElementAt(random.Int32(0, enumerable.Count() - 1));
        }

        public static T[] GetRandom<T>(this IEnumerable<T> enumerable, SeedableRandom random, int count)
        {
            if (enumerable.Count() == 0) return default;

            T[] result = new T[count];

            for (int i = 0; i < count; i++)
                result[i] = enumerable.GetRandom(random);

            return result;
        }

        public static T[] GetRandom<T>(this IEnumerable<T> enumerable, SeedableRandom random, int min, int max)
        {
            return enumerable.GetRandom(random, random.Int32(min, max));
        }

        public static T GetAndRemoveRandom<T>(this IList<T> list, SeedableRandom random)
        {
            if (list.Count == 0) return default;

            int index = random.Int32(0, list.Count - 1);

            T value = list[index];
            list.RemoveAt(index);

            return value;
        }

        public static T[] GetAndRemoveRandom<T>(this IList<T> list, SeedableRandom random, int count)
        {
            if (list.Count == 0) return default;

            T[] result = new T[count];

            for (int i = 0; i < count; i++)
                result[i] = list.GetAndRemoveRandom(random);

            return result;
        }

        public static T[] GetAndRemoveRandom<T>(this IList<T> list, SeedableRandom random, int min, int max)
        {
            return list.GetAndRemoveRandom(random, random.Int32(min, max));
        }
    }
}