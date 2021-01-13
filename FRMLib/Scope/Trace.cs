using FRMLib.Scope.Controls;
using STDLib.Math;
using STDLib.Misc;
using STDLib.Extentions;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace FRMLib.Scope
{
    public class Trace : PropertySensitive
    {
        [TraceViewAttribute(Text = "", Width = 20)]
        public Pen Pen { get { return GetPar(Pens.Red); } set { SetPar(value); } }

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
        public ThreadedBindingList<PointD> Points { get; } = new ThreadedBindingList<PointD>();
        //public ThreadedBindingList<Mark> Marks { get; } = new ThreadedBindingList<Mark>();
        [TraceViewAttribute(Width = 80)]
        public DrawStyles DrawStyle { get { return GetPar(DrawStyles.Lines); } set { SetPar(value); } }
        [TraceViewAttribute]
        public DrawOptions DrawOption { get { return GetPar(DrawOptions.ShowScale); } set { SetPar(value); } }
        public Func<double, string> ToHumanReadable { get { return GetPar(new Func<double, string>((a) => a.ToHumanReadable(3) + Unit)); } set { SetPar(value); } }
        public PointD Minimum { get { return GetPar(PointD.Empty); } set { SetPar(value); } }
        public PointD Maximum { get { return GetPar(PointD.Empty); } set { SetPar(value); } }
        public Trace Self { get { return this; } }
        public Trace()
        {
            Points.ListChanged += Points_ListChanged;
        }

        private void Points_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case System.ComponentModel.ListChangedType.ItemAdded:
                    DoMinMax(Points[e.NewIndex]);
                    break;

                default:
                    //TODO: This has a serious potential to be optimized!
                    Minimum = PointD.Empty;
                    Maximum = PointD.Empty;
                    foreach (PointD pt in Points)
                        DoMinMax(pt);
                    break;
            }
        }

        void DoMinMax(PointD pt)
        {
            Minimum.KeepMinimum(pt);
            Maximum.KeepMaximum(pt);
        }


        public IEnumerable<PointD> GetPointsBetweenMarkers(Cursor marker1, Cursor marker2)
        {
            double x1 = 0;
            double x2 = 0;

            if (marker1.X < marker2.X)
            {
                x1 = marker1.X;
                x2 = marker2.X;
            }
            else
            {
                x1 = marker2.X;
                x2 = marker1.X;
            }


            bool startFound = false;
            bool endFound = false;
            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].X >= x1)
                    startFound = true;

                if (Points[i].X > x2)
                    endFound = true;

                if (startFound == true && endFound == false)
                    yield return Points[i];

            }
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


        public void Draw(Graphics g, Rectangle viewPort, Func<PointD, Point> convert, double firstX, double lastX, double xLeft, double xRight, Font font)
        {
            Pen pen = Pen;
            Brush brush = new SolidBrush(pen.Color);

            if (Visible)
            {
                //Pen linePen = new Pen(trace.Colour);
                double stateY = convert(new PointD(0, 1)).Y;// viewPort.Height / 2 - Offset * pxPerUnits_ver;// * trace.Scale;

                int pointCnt = Points.Count;
                int inc = 1;
                //Only draw points within screen, this can be calculated.

                for (int i = 0; i < pointCnt; i += inc)
                {
                    bool last = (i == (pointCnt - inc));
                    bool first = i == 0;

                    bool extendEnd = DrawOption.HasFlag(Trace.DrawOptions.ExtendEnd);
                    bool extendBegin = DrawOption.HasFlag(Trace.DrawOptions.ExtendBegin);

                    PointD pPrevD = PointD.Empty;
                    PointD pActD = Points[i];
                    PointD pNextD = PointD.Empty;

                    if (!first)
                        pPrevD = Points[i - inc];
                    if (first && extendBegin)
                        pPrevD = new PointD(firstX, Points[i].Y);
                    if (!last)
                        pNextD = Points[i + inc];
                    if (last && extendEnd)
                        pNextD = new PointD(lastX, Points[i].Y);

                    Point pPrev = convert(pPrevD);
                    Point pAct = convert(pActD);
                    Point pNext = convert(pNextD);

                    //Outside view check
                    if (viewPort.CheckIfPointIsWithin(pAct) || viewPort.CheckIfPointIsWithin(pPrev) || viewPort.CheckIfPointIsWithin(pNext))
                    {
                        if(pActD.X < xLeft)
                            pAct = convert(new PointD(xLeft, GetYValue(xLeft)));
                        else if (pActD.X > xRight)
                            pAct = convert(new PointD(xRight, GetYValue(xRight)));

                        if (pPrevD.X < xLeft)
                            pPrev = convert(new PointD(xLeft, GetYValue(xLeft)));

                        if (pNextD.X > xRight)
                            pNext = convert(new PointD(xRight, GetYValue(xRight)));

                        if (DrawOption.HasFlag(Trace.DrawOptions.ShowCrosses) && viewPort.CheckIfPointIsWithin(convert(pActD)))
                            g.DrawCross(pen, convert(pActD), 3);

                        switch (DrawStyle)
                        {
                            case Trace.DrawStyles.Points:
                                g.Drawpoint(brush, pAct, 2);
                                break;

                            case Trace.DrawStyles.DiscreteSingal:
                                g.Drawpoint(brush, pAct, 4);
                                g.DrawLine(pen, new Point(pAct.X, viewPort.Height / 2), pAct);
                                break;

                            case Trace.DrawStyles.Lines:

                                if (first && extendBegin)
                                    g.DrawLine(pen, pPrev, pAct, true, false);

                                if (last && extendEnd)
                                    g.DrawLine(pen, pAct, pNext, false, true);

                                if (!last)
                                    g.DrawLine(pen, pAct, pNext, false, false);

                                break;

                            case Trace.DrawStyles.NonInterpolatedLine:
                                if (first && extendBegin)
                                    g.DrawLine(pen, pPrev, pAct, true, false);

                                if (last && extendEnd)
                                    g.DrawLine(pen, pAct, pNext, false, true);

                                if (!last)
                                {
                                    g.DrawLine(pen, pAct, new Point(pNext.X, pAct.Y));
                                    g.DrawLine(pen, new Point(pNext.X, pAct.Y), pNext);
                                }
                                break;

                            case Trace.DrawStyles.State:
                                string text = ToHumanReadable(Points[i].Y);

                                //1, 2, 3

                                Rectangle rect = Rectangle.Empty;
                                //throw new NotImplementedException("Implement stateY");
                                if (first && extendBegin)
                                    rect = new Rectangle((int)pPrev.X, (int)stateY - 8, pAct.X - pPrev.X, font.Height);
                                
                                if (last && extendEnd)
                                    rect = new Rectangle((int)pAct.X, (int)stateY - 8, pNext.X - pAct.X, font.Height);
                                
                                if (!last)
                                    rect = new Rectangle((int)pAct.X, (int)stateY - 8, pNext.X - pAct.X, font.Height);
                                
                                if (rect != Rectangle.Empty)
                                    g.DrawState(pen, rect, text, font, !(first && extendBegin), !(last && extendEnd));

                                break;

                            default:
                                throw new NotImplementedException($"Drawing of '{DrawStyle}' is not supported yet.");
                        }
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


}
