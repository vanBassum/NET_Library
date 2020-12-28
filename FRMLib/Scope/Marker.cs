using STDLib.Math;
using System.Drawing;

namespace FRMLib.Scope
{
    public abstract class Marker
    {
        public virtual double Scale { get; set; } = 1;
        public virtual double Offset { get; set; } = 0;
        public virtual Pen Pen { get; set; } = Pens.White;
        public virtual PointD Point { get; set; }
        public string Text { get; set; }
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

        public override Pen Pen { get; set; }
        public override double Scale { get => Trace.Scale; }
        public override double Offset { get => Trace.Offset; }

    }


}
