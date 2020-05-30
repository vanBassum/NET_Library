using System;
using System.Drawing;

namespace FRMLib.Scope
{
    public class PointD
    {
        public double X { get; set; } = double.NaN;
        public double Y { get; set; } = double.NaN;
        public bool IsEmpty { get { return double.IsNaN(X) && double.IsNaN(Y); } set { X = double.NaN; Y = double.NaN; } }


        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }


        public double Distance(PointD p2)
        {
            return Math.Sqrt(Math.Pow(X - p2.X, 2) + Math.Pow(Y - p2.Y, 2));
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




        public static implicit operator PointF(PointD d) => new PointF((float)d.X, (float)d.Y);
        public static implicit operator PointD(Point d) => new PointD((float)d.X, (float)d.Y);

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
