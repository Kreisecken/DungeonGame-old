using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

namespace DungeonGame.Utils.Graph   
{
    [Serializable]
    public readonly struct Circle
    {
        public readonly Vector2 center;
        public readonly float radius;

        public float Circumference => 2 * Mathf.PI * radius;
        public float Area => Mathf.PI * radius * radius;

        public Circle(Vector2 center = default, float radius = default)
        {
            this.center = center;
            this.radius = radius;
        }

        public bool IsPointInsideCircle(Vector2 point)
        {
            return Vector2.Distance(point, center) < radius;
        }

        public Vector2 ClampPointToCircumference(Vector2 point)
        {
            if ((center - point).magnitude == radius) return point;

            float distance = (point - center).magnitude;

            Vector2 pointOnCircumference = center + radius * ((point - center) / distance);

            return pointOnCircumference;
        }

        public Vector2 ClampPoint(Vector2 point)
        {
            return (center - point).magnitude <= radius ? point : ClampPointToCircumference(point); 
        }

        public Vector2 GetRandomPointInsideCircle(SeedableRandom random)
        {
            return random.PointInsideCircle(this);
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

            return new (center, radius);
        }

        public static readonly Circle UNIT_CIRCLE = new(Vector2.zero, 1);
    }
}