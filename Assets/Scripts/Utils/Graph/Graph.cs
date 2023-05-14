using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DungeonGame.Utils.Graph
{
    [Serializable]
    public class Graph<T>
    {
        public List<Vertex<T>> vertecies;

        public int Count => vertecies.Count;

        public Graph(List<Vertex<T>> vertices = null)
        {
            this.vertecies = vertecies ?? new();
        }

        public void Triangulate()
        {
            Triangulation.Triangulate(this);
        }

        public void MinimumSpanningTree()
        {

        }

        public void AddVertex(Vertex<T> vertex)
        {
            if (!vertecies.Contains(vertex))
                vertecies.Add(vertex);
        }

        public void RemoveVertex(Vertex<T> vertex)
        {
            vertecies.Remove(vertex);
        }

        public void SetPositions(Func<T, Vector2> positionProvider)
        {
            vertecies.ForEach(vertex => vertex.position = positionProvider(vertex));
        }
    }
} 