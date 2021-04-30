using System;
using System.Drawing;

namespace STDLib.Math.Figures
{
    public class Circle : Figure
    {
        public V2D CenterPos { get; set; }
        public double Radius { get; set; }


        /*
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
        */
    }
}
