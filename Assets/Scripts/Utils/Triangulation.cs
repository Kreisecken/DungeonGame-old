using DungeonGame.Dungeon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEditor.SearchableEditorWindow;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;

namespace DungeonGame.Utils
{
    public readonly struct Vertex<T>
    {
        public readonly T data;
        public readonly Vector2 position;

        public Vertex(T data = default, Vector2 position = default)
        {
            this.data = data;
            this.position = position;
        }


        public override string ToString()
        {
            return $"v({position})";
        }

        public override bool Equals(object obj)
        {
            if (obj is not Vertex<T> vertex) return false;

            return position.x == vertex.position.x && position.y == vertex.position.y; // only check positions
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Vertex<T> lh, Vertex<T> rh) =>  lh.Equals(rh);
        public static bool operator !=(Vertex<T> lh, Vertex<T> rh) => !lh.Equals(rh);
    }

    public readonly struct Edge<T>
    {
        public readonly Vertex<T> a;
        public readonly Vertex<T> b;

        public Edge(Vertex<T> a, Vertex<T> b)
        {
            this.a = a;
            this.b = b;
        }

        public override string ToString()
        {
            return $"e({a}, {b})";
        }

        public override bool Equals(object obj)
        {
            if (obj is not Edge<T> edge) return false;

            return a == edge.a && b == edge.b || a == edge.b && b == edge.a;
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Edge<T> lh, Edge<T> rh) =>  lh.Equals(rh);
        public static bool operator !=(Edge<T> lh, Edge<T> rh) => !lh.Equals(rh);
    }

    public readonly struct Triangle<T>
    {
        public readonly Vertex<T>[] vertecies;

        public readonly CircumCircle circumCircle;

        public readonly Edge<T>[] edges;

        public Triangle(Vertex<T> a, Vertex<T> b, Vertex<T> c)
        {
            vertecies = new Vertex<T>[] { a, b, c };

            circumCircle = Triangulation.CalculateCircumCircle(a.position, b.position, c.position);

            edges = new Edge<T>[] 
            { 
                new Edge<T>(a, b), 
                new Edge<T>(a, c), 
                new Edge<T>(b, c) 
            };
        }

        public bool IsPointInsideCircumCircle(Vector2 point)
        {
            return Vector2.Distance(point, circumCircle.center) <= circumCircle.radius;
        }

        public override string ToString()
        {
            return $"t({vertecies[0]}, {vertecies[0]}, {vertecies[0]})";
        }

        public override bool Equals(object obj)
        {
            if (obj is not Triangle<T> triangle) return false;

            return vertecies[0] == triangle.vertecies[0] && vertecies[1] == triangle.vertecies[1] && vertecies[2] == triangle.vertecies[2] ||
                   vertecies[0] == triangle.vertecies[0] && vertecies[1] == triangle.vertecies[2] && vertecies[2] == triangle.vertecies[1] ||
                   vertecies[0] == triangle.vertecies[1] && vertecies[1] == triangle.vertecies[0] && vertecies[2] == triangle.vertecies[2] ||
                   vertecies[0] == triangle.vertecies[1] && vertecies[1] == triangle.vertecies[2] && vertecies[2] == triangle.vertecies[0] ||
                   vertecies[0] == triangle.vertecies[2] && vertecies[1] == triangle.vertecies[0] && vertecies[2] == triangle.vertecies[1] ||
                   vertecies[0] == triangle.vertecies[2] && vertecies[1] == triangle.vertecies[1] && vertecies[2] == triangle.vertecies[0];

        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Triangle<T> lh, Triangle<T> rh) =>  lh.Equals(rh);
        public static bool operator !=(Triangle<T> lh, Triangle<T> rh) => !lh.Equals(rh);
    }

    public readonly struct CircumCircle
    {
        public readonly Vector2 center;
        public readonly float radius;

        public CircumCircle(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }
    }

    public static class Triangulation
    {
        public static CircumCircle CalculateCircumCircle(Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 sqrtA = a*a;
            Vector2 sqrtB = b*b;
            Vector2 sqrtC = c*c;

            float D = (a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) * 2f;
            float x = ((sqrtA.x + sqrtA.y) * (b.y - c.y) + (sqrtB.x + sqrtB.y) * (c.y - a.y) + (sqrtC.x + sqrtC.y) * (a.y - b.y)) / D;
            float y = ((sqrtA.x + sqrtA.y) * (c.x - b.x) + (sqrtB.x + sqrtB.y) * (a.x - c.x) + (sqrtC.x + sqrtC.y) * (b.x - a.x)) / D;
            
            Vector2 center = new Vector2(x, y);

            float radius = Vector2.Distance(center, a);

            return new CircumCircle(center, radius);
        }

        // TODO: refactor and fix problems - put parts of algorithm in own methods for readability purposes
        public static List<Triangle<T>> Triangulate<T>(List<Vertex<T>> vertecies)
        {
            List<Triangle<T>> triangles = new();

            Triangle<T> superTriangle = SuperTriangle(vertecies);

            triangles.Add(superTriangle);

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

                List<Edge<T>> polygon = new();

                foreach (Triangle<T> triangle in badTriangles)
                {
                    foreach (Edge<T> edge in triangle.edges)
                    {
                        bool exist = false;
                        
                        foreach (Triangle<T> triangleA in badTriangles)
                        {
                            if (triangle == triangleA) continue; 

                            foreach (Edge<T> edgeA in triangleA.edges)
                            {
                                if (edge == edgeA)
                                {
                                    exist = true;

                                    goto searchDone; // TODO: never use goto! But I just did...
                                }
                            }
                        }
                    searchDone:

                        if (!exist)
                            polygon.Add(edge);
                    }
                }


                foreach (var badTriangle in badTriangles)
                    triangles.Remove(badTriangle);

                foreach (var edge in polygon)
                {
                    triangles.Add(new Triangle<T>(vertex, edge.a, edge.b));
                }
            }

            List<Triangle<T>> result = new();

            foreach (var triangle in triangles)
            {
                if (!triangle.vertecies.Any((vertexA) => superTriangle.vertecies.Any((vertexB) => vertexA == vertexB)))
                {
                    result.Add(triangle);
                }
            }

            return result;
        }

        public static Triangle<T> SuperTriangle<T>(List<Vertex<T>> vertecies)
        {
            float minX = vertecies[0].position.x;
            float minY = vertecies[0].position.y;
            float maxX = minX;
            float maxY = minY;

            for (var i = 1; i < vertecies.Count; i++)
            {
                if (vertecies[i].position.x < minX) minX = vertecies[i].position.x;
                if (vertecies[i].position.y < minY) minY = vertecies[i].position.y;
                if (vertecies[i].position.x > maxX) maxX = vertecies[i].position.x;
                if (vertecies[i].position.y > maxY) maxY = vertecies[i].position.y;
            }

            var width  = maxX - minX;
            var height = maxY - minY;

            var tcenter = new Vector3(minX + width / 2, maxY);
            var bcenter = tcenter - new Vector3(0, height);

            var diagonal = Mathf.Sqrt(width * width + height * height);

            Vector3 a = bcenter + new Vector3(-1f, 0.0f) * diagonal;
            Vector3 b = tcenter + new Vector3( 0f, 0.5f) * diagonal;
            Vector3 c = bcenter + new Vector3( 1f, 0.0f) * diagonal;

            return new Triangle<T>
            (
                new Vertex<T>(position: a),
                new Vertex<T>(position: b), 
                new Vertex<T>(position: c)
            );
        }
    }
}