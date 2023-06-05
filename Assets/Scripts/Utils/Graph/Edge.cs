using DungeonGame.Utils.Graph;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using System;
using Unity.VisualScripting;

namespace DungeonGame.Utils.Graph
{
    public class BaseEdge<TVertex, TData> where TVertex : Vertex<TData>
    {
        public TVertex a;
        public TVertex b;

        public float weight;

        public BaseEdge(TVertex a, TVertex b, float weight)
        {
            this.a = a;
            this.b = b;

            if (a == b) throw new Exception("Vertecies are equal!");

            this.weight = weight;
        }

        public bool HasVertex(TVertex vertex)
        {
            return a == vertex || b == vertex;
        }

        public override string ToString()
        {
            return $"Edge({a}, {b})";
        }

        public override bool Equals(object obj)
        {
            if (obj is not BaseEdge<TVertex, TData> edge) return false;

            return a == edge.a && b == edge.b || a == edge.b && b == edge.a;
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(BaseEdge<TVertex, TData> lh, BaseEdge<TVertex, TData> rh) => lh.Equals(rh);
        public static bool operator !=(BaseEdge<TVertex, TData> lh, BaseEdge<TVertex, TData> rh) => !lh.Equals(rh);

        public static implicit operator float(BaseEdge<TVertex, TData> vertex) => vertex.weight;
    }
  
    [Serializable]
    public class Edge<TData> : BaseEdge<Vertex<TData>, TData>
    {
        public Edge(Vertex<TData> a, Vertex<TData> b, float weight) : base(a, b, weight) { }
    }

    [Serializable]
    public class Edge3<TData> : BaseEdge<Vertex3<TData>, TData>
    {
        public Edge3(Vertex3<TData> a, Vertex3<TData> b, float weight) : base(a, b, weight) { }
    }
}