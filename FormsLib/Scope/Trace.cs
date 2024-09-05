using FormsLib.Scope.Controls;
using CoreLib.Misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using FormsLib.Design;
using FormsLib.Misc;
using FormsLib.Extentions;
using FormsLib.Maths;
using System.Security.Cryptography.X509Certificates;

namespace FormsLib.Scope
{
    public class Trace : PropertySensitive
    {
        [TraceViewAttribute(Width = 20, HideValue = true)]
        public Color Color { get { return GetPar(Colors.Green); } set { SetPar(value); } }
        [TraceViewAttribute(Width = 20, HeaderText = "")]
        public bool Visible { get { return GetPar(true); } set { SetPar(value); } }
        [TraceViewAttribute(Width = 50)]
        public string Name { get { return GetPar("New trace"); } set { SetPar(value); } }

        //[TraceViewAttribute]
        public string Unit { get { return GetPar(""); } set { SetPar(value); } }
        [TraceViewAttribute(Width = 40)]
        public double Scale { get { return GetPar<double>(1f); } set { SetPar<double>(value); } }
        [TraceViewAttribute(Width = 40)]
        public double Offset { get { return GetPar<double>(0f); } set { SetPar<double>(value); } }
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
        public object Tag { get; set; }
        public Trace()
        {
            Points.ListChanged += Points_ListChanged;
        }

        public void AddPointSort(PointD pt)
        {
            int i = 0;
            for(i=0; i< Points.Count; i++)
            {
                if (pt.X < Points[i].X)
                {
                    Points.Insert(i, pt);
                    return;
                }
            }
            Points.Add(pt);
        }

        public override string ToString()
        {
            return Name;
        }

        public void AddPoint(DateTime dt, double value) => Points.Add(dt.Ticks, value);
        public void AddPoint(double x, double value) => Points.Add(x, value);


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


        public IEnumerable<PointD> GetPointsBetweenMarkers(Marker marker1, Marker marker2)
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

        private int BinarySearchPoint(ThreadedBindingList<PointD> points, double x)
        {
            int left = 0;
            int right = points.Count - 1;

            while (left <= right)
            {
                int mid = (left + right) / 2;

                if (points[mid].X == x)
                    return mid; // Return exact match
                else if (points[mid].X < x)
                    left = mid + 1;
                else
                    right = mid - 1;
            }

            // Return the first index where points[i].X > x, or points.Count if x is beyond the last point
            return left;
        }


        public double GetYValue(double x)
        {
            if (Points.Count == 0)
                return 0;

            // Use binary search to find the closest point
            int i = BinarySearchPoint(Points, x);

            if (i < 0) i = ~i; // Get the insertion point if x is not found exactly

            // If we're beyond the last point, check if we can extend
            bool canExtendEnd = DrawOption.HasFlag(DrawOptions.ExtendEnd);

            switch (DrawStyle)
            {
                case DrawStyles.Lines:
                    if (i > 0 && i < Points.Count)
                    {
                        double x1 = Points[i - 1].X;
                        double x2 = Points[i].X;
                        double y1 = Points[i - 1].Y;
                        double y2 = Points[i].Y;

                        // Linear interpolation formula
                        double a = (y2 - y1) / (x2 - x1);
                        return a * (x - x1) + y1;
                    }
                    break;

                case DrawStyles.DiscreteSingal:
                case DrawStyles.Points:
                case DrawStyles.NonInterpolatedLine:
                case DrawStyles.State:
                    if (i > 0 && (i < Points.Count || (i == Points.Count && canExtendEnd)))
                        return Points[i - 1].Y;
                    break;

                default:
                    throw new NotImplementedException($"Not yet implemented GetYValue for drawstyle '{DrawStyle}'");
            }

            return double.NaN;
        }

