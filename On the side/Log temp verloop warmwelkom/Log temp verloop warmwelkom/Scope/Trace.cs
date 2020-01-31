using System;
using System.Collections.Generic;
using System.Drawing;

namespace Oscilloscope
{
    public class Trace
    {
        public Color   Colour   {get; set;}= Color.Red;
        public bool    Visible  {get; set;}= true;
        public string  Name     {get; set;}= "Trace";
        public string  Unit     {get; set;}= "";
        public double  Scale    {get; set;}= 1f;
        public double  Position { get; set; } = 0f;
        public List<PointD> Points { get; set; } = new List<PointD>();
        public List<Mark> Marks { get; set; } = new List<Mark> { };
        public TraceDrawStyle DrawStyle { get; set; } = TraceDrawStyle.Points;

        public Trace Self { get { return this; } }
        //public double XMAX { get; private set; } = 0;
        //public double XMIN { get; private set; } = 0;

        /*
        public override string ToString()
        {
            return Name;
        }*/

        [Flags]
        public enum TraceDrawStyle
        {
            None =              0,
            Points =            1,
            Lines =             2,
            NoInterpolation =   4,
            ExtendBegin =       8,
            ExtendEnd =         16,
        }

        public void AutoScale(int divisions)
        {
            double max = 0;
            double min = 0;

            double xMax = double.NegativeInfinity;
            double xMin = double.PositiveInfinity;

            foreach(PointD pt in Points)
            {
                double y = pt.Y;
                if (y > max)
                    max = y;
                if (y < min)
                    min = y;

                if (pt.X > xMax)
                    xMax = pt.X;
                if (pt.X < xMin)
                    xMin = pt.X;
            }

            //XMAX = xMax;
            //XMIN = xMin;

            double scaleA = Math.Abs( 2 * max / (divisions - (2 * Position)));
            double scaleB = Math.Abs(2 * min / (divisions - (2 * Position)));

            double biggestScale = scaleA > scaleB ? scaleA : scaleB;

            Scale = CalcUnitsPerDivScale(biggestScale);
            if (Scale == 0)
                Scale = 1;


        }

        double CalcUnitsPerDivScale(double xPerDiv)
        {
            int xTens = (int)Math.Log10(xPerDiv);
            double xScaled = xPerDiv * Math.Pow(10, -xTens);
            double xWhole = xScaled < 1 ? 1 : xScaled < 2 ? 2 : xScaled < 5 ? 5 : 10;
            return xWhole * Math.Pow(10, xTens);
        }

        public double GetYValue(double x)
        {
            int i;

            if (Points.Count == 0)
                return 0;

            for (i = 0; i < Points.Count; i++)
            {
                if (Points[i].X > x)
                    break;
            }
            i -= 1;
            
            if (DrawStyle.HasFlag(TraceDrawStyle.Lines))
            {
                if (DrawStyle.HasFlag(TraceDrawStyle.NoInterpolation))
                {

                    if (i == 0 && DrawStyle.HasFlag(TraceDrawStyle.ExtendBegin))
                        return Points[i].Y;

                    if (i == Points.Count - 1 && DrawStyle.HasFlag(TraceDrawStyle.ExtendEnd))
                        return Points[i].Y;

                    if (i > 0 & i + 1 < Points.Count)
                        return Points[i].Y;
                }
                else
                {
                    if (i >= 0 & i + 1 < Points.Count)
                    {
                        double x1 = Points[i].X;
                        double x2 = Points[i + 1].X;
                        double y1 = Points[i].Y;
                        double y2 = Points[i + 1].Y;
                        double a = (y2 - y1) / (x2 - x1);
                        double b = y2 - a * x2;
                        double y = a * x + b;
                        return y;
                    }
                }
            }
            else
            {
                /*
                if (DrawStyle.HasFlag(TraceDrawStyle.Points))
                {
                    if(i == 0)
                        return Points[i].Y;
                    else if(i == Points.Count -1)
                        return Points[i].Y;
                    else
                    {

                    }
                }
                */
            }
            
            return 0;
        }
    }

}
