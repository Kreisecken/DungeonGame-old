using System.Collections;
using UnityEngine;
using System;

namespace DungeonGame.Utils.Graph
{
    [Serializable]
    public struct Triangle<T>
    {
        public Vertex<T>[] vertecies;
        public Edge<T>[] edges;

        public readonly Vertex<T> A => vertecies[0];
        public readonly Vertex<T> B => vertecies[1];
        public readonly Vertex<T> C => vertecies[2];

        public readonly Edge<T> AB => edges[0];
        public readonly Edge<T> AC => edges[1];
        public readonly Edge<T> BC => edges[2];

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

            circumCircle = Circle.CalculateCircumCircle(a, b, c);
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
}