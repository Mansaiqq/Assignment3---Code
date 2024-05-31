﻿//2024 CAB301 Assignment 3 
//TransportationNetwok.cs
//Assignment3B-TransportationNetwork
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class TransportationNetwork
{
    private string[]? intersections; // array storing the names of those intersections in this transportation network design
    private int[,]? distances; // adjacency matrix storing distances between each pair of intersections, if there is a road linking the two intersections

    public string[]? Intersections
    {
        get { return intersections; }
    }

    public int[,]? Distances
    {
        get { return distances; }
    }

    // Read information about a transportation network plan into the system
   public bool ReadFromFile(string filePath)
    {
        //To be completed by students
        if (!File.Exists(filePath))
        {
            return false;
        }
        var lines = File.ReadAllLines(filePath);
        var vertices = new HashSet<string>();
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length == 3)
            {
                vertices.Add(parts[0].Trim());
                vertices.Add(parts[1].Trim());
            }
        }
        intersections = vertices.ToArray();
        distances = new int[vertices.Count, vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            for (int j = 0; j < vertices.Count; j++)
            {
                distances[i, j] = int.MaxValue;
            }
        }
        var verticesIndex = vertices.ToDictionary(v => v, v => Array.IndexOf(intersections, v));
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length == 3)
            {
                var source = parts[0].Trim();
                var target = parts[1].Trim();
                var weight = int.Parse(parts[2].Trim());
                distances[verticesIndex[source], verticesIndex[target]] = weight;
            }
        }
        return true;
    }

    // Check if this transportation network is strongly connected
    public bool IsConnected()
    {
        if (intersections == null || distances == null)
            return false;

        var n = intersections.Length;
        for (int start = 0; start < n; start++)
        {
            var visited = new bool[n];
            DFS(start, visited);
            if (visited.Contains(false))
                return false;
        }
        return true;
    }

    private void DFS(int start, bool[] visited)
    {
        visited[start] = true;
        for (int i = 0; i < distances.GetLength(1); i++)
        {
            if (distances[start, i] != int.MaxValue && !visited[i])
                DFS(i, visited);
        }
    }

    // Find the shortest path between a pair of intersections
    public int FindShortestDistance(string startVertex, string endVertex)
    {
        if (intersections == null || distances == null)
            return -1;

        int startIndex = Array.IndexOf(intersections, startVertex);
        int endIndex = Array.IndexOf(intersections, endVertex);

        if (startIndex == -1 || endIndex == -1)
            return -1;

        var dist = new int[intersections.Length];
        var visited = new bool[intersections.Length];

        for (int i = 0; i < dist.Length; i++)
            dist[i] = int.MaxValue;

        dist[startIndex] = 0;

        for (int i = 0; i < intersections.Length; i++)
        {
            int u = -1;

            for (int j = 0; j < intersections.Length; j++)
            {
                if (!visited[j] && (u == -1 || dist[j] < dist[u]))
                    u = j;
            }

            if (dist[u] == int.MaxValue)
                break;

            visited[u] = true;

            for (int v = 0; v < intersections.Length; v++)
            {
                if (distances[u, v] != int.MaxValue && dist[u] + distances[u, v] < dist[v])
                {
                    dist[v] = dist[u] + distances[u, v];
                }
            }
        }

        return dist[endIndex] == int.MaxValue ? 0 : dist[endIndex];
    }

    // Find the shortest path between all pairs of intersections using Floyd-Warshall algorithm
    public int[,] FindAllShortestDistances()
    {
        if (intersections == null || distances == null)
            return null;

        var n = intersections.Length;
        var dist = (int[,])distances.Clone();

        for (int k = 0; k < n; k++)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (dist[i, k] != int.MaxValue && dist[k, j] != int.MaxValue && dist[i, k] + dist[k, j] < dist[i, j])
                    {
                        dist[i, j] = dist[i, k] + dist[k, j];
                    }
                }
            }
        }

        return dist;
    }

    // Display the transportation network plan with intersections and distances between intersections
    public void DisplayTransportNetwork()
{
    if (intersections == null || distances == null)
    {
        Console.WriteLine("No transportation network data to display.");
        return;
    }

    Console.Write("       ");
    for (int i = 0; i < intersections.Length; i++)
    {
        Console.Write(intersections[i].PadRight(5) + "  ");
    }
    Console.WriteLine();

    for (int i = 0; i < distances.GetLength(0); i++)
    {
        Console.Write(intersections[i].PadRight(5) + "  ");
        for (int j = 0; j < distances.GetLength(1); j++)
        {
            if (i == j) // Self-loop distance should be displayed as infinity
                Console.Write("*".PadRight(5) + "  ");
            else if (distances[i, j] == int.MaxValue)
                Console.Write("*".PadRight(5) + "  ");
            else
                Console.Write(distances[i, j].ToString().PadRight(5) + "  ");
        }
        Console.WriteLine();
    }
}
}