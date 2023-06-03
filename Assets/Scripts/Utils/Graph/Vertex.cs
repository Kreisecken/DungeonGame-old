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
        public readonly Vector2 position;

        public Vertex(T data, Vector2 position)
        {
            this.data = data;
            this.position = position;
        }

        public override string ToString()
        {
            return $"Vertex(data: ${data}, position: ${position})";
        }

        public bool EqualsVertex(Vertex<T> vertex) // TODO: does not feel right
        {
            return vertex != null && data != null && data.Equals(vertex.data) && position.Equals(vertex.position);
        }

        public override int GetHashCode() => base.GetHashCode();

        public static implicit operator T(Vertex<T> vertex) => vertex.data;
    }
}