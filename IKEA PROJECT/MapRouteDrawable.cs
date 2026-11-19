using Microsoft.Maui.Graphics;

namespace IKEA_PROJECT;

public class MapRouteDrawable : IDrawable
{
    public List<PointF> RoutePoints { get; set; } = new();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (RoutePoints == null || RoutePoints.Count == 0)
            return;

        canvas.StrokeColor = Color.FromArgb("#0051BA");
        canvas.StrokeSize = 4;
        canvas.StrokeLineCap = LineCap.Round;

        PathF path = new PathF();
        path.MoveTo(RoutePoints[0]);
        for (int i = 1; i < RoutePoints.Count; i++)
        {
            path.LineTo(RoutePoints[i]);
        }
        canvas.DrawPath(path);

        foreach (var point in RoutePoints)
        {
            canvas.FillColor = Colors.White;
            canvas.FillCircle(point.X, point.Y, 8);

            canvas.StrokeColor = Color.FromArgb("#FFDB00");
            canvas.StrokeSize = 2;
            canvas.DrawCircle(point.X, point.Y, 8);

            canvas.FillColor = Color.FromArgb("#0051BA");
            canvas.FillCircle(point.X, point.Y, 4);
        }
    }
}