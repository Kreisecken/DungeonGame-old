using DungeonGame.Dungeon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungeonGame.Utils
{
    public struct Vertex<T>
    {
        public T data;
        public Vector2 position;

        public Vertex(T data, Vector2 position = default)
        {
            this.data = data;
            this.position = position;
        }
    }

    public readonly struct Triangle<T>
    {
        public readonly Vertex<T> a, b, c;

        public readonly (Vector2 center, float radius) circumCircle;

        public readonly (Vertex<T> a, Vertex<T> b)[] edges;

        public Triangle(Vertex<T> a, Vertex<T> b, Vertex<T> c)
        {
            this.a = a;
            this.b = b;
            this.c = c;

            circumCircle = Triangulation.CircumCircle(a.position, b.position, c.position);

            edges = new[] { (a, b), (a, c), (b, c) };
        }

        public bool IsPointInsideCircumCircle(Vector2 point)
        {
            return Vector2.Distance(point, circumCircle.center) < circumCircle.radius;
        }
    }

    public static class Triangulation
    {
        public static (Vector2 center, float radius) CircumCircle(Vector2 a, Vector2 b, Vector2 c)
        {
            float d = a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y) * 2;

            Vector2 center;

            center.x = (a.x * a.x + a.y * a.y) * (b.y - c.y) +
                       (b.x * b.x + b.y * b.y) * (c.y - a.y) +
                       (c.x * c.x + c.y * c.y) * (a.y - b.y);

            center.y = (a.x * a.x + a.y * a.y) * (c.x - b.x) +
                       (b.x * b.x + b.y * b.y) * (a.x - c.x) +
                       (c.x * c.x + c.y * c.y) * (b.x - a.x);

            center /= d;

            float radius = (center - a).magnitude;

            return (center, radius);
        }

        public static List<Triangle<T>> Triangulate<T>(List<Vertex<T>> vertecies)
        {
            List<Triangle<T>> triangles = new();

            foreach (var vertex in vertecies)
            {
                List<Triangle<T>> badTriangles = new();

                foreach (var triangle in triangles)
                {
                    if (triangle.IsPointInsideCircumCircle(vertex.position))
                    {
                        badTriangles.Add(triangle);
                    }
                }

                List<Triangle<T>> polygon = new();

                foreach (var triangle in badTriangles)
                {
                    foreach (var edge in triangle.edges)
                    {

                    }
                }
            }

            return triangles;
        }
    }
}