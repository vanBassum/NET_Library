using System.Drawing;

namespace FormsLib.Maths
{
    public class V2D
    {
        public double X { get; set; } = double.NaN;
        public double Y { get; set; } = double.NaN;

        public double Distance(PointD p2)
        {
            return Math.Sqrt(Math.Pow(X - p2.X, 2) + Math.Pow(Y - p2.Y, 2));
        }

        public V2D()
        {

        }

        public V2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static V2D operator +(V2D a, V2D b) => new V2D(a.X + b.X, a.Y + b.Y);
        public static V2D operator -(V2D a, V2D b) => new V2D(a.X - b.X, a.Y - b.Y);

        public static V2D operator *(V2D a, double b) => new V2D(a.X * b, a.Y * b);
        public static V2D operator *(double a, V2D b) => new V2D(a * b.X, a * b.Y);
        public static V2D operator *(V2D a, V2D b) => new V2D(a.X * b.X, a.Y * b.Y);

        public static V2D operator /(V2D a, double b) => new V2D(a.X / b, a.Y / b);
        public static V2D operator /(double a, V2D b) => new V2D(a / b.X, a / b.Y);

        public static explicit operator V2D(Point p) => new V2D(p.X, p.Y);
        public static explicit operator V2D(PointF p) => new V2D(p.X, p.Y);
        public static explicit operator V2D(Size p) => new V2D(p.Width, p.Height);
        public static explicit operator V2D(SizeF p) => new V2D(p.Width, p.Height);

        public static explicit operator Point(V2D p) => new Point((int)p.X, (int)p.Y);
        public static explicit operator PointF(V2D p) => new PointF((float)p.X, (float)p.Y);
        public static explicit operator Size(V2D p) => new Size((int)p.X, (int)p.X);
        public static explicit operator SizeF(V2D p) => new SizeF((float)p.X, (float)p.X);


    }
}
