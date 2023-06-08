using UnityEngine;
using UnityEditor;
using DungeonGame.Dungeons;
using DungeonGame.Utils;
using System.Linq;
using System.Collections.Generic;

namespace DungeonGame.Utils
{
    public class BetterDictionary<TKey, TValue> : Dictionary<TKey, TValue> 
    {
        /*
            Dictionary throws an exception when the key is not found.
            This is kinda annoying, so I made this class to return the default value instead.

            This way, lines like this:

                dictionary[key]?.DoSomething();                 or
                dictionary[key] ??= new Value();                can be used

            instead of this:
            
                if (dictionary.ContainsKey(key))
                    dictionary[key].DoSomething();              or
                if (dictionary.TryGetValue(key, out var value))
                    value.DoSomething();
        */
        public new TValue this[TKey key]
        {
            get => TryGetValue(key, out var value) ? value : default;
            set => base[key] = value;
        }
    }
}