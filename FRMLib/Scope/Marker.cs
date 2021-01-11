using STDLib.Math;
using STDLib.Extentions;
using FRMLib.Scope.Controls;
using System;
using System.Drawing;

namespace FRMLib.Scope
{
    public static class E
    { 
        
    }

    public abstract class Marker
    {
        public virtual bool Visible { get; set; } = true;
        public virtual double Scale { get; set; } = 1;
        public virtual double Offset { get; set; } = 0;
        public virtual Pen Pen { get; set; } = Pens.White;
        public virtual PointD Point { get; set; }
        public string Text { get; set; }

        public void Draw(Graphics g, Rectangle viewPort, Func<PointD, Point> convert, Font font)
        {
            Point pt = convert(Point);

            if (viewPort.CheckIfPointIsWithin(pt) && Visible)
            {
                Brush brush = new SolidBrush(Pen.Color);
                g.DrawString(Text, font, brush, pt);
                g.DrawCross(Pen, pt, 7);
            }
        }
    }


    public class FreeMarker : Marker
    {

        public FreeMarker(double x, double y) { Point = new PointD(x, y); }
    }


    public class LinkedMarker : Marker
    {
        public Trace Trace { get; set; }
        public LinkedMarker(Trace trace) { Trace = trace; }
        public LinkedMarker(Trace trace, double x) { Trace = trace; Point = new PointD(x, trace.GetYValue(x)); }
        public LinkedMarker(Trace trace, double x, double y) { Trace = trace; Point = new PointD(x, y); }

        public override Pen Pen { get { return Trace.Pen; } }
        public override double Scale { get => Trace.Scale; }
        public override double Offset { get => Trace.Offset; }
        public override bool Visible { get => Trace.Visible; }


    }


}
