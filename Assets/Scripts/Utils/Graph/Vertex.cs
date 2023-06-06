using DungeonGame.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

namespace DungeonGame.Utils.Graph
{
    [Serializable]
    public class Vertex<T>
    {
        public readonly T data;

        public Vertex(T data)
        {
            this.data = data;
        }

        public override string ToString()
        {
            return $"Vertex(data: ${data})";
        }

        public virtual bool EqualsVertex(Vertex<T> vertex)
        {
            return vertex is Vertex<T> vertex1 && data.Equals(vertex1.data);
        }

        public override int GetHashCode() => base.GetHashCode();

        public static implicit operator T(Vertex<T> vertex) => vertex.data;
    }

    public class Vertex3<T> : Vertex<T>
    {
        public readonly Vector3 position;

        public Vertex3(T data, Vector3 position) : base(data)
        {
            this.position = position;
        }

        public override bool EqualsVertex(Vertex<T> vertex)
        {
            return base.EqualsVertex(vertex) && (vertex is not Vertex3<T> vertex3 || position.Equals(vertex3.position));
        }
    }
}