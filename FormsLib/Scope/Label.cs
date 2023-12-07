using FormsLib.Scope.Controls;
using System;
using System.Drawing;
using FormsLib.Extentions;
using FormsLib.Maths;

namespace FormsLib.Scope
{
    public abstract class Label
    {
        public virtual bool Visible { get; set; } = true;
        public virtual double Scale { get; set; } = 1;
        public virtual double Offset { get; set; } = 0;
        public virtual Color Color { get; set; } = Color.Green;
        public virtual PointD Point { get; set; }
        public string Text { get; set; }
        public int MaxLines { get; set; } = 1;

        public void Draw(Graphics g, Style style, Rectangle viewPort, Func<PointD, Point> convert)
        {
            Point pt = convert(Point);

            if (viewPort.CheckIfPointIsWithin(pt) && Visible)
            {
                Brush brush = new SolidBrush(Color);
                Pen pen = new Pen(Color);
                //g.DrawString(Text, style.Font, brush, pt);

                string[] lines = Text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string truncatedText = string.Join(Environment.NewLine, lines.Take(MaxLines));
                g.DrawCross(pen, viewPort, pt.X, pt.Y, 5, truncatedText, style.Font );
            }
        }
    }


    public class FreeLabel : Label
    {
        public FreeLabel(double x, double y) { Point = new PointD(x, y); }
    }


    public class LinkedLabel : Label
    {
        public Trace Trace { get; set; }
        public LinkedLabel(Trace trace) { Trace = trace; }
        public LinkedLabel(Trace trace, double x) { Trace = trace; Point = new PointD(x, trace.GetYValue(x)); }
        public LinkedLabel(Trace trace, double x, double y) { Trace = trace; Point = new PointD(x, y); }

        public override Color Color { get { return Trace.Color; } }
        public override double Scale { get => Trace.Scale; }
        public override double Offset { get => Trace.Offset; }
        public override bool Visible { get => Trace.Visible; }


    }


}
