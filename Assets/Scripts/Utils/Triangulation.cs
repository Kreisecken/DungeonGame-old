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
    [System.Serializable]
    public struct Vertex<T>
    {
        public T data;
        public Vector2 position;

        public Vertex(T data, Vector2 position)
        {
            this.data = data;
            this.position = position;
        }

        public override string ToString()
        {
            return $"v({position.x}, {position.y})";
        }

        public override bool Equals(object obj)
        {
            if (obj is not Vertex<T> vertex) return false;

            return position.x == vertex.position.x && position.y == vertex.position.y;
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Vertex<T> lh, Vertex<T> rh) =>  lh.Equals(rh);
        public static bool operator !=(Vertex<T> lh, Vertex<T> rh) => !lh.Equals(rh);
    }

    [System.Serializable]
    public struct Edge<T>
    {
        public Vertex<T> a;
        public Vertex<T> b;

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

    [System.Serializable]
    public struct Triangle<T>
    {
        public Vertex<T>[] vertecies;
        public Edge<T>[] edges;

        public readonly Vertex<T> A { get => vertecies[0]; }
        public readonly Vertex<T> B { get => vertecies[1]; }
        public readonly Vertex<T> C { get => vertecies[2]; }

        public readonly Edge<T> AB { get => edges[0]; }
        public readonly Edge<T> AC { get => edges[1]; }
        public readonly Edge<T> BC { get => edges[2]; }

        public Circle circumCircle;

        public Triangle(Vertex<T> a, Vertex<T> b, Vertex<T> c)
        {
            vertecies = new Vertex<T>[] { a, b, c };

            edges = new Edge<T>[] 
            { 
                new(a, b), 
                new(a, c), 
                new(b, c) 
            };

            circumCircle = Circle.CalculateCircumCircle(a.position, b.position, c.position);
        }

        public bool HasVertex(Vertex<T> vertex)
        {
            return A == vertex || B == vertex || C == vertex;
        }

        public bool HasAnyVertexOfTriangle(Triangle<T> triangle)
        {
            return HasVertex(triangle.A) || HasVertex(triangle.B) || HasVertex(triangle.C);
        }

        public bool HasEdge(Edge<T> edge)
        {
            return AB == edge || AC == edge || BC == edge;
        }

        public bool HasAnyEdgeOfTriangle(Triangle<T> triangle)
        {
            return HasEdge(triangle.AB) || HasEdge(triangle.AC) || HasEdge(triangle.BC);
        }

        public override string ToString()
        {
            return $"t({vertecies[0]}, {vertecies[0]}, {vertecies[0]})";
        }

        public override bool Equals(object obj)
        {
            if (obj is not Triangle<T> triangle) return false;

            // TODO: this could be optimized by defining vertecies of triangle counter clockwise

            return A == triangle.A && B == triangle.B && C == triangle.C ||
                   A == triangle.A && B == triangle.C && C == triangle.B ||
                   A == triangle.B && B == triangle.A && C == triangle.C ||
                   A == triangle.B && B == triangle.C && C == triangle.A ||
                   A == triangle.C && B == triangle.A && C == triangle.B ||
                   A == triangle.C && B == triangle.B && C == triangle.A;

        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Triangle<T> lh, Triangle<T> rh) =>  lh.Equals(rh);
        public static bool operator !=(Triangle<T> lh, Triangle<T> rh) => !lh.Equals(rh);
    }

    [System.Serializable]
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

    public static class Triangulation
    {
        public static List<Triangle<T>> Triangulate<T>(List<T> vertecies, Func<T, Vector2> positionProvider)
        {
            List<Vertex<T>> mappedVertecies = new();

            foreach (var data in vertecies)
                mappedVertecies.Add(new(data, positionProvider(data)));

            return Triangulate(mappedVertecies);
        }

        // TODO: refactor and fix problems - put parts of algorithm in own methods for readability purposes
        public static List<Triangle<T>> Triangulate<T>(List<Vertex<T>> vertecies)
        {
            // FIXME: this is only partially correct. E.g. this would break dungeonroom connections, because
            // this way, when we have e.g. two dungeonrooms in a section, none of them gets connected to eachother.
            // maybe resolve this edge case in dungeon generation
            if (vertecies.Count < 3) return new();

            var supraTriangle = CreateSupraTriangle(vertecies);

            var triangles = CreateTriangles(vertecies, supraTriangle);

            triangles = GetUnconnectedTriangles(triangles, supraTriangle);

            return triangles;
        }

        public static Triangle<T> CreateSupraTriangle<T>(List<Vertex<T>> vertecies)
        {
            // calculate bounding box
            // TODO: separate this part into its own method (?)
            Vector2 min = new(vertecies[0].position.x, vertecies[0].position.y);
            Vector2 max = new(vertecies[0].position.x, vertecies[0].position.y);

            for (var i = 1; i < vertecies.Count; i++)
            {
                if (vertecies[i].position.x < min.x) min.x = vertecies[i].position.x;
                if (vertecies[i].position.y < min.y) min.y = vertecies[i].position.y;
                if (vertecies[i].position.x > max.x) max.x = vertecies[i].position.x;
                if (vertecies[i].position.y > max.y) max.y = vertecies[i].position.y;
            }

            // TODO: this is a rough aproximation for a supra triangle, a lot of room for optimization

            var size = max - min;

            Vector2 tcenter = new(min.x + size.x / 2, max.y);
            Vector2 bcenter = tcenter - new Vector2(0, size.y);

            var diagonal = size.magnitude;

            Vector2 a = bcenter + new Vector2(-1f, 0.0f) * diagonal;
            Vector2 b = tcenter + new Vector2(0f, 0.5f) * diagonal;
            Vector2 c = bcenter + new Vector2(1f, 0.0f) * diagonal;

            // data attribute of vertecies is intentionally null. The Triangulation method utilizes this property.
            return new Triangle<T>
            (
                new Vertex<T>(default, a),
                new Vertex<T>(default, b),
                new Vertex<T>(default, c)
            );
        }

        public static List<Triangle<T>> CreateTriangles<T>(List<Vertex<T>> vertecies, Triangle<T> supraTriangle)
        {
            List<Triangle<T>> triangles = new() { supraTriangle };

            foreach (var vertex in vertecies)
            {
                var badTriangles = GetBadTriangles(triangles, vertex);

                List<Edge<T>> polygon = new();

                AddNotSharedEdgesToPolygon(badTriangles, polygon);

                foreach (var badTriangle in badTriangles)
                    triangles.Remove(badTriangle);

                foreach (var edge in polygon)
                {
                    triangles.Add(new(vertex, edge.a, edge.b));
                }
            }

            return triangles;
        }


        public static bool DoesAnyTriangleShareEdge<T>(List<Triangle<T>> triangles, Triangle<T> triangle, Edge<T> edge)
        {
            foreach (var triangleToCheck in triangles)
            {
                if (triangle == triangleToCheck) continue;

                if (triangleToCheck.HasEdge(edge))
                    return true;
            }

            return false;
        }

        public static void AddNotSharedEdgesToPolygon<T>(List<Triangle<T>> triangles, List<Edge<T>> polygon)
        {
            foreach (var triangle in triangles)
            {
                foreach (var edge in triangle.edges)
                {
                    if (!DoesAnyTriangleShareEdge(triangles, triangle, edge))
                        polygon.Add(edge);
                }
            }
        }

        public static List<Triangle<T>> GetUnconnectedTriangles<T>(List<Triangle<T>> triangles, Triangle<T> supraTriangle)
        {
            List<Triangle<T>> trianglesWithoutByProducts = new();

            foreach (var triangle in triangles)
            {
                // TODO: optimizable by using the property of the supratriangle, that the vertecie data attribute is null
                if (!triangle.HasAnyVertexOfTriangle(supraTriangle))
                {
                    trianglesWithoutByProducts.Add(triangle);
                }
            }

            return trianglesWithoutByProducts;
        }

        public static List<Triangle<T>> GetBadTriangles<T>(List<Triangle<T>> triangles, Vertex<T> vertex)
        {
            List<Triangle<T>> badTriangles = new();

            foreach (var triangle in triangles)
            {
                if (triangle.circumCircle.IsPointInsideCircle(vertex.position))
                {
                    badTriangles.Add(triangle);
                }
            }

            return badTriangles;
        }
    }
}