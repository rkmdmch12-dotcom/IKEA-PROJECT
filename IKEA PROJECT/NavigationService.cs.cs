using System;
using System.Collections.Generic;
using System.Linq;

namespace IkeaNavigation;


public class NavigationService
{
    private readonly Dictionary<string, List<Edge>> map = new();

    public void AddPoint(string name)
    {
        if (!map.ContainsKey(name))
            map[name] = new List<Edge>();
    }

    public void AddPath(string from, string to, double distance)
    {
        if (!map.ContainsKey(from) || !map.ContainsKey(to))
            throw new ArgumentException("יש להוסיף את הנקודות לפני יצירת מעבר.");

        if (distance < 0)
            throw new ArgumentException("המרחק לא יכול להיות שלילי.");

        map[from].Add(new Edge(to, distance));
        map[to].Add(new Edge(from, distance));
    }

    public (List<string> Route, double Distance) FindShortestPath(
        string start, string destination)
    {
        if (!map.ContainsKey(start) || !map.ContainsKey(destination))
            return (new List<string>(), double.PositiveInfinity);

        var distances = map.Keys.ToDictionary(
            point => point,
            point => double.PositiveInfinity);

        var previous = new Dictionary<string, string>();
        var unvisited = new HashSet<string>(map.Keys);

        distances[start] = 0;

        while (unvisited.Count > 0)
        {
            string? current = null;
            double shortestDistance = double.PositiveInfinity;

            foreach (string point in unvisited)
            {
                if (distances[point] < shortestDistance)
                {
                    shortestDistance = distances[point];
                    current = point;
                }
            }

            if (current == null)
                break;

            if (current == destination)
                break;

            unvisited.Remove(current);

            foreach (Edge edge in map[current])
            {
                if (!unvisited.Contains(edge.Destination))
                    continue;

                double newDistance =
                    distances[current] + edge.Distance;

                if (newDistance < distances[edge.Destination])
                {
                    distances[edge.Destination] = newDistance;
                    previous[edge.Destination] = current;
                }
            }
        }

        if (double.IsPositiveInfinity(distances[destination]))
            return (new List<string>(), double.PositiveInfinity);

        var route = new List<string>();
        string step = destination;
        route.Add(step);

        while (step != start)
        {
            if (!previous.ContainsKey(step))
                return (new List<string>(), double.PositiveInfinity);

            step = previous[step];
            route.Add(step);
        }

        route.Reverse();

        return (route, distances[destination]);
    }

    public static NavigationService CreateDemoMap()
    {
        var navigation = new NavigationService();

        navigation.AddPoint("כניסה");
        navigation.AddPoint("צומת מרכזי");
        navigation.AddPoint("סלונים");
        navigation.AddPoint("מיטות");
        navigation.AddPoint("קופות");

        navigation.AddPath("כניסה", "צומת מרכזי", 10);
        navigation.AddPath("צומת מרכזי", "סלונים", 6);
        navigation.AddPath("צומת מרכזי", "מיטות", 8);
        navigation.AddPath("סלונים", "מיטות", 4);
        navigation.AddPath("סלונים", "קופות", 12);
        navigation.AddPath("מיטות", "קופות", 5);

        return navigation;
    }
}