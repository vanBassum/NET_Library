using FRMLib.Scope.Controls;
using STDLib.Math;
using STDLib.Misc;
using STDLib.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using STDLib.Math.Figures;

namespace FRMLib.Scope
{

    public abstract class Trace : PropertySensitive
    {
        public Trace Self { get { return this; } }
        [TraceViewAttribute(Text = "", Width = 20)]
        public System.Drawing.Pen Pen { get { return GetPar(System.Drawing.Pens.Red); } set { SetPar(value); } }
        [TraceViewAttribute(Width = 20, Text = "")]
        public bool Visible { get { return GetPar(true); } set { SetPar(value); } }
        [TraceViewAttribute(Width = 50)]
        public string Name { get { return GetPar("New Trace"); } set { SetPar(value); } }
        //[TraceViewAttribute]
        public string Unit { get { return GetPar(""); } set { SetPar(value); } }
        [TraceViewAttribute(Width = 40)]
        public double Scale { get { return GetPar<double>(1f); } set { SetPar<double>(value); } }
        [TraceViewAttribute(Width = 40)]
        public double Offset { get { return GetPar<double>(0f); } set { SetPar<double>(value); } }
        //[TraceViewAttribute(Width = 40)]
        public int Layer { get { return GetPar(10); } set { SetPar(value); } }
        [TraceViewAttribute(Width = 80)]
        public DrawStyles DrawStyle { get { return GetPar(DrawStyles.Lines); } set { SetPar(value); } }
        [TraceViewAttribute]
        public DrawOptions DrawOption { get { return GetPar(DrawOptions.ShowScale); } set { SetPar(value); } }
        public Func<double, string> ToHumanReadable { get { return GetPar(new Func<double, string>((a) => a.ToHumanReadable(3) + Unit)); } set { SetPar(value); } }

        public abstract ThreadedBindingList<V2D> Points { get; }

        public abstract double GetYValue(double x);


        public virtual void Draw(System.Drawing.Graphics g, Rectangle viewPort, Func<V2D, V2D> scale)
        {
            System.Drawing.Pen pen = Pen;
            System.Drawing.Brush brush = new System.Drawing.SolidBrush(pen.Color);

            if (Visible)
            {
                int pointCnt = Points.Count;
                //Only draw points within screen, this can be calculated.

                for (int i = 0; i < pointCnt; i++)
                {
                    bool last = (i == pointCnt - 1);
                    bool first = i == 0;

                    V2D pPrevD = new V2D(0, 0);
                    V2D pActD = Points[i];
                    V2D pNextD = new V2D(0, 0);

                    if (!first)
                        pPrevD = Points[i - 1];

                    if (!last)
                        pNextD = Points[i + 1];

                    V2D pPrev = scale(pPrevD);
                    V2D pAct = scale(pActD);
                    V2D pNext = scale(pNextD);


                    List<Figure> figures = new List<Figure>();



                    switch (DrawStyle)
                    {
                        case Trace.DrawStyles.Points:
                            figures.Add(new Circle { CenterPos = pAct, Radius = 4 });
                            break;

                        case Trace.DrawStyles.Lines:
                            if (!last)
                                figures.Add(new Line(pAct, pNext));

                            break;
                    }



                    g.DrawFigure(pen, viewPort.ToLines()[0]);
                    g.DrawFigure(pen, viewPort.ToLines()[1]);
                    g.DrawFigure(pen, viewPort.ToLines()[2]);
                    g.DrawFigure(pen, viewPort.ToLines()[3]);

                    foreach (Figure fig in figures)
                    {
                        //We already checked if the figures have atleast something inside the view.
                        Figure[] figs = Figure.Clipping(viewPort, fig);

                        foreach (Figure figc in figs)
                            g.DrawFigure(pen, fig);
                        //fig.Draw(g, pen, brush);
                    }
                }
            }
        }

        public enum DrawStyles
        {
            Points,
            Lines,
            NonInterpolatedLine,
            DiscreteSingal,
            State,
        }

        [Flags]
        public enum DrawOptions
        {
            None = 0,
            ShowCrosses = (1 << 0),
            ExtendBegin = (1 << 1),
            ExtendEnd = (1 << 2),
            ShowScale = (1 << 3),
        }
    }

 
    public class SampleTrace : Trace
    {
        public override ThreadedBindingList<V2D> Points { get; } = new ThreadedBindingList<V2D>();
        public override double GetYValue(double x)
        {
            int i;

            if (Points.Count == 0)
                return 0;

            for (i = 0; i < Points.Count; i++)
            {
                if (Points[i].X > x)
                    break;
            }


            switch (DrawStyle)
            {
                case DrawStyles.DiscreteSingal:
                case DrawStyles.Points:
                    if ((Points[i].X - x) > (Points[i - 1].X - x))
                        i -= 1;
                    // i = closest to x
                    if (i == -1)
                        i = 0;
                    if (i >= Points.Count)
                        i = Points.Count - 1;
                    return Points[i].Y;

                case DrawStyles.Lines:
                    if (i > 0 & i < Points.Count)
                    {
                        double x1 = Points[i - 1].X;
                        double x2 = Points[i].X;
                        double y1 = Points[i - 1].Y;
                        double y2 = Points[i].Y;
                        double a = (y2 - y1) / (x2 - x1);
                        double b = y2 - a * x2;
                        double y = a * x + b;
                        return y;
                    }
                    else
                        return double.NaN;
                case DrawStyles.NonInterpolatedLine:
                    if (i > 0 && (i < (Points.Count + (DrawOption.HasFlag(DrawOptions.ExtendEnd) ? 1 : 0))))
                        return Points[i - 1].Y;
                    else
                        return double.NaN;

                case DrawStyles.State:
                    if (i > 0 && (i < (Points.Count + (DrawOption.HasFlag(DrawOptions.ExtendEnd) ? 1 : 0))))
                        return Points[i - 1].Y;
                    else
                        return double.NaN;

                default:
                    throw new NotImplementedException($"Not yet implemented GetYValue of drawstyle '{DrawStyle}'");

            }
        }



    }


}
