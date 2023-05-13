using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Utils.Graph
{
    public static class Triangulation
    {
        // TODO: refactor and fix problems - put parts of algorithm in own methods for readability purposes
        public static void Triangulate<T>(Graph<T> graph)
        {
            // FIXME: this is only partially correct. E.g. this would break dungeonroom connections, because
            // this way, when we have e.g. two dungeonrooms in a section, none of them gets connected to eachother.
            // maybe resolve this edge case in dungeon generation
            if (graph.Count < 3) return;

            var vertecies = graph.vertecies;

            var supraTriangle = CreateSupraTriangle(vertecies);

            var triangles = CreateTriangles(vertecies, supraTriangle);

            triangles = GetUnconnectedTriangles(triangles, supraTriangle);

            foreach (var triangle in triangles)
            {
                triangle.A.AddEdge(triangle.AB);
                triangle.A.AddEdge(triangle.AC);

                triangle.B.AddEdge(triangle.AB);
                triangle.B.AddEdge(triangle.BC);

                triangle.C.AddEdge(triangle.BC);
                triangle.C.AddEdge(triangle.AC);
            }
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

            // data attribute of vertecies is intentionally null (default). The Triangulation method utilizes this property.
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