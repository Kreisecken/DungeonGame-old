using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace DungeonGame.Utils.Graph
{
    // sources:
    // https://www.youtube.com/watch?v=4ySSsESzw2Y
    // https://www.wikiwand.com/en/Bowyer%E2%80%93Watson_algorithm
    // https://www.gorillasun.de/blog/bowyer-watson-algorithm-for-delaunay-triangulation/
    public static class Triangulation
    {
        // TODO: refactor and fix problems - put parts of algorithm in own methods for readability purposes
        public static void Triangulate<T>(Graph<T> graph)
        {
            // FIXME: this is only partially correct. E.g. this would break dungeonroom connections, because
            // this way, when we have e.g. two dungeonrooms in a section, none of them gets connected to eachother.
            // maybe resolve this edge case in dungeon generation
            if (graph.Count < 3) return;

            var supraTriangle = CreateSupraTriangle(graph.vertecies);

            var triangles = CreateTriangles(graph.vertecies, supraTriangle);

            triangles = GetUnconnectedTriangles(triangles, supraTriangle);

            foreach (var triangle in triangles)
            {
                graph.AddVertex(triangle.A);
                graph.AddVertex(triangle.B);
                graph.AddVertex(triangle.C);

                graph.AddEdge(triangle.AB);
                graph.AddEdge(triangle.BC);
                graph.AddEdge(triangle.AC);
            }
        }

        public static Triangle<T> CreateSupraTriangle<T>(IEnumerable<Vertex<T>> vertecies)
        {
            // calculate bounding box
            // TODO: separate this part into its own method (?)
            Vector2 min = Vector2.zero;
            Vector2 max = Vector2.zero;

            foreach (var vertex in vertecies)
            {
                if (vertex.position.x < min.x) min.x = vertex.position.x;
                if (vertex.position.y < min.y) min.y = vertex.position.y;
                if (vertex.position.x > max.x) max.x = vertex.position.x;
                if (vertex.position.y > max.y) max.y = vertex.position.y;
            }

            // TODO: this is a rough aproximation for a supra triangle, a lot of room for optimization

            var size = max - min;

            Vector2 tcenter = new(min.x + size.x / 2, max.y);
            Vector2 bcenter = tcenter - new Vector2(0, size.y);

            var diagonal = size.magnitude;

            Vector2 a = bcenter + new Vector2(-1f, 0.0f) * diagonal;
            Vector2 b = tcenter + new Vector2(0f, 0.5f) * diagonal;
            Vector2 c = bcenter + new Vector2(1f, 0.0f) * diagonal;

            // data attribute of vertecies is intentionally null (default). The Triangulation method utilizes this property.
            return new Triangle<T>
            (
                new Vertex<T>(default, a),
                new Vertex<T>(default, b),
                new Vertex<T>(default, c)
            );
        }

        public static List<Triangle<T>> CreateTriangles<T>(IEnumerable<Vertex<T>> vertecies, Triangle<T> supraTriangle)
        {
            List<Triangle<T>> triangles = new() { supraTriangle };
            
            foreach (var vertex in vertecies)
            {
                List<Edge<T>> polygon = new();

                var badTriangles = GetBadTriangles(triangles, vertex);

                AddNotSharedEdgesToPolygon(badTriangles, polygon);

                foreach (var badTriangle in badTriangles)
                    triangles.Remove(badTriangle);

                foreach (var edge in polygon)
                    triangles.Add(new(vertex, edge.a, edge.b));
            }

            return triangles;
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

        public static void AddNotSharedEdgesToPolygon<T>(List<Triangle<T>> badTriangles, List<Edge<T>> polygon)
        {
            foreach (var triangle in badTriangles)
            {
                foreach (var edge in triangle.edges)
                {
                    if (!DoesAnyTriangleShareEdge(badTriangles, triangle, edge))
                        polygon.Add(edge);
                }
            }
        }

        public static bool DoesAnyTriangleShareEdge<T>(List<Triangle<T>> badTriangles, Triangle<T> badTriangle, Edge<T> edge)
        {
            foreach (var triangleToCheck in badTriangles)
            {
                if (triangleToCheck == badTriangle) continue;

                if (triangleToCheck.HasEdge(edge))
                    return true;
            }

            return false;
        }

        public static List<Triangle<T>> GetUnconnectedTriangles<T>(List<Triangle<T>> triangles, Triangle<T> supraTriangle)
        {
            List<Triangle<T>> unconnectedTriangles = new();

            foreach (var triangle in triangles)
            {
                // TODO: optimizable by using the property of the supratriangle, that the vertecie data attribute is null
                //if (!triangle.HasVertex(null))
                if (!triangle.HasAnyVertexOfTriangle(supraTriangle))
                {
                    unconnectedTriangles.Add(triangle);
                }
            }

            return unconnectedTriangles;
        }
    }
}