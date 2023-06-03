using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace DungeonGame.Utils.Graph
{
    [Serializable]
    public class Graph<T>
    {
        public HashSet<Vertex<T>> vertecies;

        public int Count => vertecies.Count;

        public Dictionary<Vertex<T>, HashSet<Edge<T>>> edgesMap;
        public HashSet<Edge<T>> edges;

        #region DEBUGGING
        public List<Vertex<T>> list;

        public void Refresh()
        {
            list = new();

            foreach (var vertex in vertecies)
            {
                list.Add(vertex);
            }
        }

        #endregion DEBUGGING

        public Graph(HashSet<Vertex<T>> vertecies = null)
        {
            this.vertecies = vertecies ?? new();

            edges = new();
            edgesMap = new();
        }

        public void Triangulate()
        {
            Triangulation.Triangulate(this);
        }

        // https://www.wikiwand.com/de/Algorithmus_von_Kruskal
        // without any thinking implemented. Works but needs to be looked at again.
        // mhm, reafactor it lalter, it realy is ugly
        public HashSet<Edge<T>> MinimumSpanningTree()
        {
            // Create a list to store the minimum spanning tree edges
            List<Edge<T>> minimumSpanningTree = new();

            // Create a dictionary to track the parent of each vertex in the tree
            Dictionary<Vertex<T>, Vertex<T>> parent = new();

            // Create a dictionary to track the rank of each vertex in the tree
            Dictionary<Vertex<T>, int> rank = new();

            // Initialize the parent and rank dictionaries
            foreach (var vertex in vertecies)
            {
                parent[vertex] = vertex;
                rank[vertex] = 0;
            }

            // Sort all edges in ascending order by weight
            List<Edge<T>> sortedEdges = edges.ToList();
            sortedEdges.Sort((a, b) => a.weight.CompareTo(b.weight));

            // Process each edge in the sorted order
            foreach (var edge in sortedEdges)
            {
                var rootA = FindRoot(edge.a, parent);
                var rootB = FindRoot(edge.b, parent);

                // Check if adding the edge will form a cycle
                if (rootA != rootB)
                {
                    // Add the edge to the minimum spanning tree
                    minimumSpanningTree.Add(edge);

                    // Merge the two sets by rank
                    if (rank[rootA] < rank[rootB])
                    {
                        parent[rootA] = rootB;
                    }
                    else if (rank[rootA] > rank[rootB])
                    {
                        parent[rootB] = rootA;
                    }
                    else
                    {
                        parent[rootB] = rootA;
                        rank[rootA]++;
                    }
                }
            }

            HashSet<Edge<T>> removedEdges = edges.Where((edge) => !minimumSpanningTree.Contains(edge)).ToHashSet();

            // Update the edges of the graph to contain only the minimum spanning tree edges
            edges = new HashSet<Edge<T>>(minimumSpanningTree);

            // Update the edges map
            edgesMap.Clear();

            foreach (var vertex in vertecies)
            {
                edgesMap[vertex] = new HashSet<Edge<T>>();
            }

            foreach (var edge in edges)
            {
                edgesMap[edge.a].Add(edge);
                edgesMap[edge.b].Add(edge);
            }

            return removedEdges;
        }

        private Vertex<T> FindRoot(Vertex<T> vertex, Dictionary<Vertex<T>, Vertex<T>> parent)
        {
            Stack<Vertex<T>> stack = new();

            while (parent[vertex] != vertex)
            {
                stack.Push(vertex);
                vertex = parent[vertex];
            }

            while (stack.Count > 0)
                parent[stack.Pop()] = vertex;

            return vertex;
        }

        public void AddVertex(Vertex<T> vertex)
        {
            if (!vertecies.Add(vertex)) return;

            edgesMap.Add(vertex, new());
        }

        public void RemoveVertex(Vertex<T> vertex)
        {
            if (!vertecies.Remove(vertex)) return;

            edges.RemoveWhere((edge) => edge.HasVertex(vertex));

            edgesMap.Remove(vertex);
            
            foreach (var (_, edges) in edgesMap)
            {
                edges.RemoveWhere((edge) => edge.HasVertex(vertex));
            }
        }

        public void AddEdge(Edge<T> edge)
        {
            if (!vertecies.Contains(edge.a)) AddVertex(edge.a);
            if (!vertecies.Contains(edge.b)) AddVertex(edge.b);

            if (!edges.Add(edge)) return;

            edgesMap.TryGetValue(edge.a, out var a); // should not be null by design
            edgesMap.TryGetValue(edge.b, out var b);

            a.Add(edge);
            b.Add(edge);
        }

        public void RemoveEdge(Edge<T> edge)
        {
            if (!vertecies.Contains(edge.a) || !vertecies.Contains(edge.b)) return;

            if (!edges.Remove(edge)) return;

            edgesMap.TryGetValue(edge.a, out var a); // should not be null by design
            edgesMap.TryGetValue(edge.b, out var b);

            a.Add(edge);
            b.Add(edge);
        }
    }
} 