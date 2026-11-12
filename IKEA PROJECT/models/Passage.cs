using System;

public class Passage
{
    // Properties עם get ציבורי ו-private set להגנה על הנתונים
    public Point FromPoint { get; private set; }
    public Point ToPoint { get; private set; }
    public double Distance { get; private set; }

    // בנאי המחלקה שמקבל את נקודת המוצא, היעד והמרחק
    public Passage(Point from, Point to, double distance)
    {
        // 1. בדיקה האם נקודת המוצא או היעד קיימות (לא null)
        if (from == null)
        {
            throw new ArgumentNullException(nameof(from), "נקודת המוצא (From) לא יכולה להיות ריקה.");
        }
        if (to == null)
        {
            throw new ArgumentNullException(nameof(to), "נקודת היעד (To) לא יכולה להיות ריקה.");
        }

        // 2. בדיקה האם המוצא והיעד הם שתי נקודות שונות
        // (הנחה שיש לפחות מאפיינים מזהים כמו X ו-Y או מזהה ייחודי Id בתוך מחלקת Point)
        if (from.X == to.X && from.Y == to.Y) // או בדיקה לפי Id אם קיים: if (from.Id == to.Id)
        {
            throw new ArgumentException("נקודת המוצא ונקודת היעד חייבות להיות שונות (אי אפשר מעבר לעצמו).");
        }

        // 3. בדיקה שהמרחק הגיוני (גדול מאפס)
        if (distance <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(distance), "המרחק חייב להיות גדול מאפס.");
        }

        // אם הכל תקין, שומרים את הערכים
        FromPoint = from;
        ToPoint = to;
        Distance = distance;
    }

    // מתודה נוחה להדפסה או דיבוג (ToString)
    public override string ToString()
    {
        return $"Passage from ({FromPoint.X}, {FromPoint.Y}) to ({ToPoint.X}, {ToPoint.Y}) | Distance: {Distance}";
    }
}