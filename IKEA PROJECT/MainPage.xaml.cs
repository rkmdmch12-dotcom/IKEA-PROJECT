using Microsoft.Maui.Graphics;

namespace IKEA_PROJECT;

public partial class MainPage : ContentPage
{
    private readonly string currentCity;
    private readonly MapRouteDrawable mapDrawable = new();
    private readonly List<PointF> tappedPoints = new();

    public class CityMapConfig
    {
        public string ImageFileName { get; set; } = string.Empty;
        public Dictionary<string, PointF> Coordinates { get; set; } = new();
    }

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

    private readonly Dictionary<string, List<(string destination, double distance)>> map = new();
    private readonly Dictionary<string, CityMapConfig> cityMaps = new();

    public MainPage() : this("תל אביב")
    {
    }

    public MainPage(string city)
    {
        InitializeComponent();
        currentCity = city;
        CityTitleLabel.Text = $"איקאה • {currentCity}";
        Title = currentCity;

        mapGraphicsView.Drawable = mapDrawable;

        StartPicker.ItemsSource = locations;
        TargetPicker.ItemsSource = locations;

        SetupCityMaps();
        LoadCityImage();
        BuildMap();
    }

    private void SetupCityMaps()
    {
        cityMaps["תל אביב"] = new CityMapConfig
        {
            ImageFileName = "ta_map.png",
            Coordinates = new Dictionary<string, PointF>
            {
                { "כניסה", new PointF(40, 180) },
                { "סלון", new PointF(110, 120) },
                { "מטבחים", new PointF(110, 210) },
                { "פינת אוכל", new PointF(190, 210) },
                { "חדרי שינה", new PointF(210, 120) },
                { "תאורה", new PointF(170, 55) },
                { "קופות", new PointF(270, 150) }
            }
        };

        cityMaps["ראשון לציון"] = new CityMapConfig
        {
            ImageFileName = "rishon_map.png",
            Coordinates = new Dictionary<string, PointF>
            {
                { "כניסה", new PointF(35, 190) },
                { "סלון", new PointF(95, 140) },
                { "מטבחים", new PointF(145, 215) },
                { "פינת אוכל", new PointF(205, 190) },
                { "חדרי שינה", new PointF(230, 115) },
                { "תאורה", new PointF(155, 50) },
                { "קופות", new PointF(275, 165) }
            }
        };

        cityMaps["באר שבע"] = new CityMapConfig
        {
            ImageFileName = "beer_sheva_map.png",
            Coordinates = new Dictionary<string, PointF>
            {
                { "כניסה", new PointF(45, 175) },
                { "סלון", new PointF(115, 130) },
                { "מטבחים", new PointF(135, 205) },
                { "פינת אוכל", new PointF(195, 200) },
                { "חדרי שינה", new PointF(225, 125) },
                { "תאורה", new PointF(165, 60) },
                { "קופות", new PointF(265, 155) }
            }
        };
    }

    private void LoadCityImage()
    {
        if (cityMaps.TryGetValue(currentCity, out var config) && !string.IsNullOrWhiteSpace(config.ImageFileName))
        {
            CityMapImage.Source = config.ImageFileName;
        }
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

    private void AddConnection(string first, string second, double distance)
    {
        map[first].Add((second, distance));
        map[second].Add((first, distance));
    }

    private void CalculateRoute_Clicked(object sender, EventArgs e)
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
            StatusLabel.TextColor = Colors.Red;
            return;
        }

        RouteLabel.Text = string.Join(" ← ", result.path);

        double distanceInKm = result.distance / 1000.0;
        DistanceKmLabel.Text = $"{distanceInKm:0.##} ק\"מ";

        double minutes = Math.Ceiling(result.distance / 60.0);
        TimeLabel.Text = $"{minutes} דקות";

        StatusLabel.Text = "המסלול חושב בהצלחה!";
        StatusLabel.TextColor = Colors.Green;

        DrawRouteOnMap(result.path);
    }

    private void DrawRouteOnMap(List<string> path)
    {
        if (!cityMaps.TryGetValue(currentCity, out var cityConfig))
            return;

        var calculatedPoints = new List<PointF>();

        foreach (var locationName in path)
        {
            if (cityConfig.Coordinates.TryGetValue(locationName, out var point))
            {
                calculatedPoints.Add(point);
            }
        }

        mapDrawable.RoutePoints = calculatedPoints;
        mapGraphicsView.Invalidate();
    }

    private (List<string> path, double distance) FindShortestPath(string start, string target)
    {
        var distances = locations.ToDictionary(x => x, x => double.PositiveInfinity);
        var previous = new Dictionary<string, string>();
        var visited = new HashSet<string>();

        distances[start] = 0;

        while (visited.Count < locations.Count)
        {
            string? current = locations
                .Where(x => !visited.Contains(x))
                .OrderBy(x => distances[x])
                .FirstOrDefault();

            if (current == null || double.IsPositiveInfinity(distances[current]))
                break;

            if (current == target)
                break;

            visited.Add(current);

            foreach (var neighbor in map[current])
            {
                double newDistance = distances[current] + neighbor.distance;

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

    private void OnMapTapped(object sender, TappedEventArgs e)
    {
        if (e.GetPosition(mapGraphicsView) is not Point tapPos)
            return;

        PointF clickedPoint = new PointF((float)tapPos.X, (float)tapPos.Y);

        if (tappedPoints.Count >= 2)
        {
            tappedPoints.Clear();
        }

        tappedPoints.Add(clickedPoint);
        mapDrawable.RoutePoints = new List<PointF>(tappedPoints);
        mapGraphicsView.Invalidate();
    }
}