        public void Draw(Graphics g, Style style, Rectangle viewPort, Func<PointD, Point> convert, double firstX, double lastX, double xLeft, double xRight)
        {
            if (!Visible || Points.Count == 0) return;

            // Cache Pen and Brush to avoid recreating them
            using (Pen pen = new Pen(Color))
            using (Brush brush = new SolidBrush(Color))
            {
                double stateY = convert(new PointD(0, 1)).Y;
                int pointCnt = Points.Count;

                // Cache flags to avoid repeated calls to HasFlag
                bool extendEnd = DrawOption.HasFlag(Trace.DrawOptions.ExtendEnd);
                bool extendBegin = DrawOption.HasFlag(Trace.DrawOptions.ExtendBegin);
                bool showCrosses = DrawOption.HasFlag(Trace.DrawOptions.ShowCrosses);

                for (int i = 0; i < pointCnt; i++)
                {
                    bool last = (i == pointCnt - 1);
                    bool first = (i == 0);

                    PointD pPrevD = first && extendBegin ? new PointD(firstX, Points[i].Y) : (i > 0 ? Points[i - 1] : PointD.Empty);
                    PointD pActD = Points[i];
                    PointD pNextD = last && extendEnd ? new PointD(lastX, Points[i].Y) : (i < pointCnt - 1 ? Points[i + 1] : PointD.Empty);

                    Point pPrev = convert(pPrevD);
                    Point pAct = convert(pActD);
                    Point pNext = convert(pNextD);

                    bool across = pAct.X <= viewPort.X && pNext.X >= (viewPort.X + viewPort.Width);

                    // Check if the points are within the viewport or across it
                    if (viewPort.CheckIfPointIsWithin(pAct) || viewPort.CheckIfPointIsWithin(pPrev) || viewPort.CheckIfPointIsWithin(pNext) || across)
                    {
                        // Show crosses if necessary
                        if (showCrosses && viewPort.CheckIfPointIsWithin(convert(pActD)))
                        {
                            g.DrawCross(pen, viewPort, pAct, 3);
                        }

                        // Handle drawing styles
                        switch (DrawStyle)
                        {
                            case Trace.DrawStyles.Points:
                                g.Drawpoint(brush, pAct, 2);
                                break;

                            case Trace.DrawStyles.Cross:
                                g.DrawCross(pen, viewPort, pAct, 5);
                                break;

                            case Trace.DrawStyles.DiscreteSingal:
                                g.Drawpoint(brush, pAct, 4);
                                g.DrawLine(pen, new Point(pAct.X, viewPort.Height / 2), pAct);
                                break;

                            case Trace.DrawStyles.Lines:
                                if (first && extendBegin) g.DrawLine(pen, viewPort, pPrev, pAct, true, false);
                                if (last && extendEnd) g.DrawLine(pen, viewPort, pAct, pNext, false, true);
                                if (!last) g.DrawLine(pen, viewPort, pAct, pNext, false, false);
                                break;

                            case Trace.DrawStyles.NonInterpolatedLine:
                                if (first && extendBegin) g.DrawLine(pen, viewPort, pPrev, pAct, true, false);
                                if (last && extendEnd) g.DrawLine(pen, viewPort, pAct, pNext, false, true);
                                if (!last)
                                {
                                    Point pInbetween = new Point(pNext.X, pAct.Y);
                                    g.DrawLine(pen, viewPort, pAct, pInbetween, false, false);
                                    g.DrawLine(pen, viewPort, pInbetween, pNext, false, false);
                                }
                                break;

                            case Trace.DrawStyles.State:
                                string text = ToHumanReadable(pActD.Y);
                                int rectHeight = style.Font.Height;

                                Rectangle rect = first && extendBegin
                                    ? new Rectangle(pPrev.X, (int)stateY - 8, pAct.X - pPrev.X, rectHeight)
                                    : last && extendEnd
                                        ? new Rectangle(pAct.X, (int)stateY - 8, pNext.X - pAct.X, rectHeight)
                                        : new Rectangle(pAct.X, (int)stateY - 8, pNext.X - pAct.X, rectHeight);

                                if (!rect.IsEmpty)
                                    g.DrawState(pen, viewPort, rect, text, style.Font, !(first && extendBegin), !(last && extendEnd));
                                break;

                            default:
                                throw new NotImplementedException($"Drawing style '{DrawStyle}' not supported.");
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
            Cross,
        }

        [Flags]
        public enum DrawOptions
        {
            None = 0,
            ShowCrosses =   0x01,
            ExtendBegin =   0x02,
            ExtendEnd =     0x04,
            ShowScale =     0x08,
            DrawNames =     0x10,
        }


    }


}
