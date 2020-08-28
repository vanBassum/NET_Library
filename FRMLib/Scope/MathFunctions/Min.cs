using FRMLib.Scope.Controls;
using STDLib.Math;
using System;
using System.Drawing;

namespace FRMLib.Scope.MathFunctions
{
    public class Min : MathFunction
    {
        public override object Calculate(MathItem mathItem)
        {
            double min = double.NaN;
            foreach (PointD pt in mathItem.Trace.GetPointsBetweenMarkers(mathItem.Marker1, mathItem.Marker2))
            {
                if (double.IsNaN(min))
                    min = pt.Y;
                else if (pt.Y < min)
                    min = pt.Y;
            }

            return min;
        }

        public override void Draw(Graphics g, MathItem mathItem, Func<double, int> scaleY, Func<double, int> scaleX)
        {
            double maxx = double.NaN;
            double max = double.NaN;
            foreach (PointD pt in mathItem.Trace.GetPointsBetweenMarkers(mathItem.Marker1, mathItem.Marker2))
            {
                if (double.IsNaN(max))
                {
                    max = pt.Y;
                    maxx = pt.X;
                }
                else if (pt.Y < max)
                {
                    max = pt.Y;
                    maxx = pt.X;
                }
            }

            if (!double.IsNaN(max))
            {
                int x = scaleX(maxx);
                int y = scaleY(max);
                g.DrawCross(mathItem.Pen, new Point(x, y), 5);
            }
        }
    }

}
