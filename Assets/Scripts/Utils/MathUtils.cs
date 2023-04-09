using System.Collections;
using UnityEngine;

namespace DungeonGame.Utils
{
    public static class MathUtils 
    {
        public static (Vector2 center, float radius) CircumCircle(Vector2 a, Vector2 b, Vector2 c)
        {
            float d = a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y) * 2;

            Vector2 center;

            center.x = (a.x*a.x + a.y*a.y) * (b.y - c.y) +
                       (b.x*b.x + b.y*b.y) * (c.y - a.y) + 
                       (c.x*c.x + c.y*c.y) * (a.y - b.y);

            center.y = (a.x * a.x + a.y * a.y) * (c.x - b.x) +
                       (b.x * b.x + b.y * b.y) * (a.x - c.x) +
                       (c.x * c.x + c.y * c.y) * (b.x - a.x);

            center /= d;

            float radius = (center - a).magnitude;

            return (center, radius);
        }
    }
}