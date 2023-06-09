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
        public int Count => VerteciesAndEdges.Count;

        public BetterDictionary<TVertex, HashSet<TEdge>> VerteciesAndEdges;

        public IEnumerable<TVertex> Vertecies => VerteciesAndEdges.Keys;
        public IEnumerable<HashSet<TEdge>> EdgeSets => VerteciesAndEdges.Values;
        public IEnumerable<TEdge> Edges => EdgeSets.SelectMany((edges) => edges);

        //public HashSet<TEdge> edges;

        public BaseGraph(HashSet<TVertex> vertecies = null)
        {
            VerteciesAndEdges = new();

            if (vertecies != null)
            {
                foreach (var vertex in vertecies)
                {
                    VerteciesAndEdges[vertex] = new();
                }
            }
        }

        // https://www.wikiwand.com/de/Algorithmus_von_Kruskal
        // without any thinking implemented. Works but needs to be looked at again.
        // hmm, reafactor it later.
        public void MinimumSpanningTree(out HashSet<TEdge> removedEdges)
        {
            // Create a list to store the minimum spanning tree edges
            List<TEdge> minimumSpanningTree = new();

            // Create a dictionary to track the parent of each vertex in the tree
            Dictionary<TVertex, TVertex> parent = new();

            // Create a dictionary to track the rank of each vertex in the tree
            Dictionary<TVertex, int> rank = new();

            // Initialize the parent and rank dictionaries
            foreach (var vertex in Vertecies)
            {
                parent[vertex] = vertex;
                rank[vertex] = 0;
            }

            // Sort all edges in ascending order by weight
            List<TEdge> sortedEdges = Edges.ToList();
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


            removedEdges = Edges.Except(minimumSpanningTree).ToHashSet();

            // Update the edges of the graph to contain only the minimum spanning tree edges

            RemoveEdges(removedEdges);
            
            //AddEdges(minimumSpanningTree);

            //Debug.Log(Edges.Count() + " <---");
            //Debug.Log(removedEdges.Count() + " <------");
        }

        private TVertex FindRoot(TVertex vertex, Dictionary<TVertex, TVertex> parent)
        {
            Stack<TVertex> stack = new();

            while (parent[vertex] != vertex)
            {
                stack.Push(vertex);
                vertex = parent[vertex];
            }

            //while (stack.Count > 0)
            //    parent[stack.Pop()] = vertex;

            return vertex;
        }

        public void AddVertex(TVertex vertex)
        {
            VerteciesAndEdges[vertex] ??= new();
        }

        public void RemoveVertex(TVertex vertex)
        {
            VerteciesAndEdges.Remove(vertex);

            foreach (var edges in EdgeSets)
            {
                edges.RemoveWhere((edge) => edge.HasVertex(vertex));
            }
        }

        public void AddEdge(TEdge edge)
        {
            AddVertex(edge.a);
            AddVertex(edge.b);

            VerteciesAndEdges[edge.a].Add(edge);
            VerteciesAndEdges[edge.b].Add(edge);
        }

        public void RemoveEdge(TEdge edge)
        {
            VerteciesAndEdges[edge.a]?.Remove(edge);
            VerteciesAndEdges[edge.b]?.Remove(edge);
        }

        public void AddVertecies(IEnumerable<TVertex> vertecies)
        {
            foreach (var vertex in vertecies)
                AddVertex(vertex);
        }

        public void RemoveVertecies(IEnumerable<TVertex> vertecies)
        {
            foreach (var vertex in vertecies)
                RemoveVertex(vertex);
        }

        public void AddEdges(IEnumerable<TEdge> edges)
        {
            if (edges == null)
                return;

            foreach (var edge in edges)
                AddEdge(edge);
        }

        public void RemoveEdges(IEnumerable<TEdge> edges)
        {
            foreach (var edge in edges.ToList())
                RemoveEdge(edge);
        }

        public void MergeGraph(BaseGraph<TVertex, TEdge, TData> graph)
        {
            foreach (var vertex in graph.Vertecies)
                AddVertex(vertex);

            foreach (var edge in graph.Edges)
                AddEdge(edge);
        }
    }

    public class Graph<TData> : BaseGraph<Vertex<TData>, Edge<TData>, TData>
    {
        public Graph(HashSet<Vertex<TData>> vertecies = null) : base(vertecies) { }
    }

    public class Graph3<TData> : BaseGraph<Vertex3<TData>, Edge3<TData>, TData>
    {
        public Graph3(HashSet<Vertex3<TData>> vertecies = null) : base(vertecies) { }

        public void SetWeights(Func<Vertex3<TData>, Vertex3<TData>, float> weightFunction)
        {
            foreach (var edge in Edges)
            {
                edge.weight = weightFunction(edge.a, edge.b);
            }
        }

        public void SetWeightsToDistance()
        {
            SetWeights((a, b) => Vector3.Distance(a.position, b.position));
        }

        public void DrawGizmos()
        {
            foreach (var vertex in Vertecies)
            {
                Gizmos.DrawSphere(vertex.position, 0.1f);
            }

            foreach (var edge in Edges)
            {
                Gizmos.DrawLine(edge.a.position, edge.b.position);
            }
        }
    }
}