using System.Collections;
using UnityEngine;
using System;

namespace DungeonGame.Utils.Graph
{
    [Serializable]
    public class Triangle<T>
    {
        public Vertex3<T>[] vertecies;
        public Edge3<T>[] edges;

        public Vertex3<T> A => vertecies[0];
        public Vertex3<T> B => vertecies[1];
        public Vertex3<T> C => vertecies[2];

        public Edge3<T> AB => edges[0];
        public Edge3<T> AC => edges[1];
        public Edge3<T> BC => edges[2];

        public Circle circumCircle;

        public Triangle(Vertex3<T> a, Vertex3<T> b, Vertex3<T> c)
        {
            (a, b, c) = SortVerteciesClockwise(a, b, c);

            vertecies = new Vertex3<T>[] { a, b, c };

            edges = new Edge3<T>[3]
            {
                new(a, b, 0),
                new(a, c, 0),
                new(b, c, 0)
            };

            circumCircle = Circle.CalculateCircumCircle(a.position, b.position, c.position);
        }

        public bool HasVertex(Vertex3<T> vertex)
        {
            return A == vertex || B == vertex || C == vertex;
        }

        public bool HasAnyVertexOfTriangle(Triangle<T> triangle)
        {
            return HasVertex(triangle.A) || HasVertex(triangle.B) || HasVertex(triangle.C);
        }

        public bool HasEdge(Edge3<T> edge)
        {
            return AB == edge || AC == edge || BC == edge;
        }

        public bool HasAnyEdgeOfTriangle(Triangle<T> triangle)
        {
            return HasEdge(triangle.AB) || HasEdge(triangle.AC) || HasEdge(triangle.BC);
        }

        public override string ToString()
        {
            return $"t({vertecies[0]}, {vertecies[1]}, {vertecies[2]})";
        }

        public override bool Equals(object obj)
        {
            if (obj is not Triangle<T> triangle) return false;

            // TODO: this could be optimized by defining vertecies of triangle (counter) clockwise

            return A == triangle.A && B == triangle.B && C == triangle.C;
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Triangle<T> lh, Triangle<T> rh) =>  lh.Equals(rh);
        public static bool operator !=(Triangle<T> lh, Triangle<T> rh) => !lh.Equals(rh);
    
        public static (Vertex3<T> a, Vertex3<T> b, Vertex3<T> c) SortVerteciesClockwise(Vertex3<T> a, Vertex3<T> b, Vertex3<T> c)
        {
            // not the best implementation, but should work for now

            float centerX = (a.position.x + b.position.x + c.position.x) / 3f;
            float centerY = (a.position.y + b.position.y + c.position.y) / 3f;
            
            float angleA = Mathf.Atan2(a.position.y - centerY, a.position.x - centerX);
            float angleB = Mathf.Atan2(b.position.y - centerY, b.position.x - centerX);
            float angleC = Mathf.Atan2(c.position.y - centerY, c.position.x - centerX);

            if (angleA < angleB && angleA < angleC)
                return (a, b, c);
            else if (angleB < angleA && angleB < angleC)
                return (b, c, a);
            else if (angleC < angleA && angleC < angleB)
                return (c, a, b);
            else
                return (a, b, c);
        }
    }
}