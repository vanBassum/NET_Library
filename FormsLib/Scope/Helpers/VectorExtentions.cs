using System.Numerics;

namespace FormsLib.Scope.Helpers
{
    public static class VectorExtentions
    {
        // Point
        public static Vector2 ToVector2(this Point point) => new Vector2(point.X, point.Y);
        public static Point ToPoint(this Vector2 vector) => new Point((int)vector.X, (int)vector.Y);
        public static IEnumerable<Point> ToPoints(this IEnumerable<Vector2> vectors) => vectors.Select(v => v.ToPoint());

        // PointF
        public static Vector2 ToVector2(this PointF point) => new Vector2(point.X, point.Y);
        public static PointF ToPointF(this Vector2 vector) => new PointF(vector.X, vector.Y);
        public static IEnumerable<PointF> ToPointsF(this IEnumerable<Vector2> vectors) => vectors.Select(v => v.ToPointF());

        // Size
        public static Vector2 ToVector2(this Size size) => new Vector2(size.Width, size.Height);
        public static Size ToSize(this Vector2 vector) => new Size((int)vector.X, (int)vector.Y);
        public static IEnumerable<Size> ToSizes(this IEnumerable<Vector2> vectors) => vectors.Select(v => v.ToSize());

        // SizeF
        public static Vector2 ToVector2(this SizeF size) => new Vector2(size.Width, size.Height);
        public static SizeF ToSizeF(this Vector2 vector) => new SizeF(vector.X, vector.Y);
        public static IEnumerable<SizeF> ToSizesF(this IEnumerable<Vector2> vectors) => vectors.Select(v => v.ToSizeF());
    }
}
