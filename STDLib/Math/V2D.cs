using System.Drawing;
using System;

namespace STDLib.Math
{
    public class V2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        
        public double Angle { get { return System.Math.Atan2(Y, X); } set { double mag = this.Magnitude; X = mag * System.Math.Cos(value); Y = mag * System.Math.Sin(value); } }
        public double Magnitude { get { return System.Math.Sqrt(X * X + Y * Y); } set { double ang = this.Angle; X = value * System.Math.Cos(ang); Y = value * System.Math.Sin(ang); } }


        public V2D()
        {
            X = 0;
            Y = 0;
        }

        public V2D(double _x, double _y)
        {
            X = _x;
            Y = _y;
        }

        public V2D Rotate(double rotation)
        {
            return Create(Angle + rotation, Magnitude);
        }

        public static V2D Create(double angle, double magnitude)
        {
            return new V2D(magnitude * System.Math.Cos(angle), magnitude * System.Math.Sin(angle));
        }


        public static double DotProduct(V2D lhs, V2D rhs)
        {
            return lhs.X * rhs.X + lhs.Y * rhs.Y;
        }

        public V2D ProjectTo(V2D a)
        {
            return Create(a.Angle, DotProduct(this, a) / a.Magnitude);
        }


        public static V2D operator +(V2D lhs, V2D rhs) => new V2D(lhs.X + rhs.X, lhs.Y + rhs.Y);
        public static V2D operator -(V2D lhs, V2D rhs) => new V2D(lhs.X - rhs.X, lhs.Y - rhs.Y);
        public static V2D operator -(double lhs, V2D rhs) => new V2D(lhs - rhs.X, lhs - rhs.Y);
        public static V2D operator *(V2D vec, double scalair) => new V2D(vec.X * scalair, vec.Y * scalair);
        public static V2D operator *(double scalair, V2D vec) => new V2D(vec.X * scalair, vec.Y * scalair);
        public static V2D operator /(V2D vec, double scalair) => new V2D(vec.X / scalair, vec.Y / scalair);

        public static bool operator !=(V2D lhs, V2D rhs) => !(lhs == rhs);
        public static bool operator ==(V2D lhs, V2D rhs)
        {
            if (object.ReferenceEquals(null, lhs))
                return object.ReferenceEquals(null, rhs);


            return lhs.Equals(rhs);//lhs.X == rhs.X && lhs.Y == rhs.Y;
        }

        


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
