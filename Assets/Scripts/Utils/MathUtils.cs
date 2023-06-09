using DungeonGame.Utils.Graph;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DungeonGame.Utils
{
    public static class Vector3Utils
    {
        public static Vector3 Floored(this Vector3 vector)
        {
            return new Vector3((int) vector.x, (int) vector.y, (int) vector.z);
        }
    }
}