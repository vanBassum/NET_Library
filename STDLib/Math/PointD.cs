using System.Drawing;

namespace STDLib.Math
{

    public class PointD
    {
        public double X { get; set; }
        public double Y { get; set; }
        public bool IsEmpty { get { return double.IsNaN(X) && double.IsNaN(Y); } set { X = double.NaN; Y = double.NaN; } }


        public PointD()
        {
            X = double.NaN;
            Y = double.NaN;
        }

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void Scale(double offsetX, double scaleX, double offsetY, double scaleY)
        {
            X = offsetX + X * scaleX;
            Y = offsetY + Y * scaleY;
        }

        public void KeepMinimum(PointD pt)
        {
            if (IsEmpty)
            {
                X = pt.X;
                Y = pt.Y;
            }
            else
            {
                X = pt.X < X ? pt.X : X;
                Y = pt.Y < Y ? pt.Y : Y;
            }
        }

        public void KeepMaximum(PointD pt)
        {
            if (IsEmpty)
            {
                X = pt.X;
                Y = pt.Y;
            }
            else
            {
                X = pt.X > X ? pt.X : X;
                Y = pt.Y > Y ? pt.Y : Y;
            }
        }



        public static implicit operator PointD(V2D s) => new PointD(s.X, s.Y);
        public static implicit operator V2D(PointD s) => new V2D(s.X, s.Y);
        public static explicit operator Point(PointD d) => new Point((int)d.X, (int)d.Y);

        /*
        public static explicit operator Point(PointD d) => new PointF((float)d.X, (float)d.Y);
        public static explicit operator PointF(PointD d) => new PointF((float)d.X, (float)d.Y);
        public static explicit operator PointD(Point d) => new PointD((float)d.X, (float)d.Y);
        */

        public static PointD Empty
        {
            get
            {
                PointD pt = new PointD(0, 0);
                pt.IsEmpty = true;
                return pt;
            }
        }

    }
}
