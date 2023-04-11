using CoreLib.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FormsLib.Scope.MathFunctions
{
    public class AVG : MathFunction
    {
        double N = 2;

        public override object Calculate(MathItem mathItem)
        {
            double avg = 0;
            IEnumerable<PointD> pts = mathItem.Trace.GetPointsBetweenMarkers(mathItem.Marker1, mathItem.Marker2);
            foreach (PointD pt in pts)
                avg += pt.Y;


            return avg / pts.Count();
        }

        public override void Draw(Graphics g, MathItem mathItem, Func<double, int> scaleY, Func<double, int> scaleX)
        {
            double avg = 0;
            int px = 0;
            int py = 0;
            int x = 0;
            int y = 0;
            bool first = true;
            foreach (PointD pt in mathItem.Trace.GetPointsBetweenMarkers(mathItem.Marker1, mathItem.Marker2))
            {
                avg = avg + (pt.Y - avg) / N;
                px = x;
                py = y;
                x = scaleX(pt.X);
                y = scaleY(avg);
                if (!first)
                    g.DrawLine(mathItem.Pen, px, py, x, y);
                first = false;
            }
        }
    }
}
