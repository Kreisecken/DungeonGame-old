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
    }
}