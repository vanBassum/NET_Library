using STDLib.Math;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FRMLib.Scope.Controls
{
    public partial class ScopeView : UserControl
    {
        public ScopeViewSettings Settings { get; set; } = new ScopeViewSettings();
        private ScopeController dataSource;
        public ScopeController DataSource
        {
            get { return dataSource; }
            set
            {
                dataSource = value;
                if (dataSource != null)
                {
                    dataSource.Traces.ListChanged += Traces_ListChanged;
                    dataSource.Cursors.ListChanged += Markers_ListChanged;
                }
            }
        }


        private ContextMenuStrip menu;
        private Point lastClick = Point.Empty;
        private Point lastClickDown = Point.Empty;
        private double horOffsetLastClick = 0;
        private Cursor dragMarker = null;
        private Cursor hoverMarker = null;
        int columns;
        int hPxPerSub;
        int thiswidth;
        int rows;
        int vPxPerSub;
        int thisheight;


        PictureBox pictureBox1 = new PictureBox();
        PictureBox pictureBox2 = new PictureBox();
        PictureBox pictureBox3 = new PictureBox();

        public ScopeView()
        {
            InitializeComponent();
            DrawAll();

            this.Controls.Add(pictureBox1);
            pictureBox1.Controls.Add(pictureBox2);
            pictureBox2.Controls.Add(pictureBox3);

            pictureBox1.Dock = DockStyle.Fill;
            pictureBox2.Dock = DockStyle.Fill;
            pictureBox3.Dock = DockStyle.Fill;


            pictureBox1.BackColor = Color.Transparent;
            pictureBox2.BackColor = Color.Transparent;
            pictureBox3.BackColor = Color.Transparent;

            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox2.Paint += PictureBox2_Paint;
            pictureBox3.Paint += PictureBox3_Paint;

            pictureBox1.BringToFront();
            pictureBox2.BringToFront();
            pictureBox3.BringToFront();

            menu = new ContextMenuStrip();

            ToolStripMenuItem item;

            item = new ToolStripMenuItem("Add marker");
            item.Click += AddMarker_Click;
            menu.Items.Add(item);

            item = new ToolStripMenuItem("Zoom");
            item.Click += Zoom_Click;
            menu.Items.Add(item);

            item = new ToolStripMenuItem("Autoscale hor");
            item.Click += AutoscaleHor_Click;
            menu.Items.Add(item);

            item = new ToolStripMenuItem("Clear");
            item.Click += Clear_Click;
            menu.Items.Add(item);
        }

        private void AutoscaleHor_Click(object sender, EventArgs e)
        {
            FitHorizontalInXDivs(Settings.HorizontalDivisions);
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            dataSource.Clear();
        }

        private void AddMarker_Click(object sender, EventArgs e)
        {
            dataSource.Cursors.Add(dragMarker = new Cursor() { X = -Settings.HorOffset });
        }

        private void Zoom_Click(object sender, EventArgs e)
        {
            if (DataSource.Cursors.Count == 0)
                return;

            Cursor left = null;
            Cursor right = null;

            GetMarkersAdjecentToX(lastClick.X, ref left, ref right);

            double x1 = 0;
            double x2 = 0;

            if (left != null)
                x1 = left.X;
            else
            {
                x1 = (from trace in DataSource.Traces
                      from pt in trace.Points
                      orderby pt.X ascending
                      select pt.X).FirstOrDefault();
            }

            if (right != null)
                x2 = right.X;
            else
            {
                x2 = (from trace in DataSource.Traces
                      from pt in trace.Points
                      orderby pt.X descending
                      select pt.X).FirstOrDefault();
            }

            Settings.HorOffset = -x1;
            Settings.HorScale = (x2 - x1) / Settings.HorizontalDivisions;
            DrawAll();
        }

        void GetMarkersAdjecentToX(double xPos, ref Cursor left, ref Cursor right)
        {
            //double pxPerUnits_hor = thiswidth / (Settings.HorizontalDivisions * Settings.HorScale);
            //double x = (float)(trace.Points[i].X + Settings.HorOffset) * pxPerUnits_hor;



            double x = (xPos * Settings.HorScale * Settings.HorizontalDivisions / thiswidth) - Settings.HorOffset;

            int iLeft = -1;
            int iRight = -1;

            for (int i = 0; i < DataSource.Cursors.Count; i++)
            {
                if (DataSource.Cursors[i].X < x)
                {
                    if (iLeft == -1)
                        iLeft = i;
                    else
                    {
                        if (DataSource.Cursors[i].X > DataSource.Cursors[iLeft].X)
                            iLeft = i;
                    }
                }

                if (DataSource.Cursors[i].X > x)
                {
                    if (iRight == -1)
                        iRight = i;
                    else
                    {
                        if (DataSource.Cursors[i].X < DataSource.Cursors[iRight].X)
                            iRight = i;
                    }
                }
            }

            if (iLeft == -1)
                left = null;
            else
                left = DataSource.Cursors[iLeft];

            if (iRight == -1)
                right = null;
            else
                right = DataSource.Cursors[iRight];
        }

        private void ScopeView_Load(object sender, EventArgs e)
        {
            this.Resize += Form_ResizeEnd;
            Settings.PropertyChanged += (a, b) => this.InvokeIfRequired(() => DrawBackground());
            DrawAll();


            pictureBox3.MouseClick += picBox_MouseClick;
            pictureBox3.MouseMove += picBox_MouseMove;
            pictureBox3.MouseDown += picBox_MouseDown;
            pictureBox3.MouseUp += picBox_MouseUp;
            pictureBox3.MouseWheel += PictureBox3_MouseWheel;

            //pictureBox1.Resize += PictureBox1_Resize;
        }

        private void PictureBox3_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {

                double scroll = (double)(e.Delta);
                double A = thiswidth / (Settings.HorizontalDivisions * Settings.HorScale);
                double B = Settings.HorOffset;
                double percent = (double)e.X / (double)thiswidth;   //Relative mouse position.
                double x1px = percent * scroll;
                double x2px = thiswidth - (1 - percent) * scroll;

                //Find the actual value of x1 and x2
                double x1 = x1px / A - B;
                double x2 = x2px / A - B;
                double distance = x2 - x1;
                if (distance == 0)
                {
                    Settings.HorScale = 1;
                    Settings.HorOffset = -x1;
                    return;
                }
                Settings.NotifyOnChange = false;
                Settings.HorScale = (double)distance / (double)Settings.HorizontalDivisions;
                Settings.NotifyOnChange = true;
                Settings.HorOffset = -(double)(x1);
               
            }
        }

        private void picBox_MouseUp(object sender, MouseEventArgs e)
        {
            dragMarker = null;
            lastClickDown = Point.Empty;
        }

        private void picBox_MouseDown(object sender, MouseEventArgs e)
        {
            dragMarker = hoverMarker;
            lastClickDown = e.Location;
            horOffsetLastClick = Settings.HorOffset;
        }

        private void picBox_MouseClick(object sender, MouseEventArgs e)
        {
            lastClick = e.Location;
            if (e.Button == MouseButtons.Right)
            {
                if (dragMarker != null)
                    dragMarker = null;
                else
                    menu.Show(this, e.Location);
            }
        }

        private void picBox_MouseMove(object sender, MouseEventArgs e)
        {

            double pxPerUnits_hor = thiswidth / (Settings.HorizontalDivisions * Settings.HorScale);
            if (dragMarker != null)
            {
                //Drag a marker.
                double x = (e.X / pxPerUnits_hor) - Settings.HorOffset;
                dragMarker.X = x;
                DrawForeground();
            }
            else
            {
                if (e.Button.HasFlag(MouseButtons.Left))
                {
                    //Drag all.
                    if (!lastClickDown.IsEmpty)
                    {
                        double xDif = e.X - lastClickDown.X;
                        double A = thiswidth / (Settings.HorizontalDivisions * Settings.HorScale);
                        Settings.HorOffset = xDif / A + horOffsetLastClick;
                    }
                }
                else
                {
                    //Detect markers.
                    double xMin = ((e.X - 4) / pxPerUnits_hor) - Settings.HorOffset;
                    double xMax = ((e.X + 4) / pxPerUnits_hor) - Settings.HorOffset;

                    if (DataSource != null)
                    {

                        System.Windows.Forms.Cursor cur = System.Windows.Forms.Cursors.Default;
                        hoverMarker = null;
                        for (int i = 0; i < DataSource.Cursors.Count; i++)
                        {
                            if (DataSource.Cursors[i].X > xMin && DataSource.Cursors[i].X < xMax)
                            {
                                cur = Cursors.VSplit;
                                hoverMarker = DataSource.Cursors[i];
                            }
                        }
                        System.Windows.Forms.Cursor.Current = cur;
                    }
                }
            }
        }



        private void Markers_ListChanged(object sender, ListChangedEventArgs e)
        {
            DrawForeground();
        }

        private void Traces_ListChanged(object sender, ListChangedEventArgs e)
        {
            DrawData();
        }

        private void Form_ResizeEnd(object sender, EventArgs e)
        {
            DrawAll();
        }


        #region Calculations

        private void CalculateScopeSize()
        {
            columns = Settings.HorizontalDivisions;
            hPxPerSub = this.Width / columns;
            thiswidth = (int)(columns * hPxPerSub);
            rows = Settings.VerticalDivisions;
            vPxPerSub = this.Height / rows;
            thisheight = (int)(rows * vPxPerSub);
        }


        public void AutoScaleTrace(Trace t)
        {
            if (double.IsNaN(t.Maximum.Y) || double.IsNaN(t.Maximum.X))
            {
                t.Scale = 1f;
                t.Offset = 0f;
                return;
            }

            double distance = t.Maximum.Y - t.Minimum.Y;
            double div = distance / ((double)Settings.VerticalDivisions);
            double multiplier = 1f;

            while (div > 10)
            {
                multiplier *= 10;
                div /= 10;
            }

            while (div < 0.5)
            {
                multiplier /= 10;
                div *= 10;
            }


            if (div <= 1)
                t.Scale = (double)(1 * multiplier);
            else if (div <= 2)
                t.Scale = (double)(2 * multiplier);
            else if (div <= 5)
                t.Scale = (double)(5 * multiplier);
            else
                t.Scale = (double)(10 * multiplier);

            t.Offset = -(double)(distance / 2 + t.Minimum.Y);
        }

        public void AutoScaleHorizontal()
        {
            PointD min = PointD.Empty;
            PointD max = PointD.Empty;

            foreach (Trace t in DataSource.Traces)
            {
                min.KeepMinimum(t.Minimum);
                max.KeepMaximum(t.Maximum);
            }

            double distance = max.X - min.X;
            if (distance == 0)
            {
                Settings.HorScale = 1;
                Settings.HorOffset = -min.X;
                return;
            }

            double div = distance / ((double)Settings.HorizontalDivisions);
            double multiplier = 1f;

            while (div > 10)
            {
                multiplier *= 10;
                div /= 10;
            }

            while (div < 0.5)
            {
                multiplier /= 10;
                div *= 10;
            }


            if (div <= 1)
                Settings.HorScale = (double)(1 * multiplier);
            else if (div <= 2)
                Settings.HorScale = (double)(2 * multiplier);
            else if (div <= 5)
                Settings.HorScale = (double)(5 * multiplier);
            else
                Settings.HorScale = (double)(10 * multiplier);

            Settings.HorOffset = -(double)(min.X);
        }

        public void FitHorizontalInXDivs(int divs)
        {
            PointD min = PointD.Empty;
            PointD max = PointD.Empty;

            foreach (Trace t in DataSource.Traces)
            {
                min.KeepMinimum(t.Minimum);
                max.KeepMaximum(t.Maximum);
            }

            double distance = max.X - min.X;
            if (distance == 0)
            {
                Settings.HorScale = 1;
                Settings.HorOffset = -min.X;
                return;
            }

            Settings.HorScale = (double)distance / (double)divs;
            Settings.HorOffset = -(double)(min.X);
        }



        #endregion






        #region Drawing

        public void DrawAll()
        {
            CalculateScopeSize();
            DrawBackground();
            DrawData();
            DrawForeground();
        }



        private void DrawBackground()
        {
            pictureBox1.Refresh();
        }

        //Draw the background
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Settings.BackgroundColor);

            //Draw the horizontal lines
            for (int row = 1; row < rows + 0; row++)
            {
                int y = (int)(row * vPxPerSub);
                if (row % (Settings.VerticalDivisions / Settings.VerticalDivisions.LowestDiv()) == 0)
                    g.DrawLine(Settings.GridPen, 0, y, thiswidth, y);
                else
                    g.DrawLine(Settings.GridSubPen, 0, y, thiswidth, y);
            }

            //Draw the vertical lines
            for (int i = 1; i < columns + 0; i++)
            {
                int x = (int)(i * hPxPerSub);
                if (i % (Settings.HorizontalDivisions / Settings.HorizontalDivisions.LowestDiv()) == 0)
                    g.DrawLine(Settings.GridPen, x, 0, x, thisheight);
                else
                    g.DrawLine(Settings.GridSubPen, x, 0, x, thisheight);

            }
        }



        private void DrawData()
        {
            pictureBox2.Refresh();
        }


        private void PictureBox2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (DataSource == null)
            {
                g.DrawString("No datasource bound", DefaultFont, Brushes.White, new Point(this.Width / 2 - 50, this.Height / 2));
            }
            else
            {
                double pxPerUnits_hor = thiswidth / (Settings.HorizontalDivisions * Settings.HorScale); // hPxPerSub * grid.Horizontal.SubDivs / (HorUnitsPerDivision /** grid.Horizontal.Divisions*/);


                var sortedTraces = from trace in DataSource.Traces
                                   orderby trace.Layer descending
                                   select trace;


                double lastX = double.NegativeInfinity;
                double firstX = double.PositiveInfinity;
                foreach (Trace t in DataSource.Traces)
                {
                    PointD pt = t.Points.LastOrDefault();
                    if (pt != null)
                    {
                        if (pt.X > lastX)
                            lastX = pt.X;
                    }

                    pt = t.Points.FirstOrDefault();
                    if (pt != null)
                    {
                        if (pt.X < firstX)
                            firstX = pt.X;
                    }
                }

                int errNo = 0;
                Brush errBrush = new SolidBrush(Color.Red);

                //Loop through plots
                foreach (Trace trace in sortedTraces)  // (int traceIndex = 0; traceIndex < Scope.Traces.Count; traceIndex++)
                {
                    Pen pen = trace.Pen;
                    Brush brush = new SolidBrush(pen.Color);

                    //Trace trace = Scope.Traces[traceIndex];
                    if (trace.Visible)
                    {
                        //Pen linePen = new Pen(trace.Colour);
                        double pxPerUnits_ver = thisheight / (Settings.VerticalDivisions * trace.Scale);// /** grid.Vertical.Divisions*/);
                        double stateY = thisheight / 2 - trace.Offset * pxPerUnits_ver;// * trace.Scale;

                        int pointCnt = trace.Points.Count;
                        int inc = pointCnt / thiswidth;     //TODO get points between left and right pos in screen. When this is done inc = 1 can be removed.
                        inc = 1;
                        if (inc < 1)
                            inc = 1;

                                                

                        Func<PointD, Point> convert = (p) => new Point((int)((p.X + Settings.HorOffset) * pxPerUnits_hor), (int)(thisheight / 2 - (p.Y + trace.Offset) * pxPerUnits_ver));
                        try
                        {
                            for (int i = 0; i < pointCnt; i += inc)
                            {
                                bool last = (i == (pointCnt - inc));
                                bool first = i == 0;

                                bool extendEnd = trace.DrawOption.HasFlag(Trace.DrawOptions.ExtendEnd);
                                bool extendBegin = trace.DrawOption.HasFlag(Trace.DrawOptions.ExtendBegin);


                                Point pPrev = Point.Empty;
                                Point pAct = convert(trace.Points[i]);
                                Point pNext = Point.Empty;

                               

                                if (!first)
                                    pPrev = convert(trace.Points[i - inc]);

                                if (first && extendBegin)
                                    pPrev = convert(new PointD(firstX, trace.Points[i].Y));

                                if (!last)
                                    pNext = convert(trace.Points[i + inc]);

                                if (last && extendEnd)
                                    pNext = convert(new PointD(lastX, trace.Points[i].Y));


                                //Outside view check
                                if (CheckWithinScreen(pAct) || CheckWithinScreen(pPrev) || CheckWithinScreen(pNext))
                                {

                                    if (trace.DrawOption.HasFlag(Trace.DrawOptions.ShowCrosses))
                                        g.DrawCross(pen, pAct, 3);


                                    switch (trace.DrawStyle)
                                    {
                                        case Trace.DrawStyles.Points:
                                            g.Drawpoint(brush, pAct, 2);
                                            break;

                                        case Trace.DrawStyles.DiscreteSingal:
                                            g.Drawpoint(brush, pAct, 4);
                                            g.DrawLine(pen, new Point(pAct.X, thisheight / 2), pAct);
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
                                            string text = trace.ToHumanReadable(trace.Points[i].Y);

                                            //1, 2, 3

                                            Rectangle rect = Rectangle.Empty;

                                            if (first && extendBegin)
                                                rect = new Rectangle((int)pPrev.X, (int)stateY - 8, pAct.X - pPrev.X, Settings.Font.Height);

                                            if (last && extendEnd)
                                                rect = new Rectangle((int)pAct.X, (int)stateY - 8, pNext.X - pAct.X, Settings.Font.Height);

                                            if (!last)
                                                rect = new Rectangle((int)pAct.X, (int)stateY - 8, pNext.X - pAct.X, Settings.Font.Height);

                                            if (rect != Rectangle.Empty)
                                                g.DrawState(pen, rect, text, Settings.Font, !(first && extendBegin), !(last && extendEnd));

                                            break;

                                        default:
                                            g.DrawString($"Drawing of '{trace.DrawStyle}' is not supported yet.", Settings.Font, errBrush, new Point(0, (errNo++) * Settings.Font.Height + 1));
                                            i = pointCnt;
                                            break;

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            g.DrawString(ex.Message, Settings.Font, errBrush, new Point(0, (errNo++) * Settings.Font.Height));
                        }
                    }
                }

                try
                {
                    foreach (Marker marker in dataSource.Markers)
                    {

                        double pxPerUnits_ver = thisheight / (Settings.VerticalDivisions * marker.Scale);


                        double x = (float)(marker.Point.X + Settings.HorOffset) * pxPerUnits_hor;
                        double y = thisheight / 2 - (marker.Point.Y + marker.Offset) * pxPerUnits_ver;// * trace.Scale;

                        if (CheckWithinScreen(x, y))
                        {
                            Brush brush = new SolidBrush(marker.Pen.Color);
                            g.DrawString(marker.Text, Settings.Font, brush, (int)x, (int)y);
                            g.DrawCross(marker.Pen, x, y, 3);
                        }
                    }
                }
                catch (Exception ex)
                {
                    g.DrawString(ex.Message, Settings.Font, errBrush, new Point(0, (errNo++) * Settings.Font.Height));
                }
            }
        }

        bool CheckWithinScreen(Point pt)
        {
            return CheckWithinScreen(pt.X, pt.Y);
        }

        bool CheckWithinScreen(double x, double y)
        {
            return (x > 0 && x < thiswidth && y > 0 && y < thisheight);
        }


        private void DrawForeground()
        {
            pictureBox3.Refresh();
        }

        private void PictureBox3_Paint(object sender, PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            if (DataSource != null)
            {
                double pxPerUnits_hor = thiswidth / (Settings.HorizontalDivisions * Settings.HorScale); // hPxPerSub * grid.Horizontal.SubDivs / (HorUnitsPerDivision /** grid.Horizontal.Divisions*/);

                int markerNo = 0;
                //Loop through markers
                foreach (Cursor marker in DataSource.Cursors)  // (int traceIndex = 0; traceIndex < Scope.Traces.Count; traceIndex++)
                {
                    Pen pen = marker.Pen;
                    Brush brush = new SolidBrush(pen.Color);

                    try
                    {
                        float x = (float)((marker.X + Settings.HorOffset) * pxPerUnits_hor);

                        g.DrawLine(pen, x, 0, x, thisheight);
                        g.DrawString(marker.ID.ToString(), Settings.Font, brush, new PointF(x, 0));

                    }

                    catch (Exception ex)
                    {
                        g.DrawString(ex.Message, Settings.Font, brush, new Point(0, markerNo * Settings.Font.Height));
                    }
                    markerNo++;
                }

                Func<double, int> scaleX = (x) => (int)((x + Settings.HorOffset) * pxPerUnits_hor);

                //Loop trought mathitems

                foreach (MathItem mathItem in DataSource.MathItems)  // (int traceIndex = 0; traceIndex < Scope.Traces.Count; traceIndex++)
                {
                    try
                    {
                        if (mathItem.Trace != null)
                        {
                            double pxPerUnits_ver = thisheight / (Settings.VerticalDivisions * mathItem.Trace.Scale);
                            Func<double, int> scaleY = (x) => (int)(thisheight / 2 - (x + mathItem.Trace.Offset) * pxPerUnits_ver);

                            mathItem.Draw(g, scaleY, scaleX);
                        }

                    }
                    catch (Exception ex)
                    {
                        g.DrawString(ex.Message, Settings.Font, Brushes.White, new Point(0, markerNo * Settings.Font.Height));
                    }
                }
            }
        }

        #endregion
    }
}
