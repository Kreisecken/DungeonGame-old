using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace DungeonGame.Utils.Graph
{
    [Serializable]
    public class BaseGraph<TVertex, TEdge, TData> where TVertex : Vertex<TData> where TEdge : BaseEdge<TVertex, TData>
    {
        public HashSet<TVertex> vertecies;

        public int Count => vertecies.Count;

        public Dictionary<TVertex, HashSet<TEdge>> edgesMap;
        public HashSet<TEdge> edges;

        #region DEBUGGING

        public List<TVertex> list;

        public void Refresh()
        {
            list = new();

            foreach (var vertex in vertecies)
            {
                list.Add(vertex);
            }
        }

        #endregion DEBUGGING

        public BaseGraph(HashSet<TVertex> vertecies = null)
        {
            this.vertecies = vertecies ?? new();

            edges = new();
            edgesMap = new();
        }

        // https://www.wikiwand.com/de/Algorithmus_von_Kruskal
        // without any thinking implemented. Works but needs to be looked at again.
        // mhm, reafactor it lalter, it realy is ugly
        public HashSet<TEdge> MinimumSpanningTree()
        {
            // Create a list to store the minimum spanning tree edges
            List<TEdge> minimumSpanningTree = new();

            // Create a dictionary to track the parent of each vertex in the tree
            Dictionary<TVertex, TVertex> parent = new();

            // Create a dictionary to track the rank of each vertex in the tree
            Dictionary<TVertex, int> rank = new();

            // Initialize the parent and rank dictionaries
            foreach (var vertex in vertecies)
            {
                parent[vertex] = vertex;
                rank[vertex] = 0;
            }

            // Sort all edges in ascending order by weight
            List<TEdge> sortedEdges = edges.ToList();
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

            HashSet<TEdge> removedEdges = edges.Where((edge) => !minimumSpanningTree.Contains(edge)).ToHashSet();

            // Update the edges of the graph to contain only the minimum spanning tree edges
            edges = new HashSet<TEdge>(minimumSpanningTree);

            // Update the edges map
            edgesMap.Clear();

            foreach (var vertex in vertecies)
            {
                edgesMap[vertex] = new HashSet<TEdge>();
            }

            foreach (var edge in edges)
            {
                edgesMap[edge.a].Add(edge);
                edgesMap[edge.b].Add(edge);
            }

            return removedEdges;
        }

        private TVertex FindRoot(TVertex vertex, Dictionary<TVertex, TVertex> parent)
        {
            Stack<TVertex> stack = new();

            while (parent[vertex] != vertex)
            {
                stack.Push(vertex);
                vertex = parent[vertex];
            }

            while (stack.Count > 0)
                parent[stack.Pop()] = vertex;

            return vertex;
        }

        public void AddVertex(TVertex vertex)
        {
            if (!vertecies.Add(vertex)) return;

            edgesMap.Add(vertex, new());
        }

        public void RemoveVertex(TVertex vertex)
        {
            if (!vertecies.Remove(vertex)) return;

            edges.RemoveWhere((edge) => edge.HasVertex(vertex));

            edgesMap.Remove(vertex);
            
            foreach (var (_, edges) in edgesMap)
            {
                edges.RemoveWhere((edge) => edge.HasVertex(vertex));
            }
        }

        public void AddEdge(TEdge edge)
        {
            if (!vertecies.Contains(edge.a)) AddVertex(edge.a); // these are HashSets, no need to check anything
            if (!vertecies.Contains(edge.b)) AddVertex(edge.b);

            if (!edges.Add(edge)) return;

            edgesMap.TryGetValue(edge.a, out var a); // should not be null by design
            edgesMap.TryGetValue(edge.b, out var b);

            a.Add(edge);
            b.Add(edge);
        }

        public void RemoveEdge(TEdge edge)
        {
            if (!vertecies.Contains(edge.a) || !vertecies.Contains(edge.b)) return;

            if (!edges.Remove(edge)) return;

            edgesMap.TryGetValue(edge.a, out var a); // should not be null by design
            edgesMap.TryGetValue(edge.b, out var b);

            a.Add(edge);
            b.Add(edge);
        }
    }

    [Serializable]
    public class Graph<TData> : BaseGraph<Vertex<TData>, Edge<TData>, TData>
    {
        public Graph(HashSet<Vertex<TData>> vertecies = null) : base(vertecies) { }
    }

    [Serializable]
    public class Graph3<TData> : BaseGraph<Vertex3<TData>, Edge3<TData>, TData>
    {
        public Graph3(HashSet<Vertex3<TData>> vertecies = null) : base(vertecies) { }
    }
} 