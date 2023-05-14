using System.Collections;
using System;
using UnityEngine;

namespace DungeonGame.Utils.Graph   
{
    [Serializable]
    public struct Circle
    {
        public Vector2 center;
        public float radius;

        public Circle(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public bool IsPointInsideCircle(Vector2 point)
        {
            return Vector2.Distance(point, center) < radius;
        }

        public static Circle CalculateCircumCircle(Vector2 a, Vector2 b, Vector2 c)
        {
            var sqrtA = a * a;
            var sqrtB = b * b;
            var sqrtC = c * c;

            var d = (a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) * 2f;
            var x = ((sqrtA.x + sqrtA.y) * (b.y - c.y) + (sqrtB.x + sqrtB.y) * (c.y - a.y) + (sqrtC.x + sqrtC.y) * (a.y - b.y)) / d;
            var y = ((sqrtA.x + sqrtA.y) * (c.x - b.x) + (sqrtB.x + sqrtB.y) * (a.x - c.x) + (sqrtC.x + sqrtC.y) * (b.x - a.x)) / d;

            Vector2 center = new(x, y);

            var radius = Vector2.Distance(center, a);

            return new(center, radius);
        }
    }
}