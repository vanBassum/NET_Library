using STDLib.Math;
using System;
using System.Drawing;
using System.Linq;

namespace FRMLib.Scope.MathFunctions
{
    public class LinearRegression : MathFunction
    {


        public override object Calculate(MathItem mathItem)
        {


            return 0;
        }


        public override void Draw(Graphics g, MathItem mathItem, Func<double, int> scaleY, Func<double, int> scaleX)
        {
            double k1 = 0;
            double k2 = 0;
            double k3 = 0;
            double k4 = 0;

            double xAvg = 0;
            double yAvg = 0;

            PointD[] points = mathItem.Trace.GetPointsBetweenMarkers(mathItem.Marker1, mathItem.Marker2).ToArray();

            foreach (PointD pt in points)
            {
                k1 += pt.X * pt.Y;
                k2 += pt.X;
                k3 += pt.Y;
                k4 += Math.Pow(pt.X, 2);
                xAvg += pt.X;
                yAvg += pt.Y;
            }

            xAvg /= points.Length;
            yAvg /= points.Length;


            double a1 = ((points.Length * k1) - (k2 * k3)) / (points.Length * k4 - Math.Pow(k2, 2));
            double a0 = yAvg - (a1 * xAvg);


            int x1 = scaleX(mathItem.Marker1.X);
            int x2 = scaleX(mathItem.Marker2.X);
            int y1 = scaleY(mathItem.Marker1.X * a1 + a0);
            int y2 = scaleY(mathItem.Marker2.X * a1 + a0);

            g.DrawLine(mathItem.Pen, x1, y1, x2, y2);

        }
    }
}
