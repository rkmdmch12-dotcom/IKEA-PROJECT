using Microsoft.Maui.Graphics;

public class MapRouteDrawable : IDrawable
{
    public List<PointF> RoutePoints { get; set; } = new();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // צביעת רקע אזור הציור
        canvas.FillColor = Color.FromArgb("#EAEAEA");
        canvas.FillRectangle(dirtyRect);

        if (RoutePoints == null || RoutePoints.Count == 0)
            return;

        // הגדרת עיצוב הקו
        canvas.StrokeColor = Color.FromArgb("#0051BA"); // כחול איקאה
        canvas.StrokeSize = 4;
        canvas.StrokeLineCap = LineCap.Round;

        // ציור הקו שעובר מנקודה לנקודה
        PathF path = new PathF();
        path.MoveTo(RoutePoints[0]);
        for (int i = 1; i < RoutePoints.Count; i++)
        {
            path.LineTo(RoutePoints[i]);
        }
        canvas.DrawPath(path);

        // ציור נקודות בולטות על כל מיקום במסלול
        foreach (var point in RoutePoints)
        {
            // עיגול לבן מסביב לנקודה להבלטה
            canvas.FillColor = Colors.White;
            canvas.FillCircle(point.X, point.Y, 8);

            canvas.StrokeColor = Color.FromArgb("#FFCC00"); // צהוב איקאה מסביב
            canvas.StrokeSize = 2;
            canvas.DrawCircle(point.X, point.Y, 8);

            // נקודה פנימית כחולה
            canvas.FillColor = Color.FromArgb("#0051BA");
            canvas.FillCircle(point.X, point.Y, 4);
        }
    }
}