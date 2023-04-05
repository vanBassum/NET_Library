using STDLib.Math;
using STDLib.Extentions;
using FRMLib.Scope.Controls;
using System;
using System.Drawing;

namespace FRMLib.Scope
{
    public abstract class Marker
    {
        public virtual bool Visible { get; set; } = true;
        public virtual double Scale { get; set; } = 1;
        public virtual double Offset { get; set; } = 0;
        public virtual int ColorPaletteIndex { get; set; } = 0;
        public virtual PointD Point { get; set; }
        public string Text { get; set; }

        public void Draw(Graphics g, Style style, Rectangle viewPort, Func<PointD, Point> convert)
        {
            Point pt = convert(Point);

            if (viewPort.CheckIfPointIsWithin(pt) && Visible)
            {
                Color color = style.ColorPalette[ColorPaletteIndex];
                Brush brush = new SolidBrush(color);
                Pen pen = new Pen(color);
                //g.DrawString(Text, style.Font, brush, pt);
                g.DrawCross(pen, viewPort, pt.X, pt.Y, 7, Text, style.Font );
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

        public override int ColorPaletteIndex { get { return Trace.ColorPaletteIndex; } }
        public override double Scale { get => Trace.Scale; }
        public override double Offset { get => Trace.Offset; }
        public override bool Visible { get => Trace.Visible; }


    }


}
