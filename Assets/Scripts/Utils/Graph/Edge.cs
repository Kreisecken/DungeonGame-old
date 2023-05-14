using DungeonGame.Utils.Graph;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using System;

namespace DungeonGame.Utils.Graph
{
    [Serializable]
    public struct Edge<T>
    {
        public Vertex<T> a;
        public Vertex<T> b;

        public float weight;

        public Edge(Vertex<T> a, Vertex<T> b, float weight = 0.0f)
        {
            this.a = a;
            this.b = b;

            this.weight = weight;
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

        public static bool operator ==(Edge<T> lh, Edge<T> rh) => lh.Equals(rh);
        public static bool operator !=(Edge<T> lh, Edge<T> rh) => !lh.Equals(rh);

        public static implicit operator float(Edge<T> vertex) => vertex.weight;
    }
}