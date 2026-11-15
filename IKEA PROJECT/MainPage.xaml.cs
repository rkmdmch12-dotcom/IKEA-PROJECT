
using System;
using System.Collections.Generic;
using System.Linq;

namespace IKEA_PROJECT;

public partial class MainPage : ContentPage
{
    private readonly List<string> locations = new()
    {
        "כניסה",
        "סלון",
        "מטבחים",
        "פינת אוכל",
        "חדרי שינה",
        "תאורה",
        "קופות"
    };

    private readonly Dictionary<string,
        List<(string destination, double distance)>> map = new();

    public MainPage()
    {
        InitializeComponent();

        StartPicker.ItemsSource = locations;
        TargetPicker.ItemsSource = locations;

        BuildMap();
    }

    private void BuildMap()
    {
        foreach (string location in locations)
        {
            map[location] = new List<(string, double)>();
        }

        AddConnection("כניסה", "סלון", 30);
        AddConnection("כניסה", "מטבחים", 55);
        AddConnection("סלון", "חדרי שינה", 40);
        AddConnection("סלון", "תאורה", 25);
        AddConnection("מטבחים", "פינת אוכל", 35);
        AddConnection("פינת אוכל", "חדרי שינה", 45);
        AddConnection("תאורה", "קופות", 60);
        AddConnection("חדרי שינה", "קופות", 35);
        AddConnection("פינת אוכל", "קופות", 50);
    }

    private void AddConnection(
        string first, string second, double distance)
    {
        map[first].Add((second, distance));
        map[second].Add((first, distance));
    }

    private void CalculateRoute_Clicked(
        object sender, EventArgs e)
    {
        string? start = StartPicker.SelectedItem as string;
        string? target = TargetPicker.SelectedItem as string;

        if (start == null || target == null)
        {
            StatusLabel.Text = "יש לבחור נקודת מוצא ויעד";
            StatusLabel.TextColor = Colors.Red;
            return;
        }

        if (start == target)
        {
            StatusLabel.Text = "יש לבחור שתי נקודות שונות";
            StatusLabel.TextColor = Colors.Red;
            return;
        }

        var result = FindShortestPath(start, target);

        if (result.path.Count == 0)
        {
            StatusLabel.Text = "לא נמצא מסלול";
            return;
        }

        RouteLabel.Text =
            string.Join(" ← ", result.path);

        LengthLabel.Text =
            $"מרחק: {result.distance} מטר";

        TimeLabel.Text =
            $"זמן הליכה משוער: {Math.Ceiling(result.distance / 60)} דקות";

        StatusLabel.Text = "המסלול חושב בהצלחה!";
        StatusLabel.TextColor = Colors.Green;
    }

    private (List<string> path, double distance)
        FindShortestPath(string start, string target)
    {
        var distances = locations.ToDictionary(
            x => x,
            x => double.PositiveInfinity);

        var previous = new Dictionary<string, string>();
        var visited = new HashSet<string>();

        distances[start] = 0;

        while (visited.Count < locations.Count)
        {
            string? current = locations
                .Where(x => !visited.Contains(x))
                .OrderBy(x => distances[x])
                .FirstOrDefault();

            if (current == null ||
                double.IsPositiveInfinity(distances[current]))
                break;

            if (current == target)
                break;

            visited.Add(current);

            foreach (var neighbor in map[current])
            {
                double newDistance =
                    distances[current] + neighbor.distance;

                if (newDistance < distances[neighbor.destination])
                {
                    distances[neighbor.destination] = newDistance;
                    previous[neighbor.destination] = current;
                }
            }
        }

        if (double.IsPositiveInfinity(distances[target]))
            return (new List<string>(), 0);

        var path = new List<string>();
        string step = target;

        path.Add(step);

        while (step != start)
        {
            if (!previous.ContainsKey(step))
                return (new List<string>(), 0);

            step = previous[step];
            path.Add(step);
        }

        path.Reverse();

        return (path, distances[target]);
    }
}




