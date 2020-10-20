using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using STDLib.Math;

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
                    dataSource.Markers.ListChanged += Markers_ListChanged;
                }
            }
        }


        private ContextMenuStrip menu;
        private Point lastClick = Point.Empty;
        private Point lastClickDown = Point.Empty;
        private double horOffsetLastClick = 0;
        private Marker dragMarker = null;
        private Marker hoverMarker = null;
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

            item = new ToolStripMenuItem("ClearData");
            item.Click += ClearData_Click;
            menu.Items.Add(item);


        }

        private void AutoscaleHor_Click(object sender, EventArgs e)
        {
            FitHorizontalInXDivs(Settings.HorizontalDivisions);
        }

        private void ClearData_Click(object sender, EventArgs e)
        {
            foreach (Trace t in dataSource.Traces)
                t.Points.Clear();
        }

        private void AddMarker_Click(object sender, EventArgs e)
        {
            dataSource.Markers.Add(dragMarker = new Marker() { X = -Settings.HorOffset });
        }

        private void Zoom_Click(object sender, EventArgs e)
        {
            if (DataSource.Markers.Count == 0)
                return;

            Marker left = null;
            Marker right = null;

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

        void GetMarkersAdjecentToX(double xPos, ref Marker left, ref Marker right)
        {
            //double pxPerUnits_hor = thiswidth / (Settings.HorizontalDivisions * Settings.HorScale);
            //double x = (float)(trace.Points[i].X + Settings.HorOffset) * pxPerUnits_hor;



            double x = (xPos * Settings.HorScale * Settings.HorizontalDivisions / thiswidth) - Settings.HorOffset;

            int iLeft = -1;
            int iRight = -1;

            for (int i = 0; i < DataSource.Markers.Count; i++)
            {
                if (DataSource.Markers[i].X < x)
                {
                    if (iLeft == -1)
                        iLeft = i;
                    else
                    {
                        if (DataSource.Markers[i].X > DataSource.Markers[iLeft].X)
                            iLeft = i;
                    }
                }

                if (DataSource.Markers[i].X > x)
                {
                    if (iRight == -1)
                        iRight = i;
                    else
                    {
                        if (DataSource.Markers[i].X < DataSource.Markers[iRight].X)
                            iRight = i;
                    }
                }
            }

            if (iLeft == -1)
                left = null;
            else
                left = DataSource.Markers[iLeft];

            if (iRight == -1)
                right = null;
            else
                right = DataSource.Markers[iRight];
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
            if(e.Delta != 0)
            {

                double scroll = (double)(e.Delta);
                double A = thiswidth / (Settings.HorizontalDivisions * Settings.HorScale);
                double B = Settings.HorOffset;
                double percent = (double)e.X / (double)thiswidth;   //Relative mouse position.
                double x1px = percent * scroll;
                double x2px = thiswidth - (1-percent) * scroll;

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

                Settings.HorScale = (double)distance / (double)Settings.HorizontalDivisions;
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
                if(e.Button.HasFlag(MouseButtons.Left))
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

                        Cursor cur = Cursors.Default;
                        hoverMarker = null;
                        for (int i = 0; i < DataSource.Markers.Count; i++)
                        {
                            if (DataSource.Markers[i].X > xMin && DataSource.Markers[i].X < xMax)
                            {
                                cur = Cursors.VSplit;
                                hoverMarker = DataSource.Markers[i];
                            }
                        }
                        Cursor.Current = cur;
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
            if(distance == 0)
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
                    
                lastX = (float)(lastX + Settings.HorOffset) * pxPerUnits_hor;
                firstX = (float)(firstX + Settings.HorOffset) * pxPerUnits_hor;
                
                int traceNo = 0;
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
                                                                                                        //Draw plot
                        int pointCnt = trace.Points.Count;
                        int inc = pointCnt / thiswidth;
                        if (inc < 1)
                            inc = 1;

                        try
                        {
                            Point p = Point.Empty;
                            Point pPrev = Point.Empty;



                            for (int i = 0; i < pointCnt; i += inc)
                            {
                                double x = (float)(trace.Points[i].X + Settings.HorOffset) * pxPerUnits_hor;
                                double y = thisheight / 2 - (trace.Points[i].Y + trace.Offset) * pxPerUnits_ver;// * trace.Scale;
                                double stateY = thisheight / 2 - trace.Offset * pxPerUnits_ver;// * trace.Scale;
                                p = new Point((int)x, (int)y);

                                bool last = (i == (pointCnt - 1));
                                bool first = i == 0;
                                bool extendEnd = trace.DrawOption.HasFlag(Trace.DrawOptions.ExtendEnd);
                                bool extendBegin = trace.DrawOption.HasFlag(Trace.DrawOptions.ExtendBegin);

                                if (trace.DrawOption.HasFlag(Trace.DrawOptions.ShowCrosses))
                                    g.DrawCross(pen, p, 3);


                                switch (trace.DrawStyle)
                                {
                                    case Trace.DrawStyles.Points:
                                        g.Drawpoint(brush, p, 2);
                                        break;

                                    case Trace.DrawStyles.DiscreteSingal:
                                        g.Drawpoint(brush, p, 4);
                                        g.DrawLine(pen, new Point(p.X, thisheight / 2), p);
                                        break;

                                    case Trace.DrawStyles.Lines:
                                        if (!pPrev.IsEmpty)
                                            g.DrawLine(pen, p, pPrev);
                                        break;

                                    case Trace.DrawStyles.NonInterpolatedLine:
                                        if (!pPrev.IsEmpty)
                                        {
                                            Point between = new Point(p.X, pPrev.Y);
                                            g.DrawLine(pen, pPrev, between);
                                            g.DrawLine(pen, between, p);
                                            if (last && extendEnd)
                                                g.DrawLine(pen, p, new Point((int)lastX, p.Y));

                                        }
                                        break;

                                    case Trace.DrawStyles.State:
                                        string text = trace.ToHumanReadable(trace.Points[i].Y);

                                        //1, 2, 3


                                        double start = x;
                                        double end = x;

                                        if (!last)
                                            end = (trace.Points[i + 1].X + Settings.HorOffset) * pxPerUnits_hor;

                                        if (first && extendBegin)
                                        {
                                            start = firstX;
                                            double strlen = g.MeasureString(text, Settings.Font).Width + 5;
                                            if (end - start < strlen)
                                                start = end - strlen;
                                        }

                                        if (last && extendEnd)
                                        {
                                            end = lastX;
                                            double strlen = g.MeasureString(text, Settings.Font).Width + 5;
                                            if (end - start < strlen)
                                                end = start + strlen;
                                        }

                                        Rectangle rect = new Rectangle((int)start, (int)stateY - 8, (int)(end - start), Settings.Font.Height);
                                        g.DrawState(pen, rect, text, Settings.Font, !(first && extendBegin), !(last && extendEnd));

                                        break;

                                    default:
                                        g.DrawString($"Drawing of '{trace.DrawStyle}' is not supported yet.", Settings.Font, brush, new Point(0, traceNo * Settings.Font.Height + 1));
                                        i = pointCnt;
                                        break;

                                }

                                pPrev = p;
                            }

                            foreach (Mark m in trace.Marks)
                            {
                                double x = (float)(m.X + Settings.HorOffset) * pxPerUnits_hor;
                                double y = thisheight / 2 - (m.Y + trace.Offset) * pxPerUnits_ver;// * trace.Scale;

                                g.DrawString(m.Text, Settings.Font, brush, (int)x, (int)y);
                                g.DrawCross(pen, x, y, 3);
                            }
                        }
                        catch (Exception ex)
                        {
                            g.DrawString(ex.Message, Settings.Font, brush, new Point(0, traceNo * Settings.Font.Height));
                        }


                        
                    }

                    traceNo++;
                }
            }
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
                foreach (Marker marker in DataSource.Markers)  // (int traceIndex = 0; traceIndex < Scope.Traces.Count; traceIndex++)
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
                        if(mathItem.Trace != null)
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
