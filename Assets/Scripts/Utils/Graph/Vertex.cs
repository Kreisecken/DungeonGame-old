using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

namespace DungeonGame.Utils.Graph
{
    [Serializable]
    public struct Vertex<T>
    {
        public T data;
        public Vector2 position;

        public List<Edge<T>> edges; //TODO: restrict usage to sync edges and neighbours properly

        public List<Vertex<T>> neighbours;

        public Vertex(T data, Vector2 position = default)
        {
            this.data = data;
            this.position = position;

            edges = new();
            neighbours = new();
        }

        // this is only true when this vertex got created by the default keyword
        // best and only attribute that can be used to check if the vertex is initialized is the edges list
        // this may or may not be the best solution
        public bool IsUninitialized() => edges == null;

        public override bool Equals(object obj) // TODO: does not feel right
            => obj is Vertex<T> vertex && data != null && data.Equals(vertex.data) && position.Equals(vertex.position);

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Vertex<T> lh, Vertex<T> rh) =>  lh.Equals(rh);
        public static bool operator !=(Vertex<T> lh, Vertex<T> rh) => !lh.Equals(rh);

        public void AddEdge(Edge<T> edge)
        {
            if (!edges.Contains(edge))
                edges.Add(edge);

            neighbours.Add(edge.a == this ? edge.b : edge.a);
        }

        public void RemoveEdge(Edge<T> edge)
        {
            edges.Remove(edge);

            neighbours.Remove(edge.a == this ? edge.b : edge.a);
        }

        public Edge<T> this[int index]
        {
            get => edges[index];
            set => edges[index] = value;
        }

        public override string ToString()
        {
            return $"v({position.x}, {position.y})";
        }

        public static implicit operator T(Vertex<T> vertex) => vertex.data;
        public static implicit operator Vector2(Vertex<T> vertex) => vertex.position;
    }
}