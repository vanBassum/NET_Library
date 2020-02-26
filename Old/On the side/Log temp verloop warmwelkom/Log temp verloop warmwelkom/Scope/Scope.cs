using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using MathNet.Numerics;

namespace Oscilloscope
{
    public partial class ScopeControl : UserControl
    {
        private int thiswidth;
        private int thisheight;
        private int hoverMarkerLine = -1;
        private int dragMarkerLine = -1;
        private Point lastClick = Point.Empty;
        private ContextMenuStrip menu;
        private PictureBox picBox;
        public MarkerlineControl MarkerLineView { get; set; }
        public MathControl MathView { get; set; }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ScopeClass Scope { get; set; } = new ScopeClass();

        public ScopeControl()
        {
            InitializeComponent();
            MarkerLineView = new MarkerlineControl();
            MathView = new MathControl(this);
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = Scope.Traces;

            
            picBox = new PictureBox();
            picBox.Dock = DockStyle.Fill;
            splitContainer1.Panel1.Controls.Add(picBox);
            

            dataGridView1.Columns[0].DataPropertyName = "Colour";
            dataGridView1.Columns[1].DataPropertyName = "Visible";
            dataGridView1.Columns[2].DataPropertyName = "Name";
            dataGridView1.Columns[3].DataPropertyName = "Unit";
            dataGridView1.Columns[4].DataPropertyName = "Scale";
            dataGridView1.Columns[5].DataPropertyName = "Position";

            picBox.MouseClick += picBox_MouseClick;
            picBox.MouseMove += picBox_MouseMove;
            picBox.MouseDown += picBox_MouseDown;
            picBox.MouseUp += picBox_MouseUp;

            picBox.Paint += PicBox_Paint;
            picBox.Resize += PicBox_Resize;

            menu = new ContextMenuStrip();

            ToolStripMenuItem item = new ToolStripMenuItem("CopyToClipboard");
            item.Click += CopyToClipboard_Click;
            menu.Items.Add(item);

            item = new ToolStripMenuItem("Autoscale");
            item.Click += Autoscale_Click;
            menu.Items.Add(item);

            item = new ToolStripMenuItem("AddMarker");
            item.Click += AddMarker_Click;
            menu.Items.Add(item);

            item = new ToolStripMenuItem("Zoom");
            item.Click += Zoom_Click;
            menu.Items.Add(item);

            Scope.MarkerLines.ListChanged += MarkerLines_ListChanged;

        }
        private void Zoom_Click(object sender, EventArgs e)
        {
            if (Scope.MarkerLines.Count == 0)
                return;

            double x = (lastClick.X * Scope.Settings.TimeBase / thiswidth) - Scope.Settings.Offset;

            int left = -1;
            int right = -1;

            for (int i = 0; i < Scope.MarkerLines.Count; i++)
            {
                if(Scope.MarkerLines[i].X < x)
                {
                    if (left == -1)
                        left = i;
                    else
                    {
                        if (Scope.MarkerLines[i].X > Scope.MarkerLines[left].X)
                            left = i;
                    }
                }

                if (Scope.MarkerLines[i].X > x)
                {
                    if (right == -1)
                        right = i;
                    else
                    {
                        if (Scope.MarkerLines[i].X < Scope.MarkerLines[right].X)
                            right = i;
                    }
                }
            }

            double x1 = 0;
            double x2 = 0;

            if(left != -1)
            {
                x1 = Scope.MarkerLines[left].X;
            }
            else
            {
                x1 = (from trace in Scope.Traces
                      from pt in trace.Points
                      orderby pt.X ascending
                      select pt.X).FirstOrDefault();
            }

            if (right != -1)
            {
                x2 = Scope.MarkerLines[right].X;
            }
            else
            {
                x2 = (from trace in Scope.Traces
                      from pt in trace.Points
                      orderby pt.X descending
                      select pt.X).FirstOrDefault();
            }

            Scope.Settings.Offset = -x1;
            Scope.Settings.TimeBase = x2 - x1;
            Draw();                              
        }

        private void AddMarker_Click(object sender, EventArgs e)
        {
            Scope.AddMarkerLine(DateTime.Now);
            dragMarkerLine = Scope.MarkerLines.Count - 1;
        }

        private void MarkerLines_ListChanged(object sender, ListChangedEventArgs e)
        {
            MarkerLineView.DoTheThing(Scope.MarkerLines);
        }

        private void PicBox_Resize(object sender, EventArgs e)
        {
            Draw();
        }

        private void picBox_MouseUp(object sender, MouseEventArgs e)
        {
            dragMarkerLine = -1;
        }

        private void picBox_MouseDown(object sender, MouseEventArgs e)
        {
            
            dragMarkerLine = hoverMarkerLine;
        }

        private void picBox_MouseClick(object sender, MouseEventArgs e)
        {
            lastClick = e.Location;
            if (e.Button == MouseButtons.Right)
            {
                menu.Show(this, e.Location);
            }

        }
        private void Autoscale_Click(object sender, EventArgs e)
        {
            Scope.AutoScale();
            Draw();
        }

        private void CopyToClipboard_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(thiswidth, thisheight);
            this.DrawToBitmap(b, new Rectangle(0, 0, b.Width, b.Height));
            Clipboard.SetImage(b);
        }


        private void picBox_MouseMove(object sender, MouseEventArgs e)
        {
            bool refresh = false;
            
            if(dragMarkerLine != -1)
            {
                double x = (e.X * Scope.Settings.TimeBase / thiswidth) - Scope.Settings.Offset;
                Scope.MarkerLines[dragMarkerLine].X = x;
                MarkerLineView.DoTheThing(Scope.MarkerLines);
                picBox.Refresh();
            }
            else
            {
                double xMin = ((e.X - 2) * Scope.Settings.TimeBase / thiswidth) - Scope.Settings.Offset;
                double x = (e.X * Scope.Settings.TimeBase / thiswidth) - Scope.Settings.Offset;
                double xMax = ((e.X + 2) * Scope.Settings.TimeBase / thiswidth) - Scope.Settings.Offset;


                Cursor cur = Cursors.Default;
                hoverMarkerLine = -1;
                for (int i=0; i< Scope.MarkerLines.Count; i++)
                {
                    if (Scope.MarkerLines[i].X > xMin && Scope.MarkerLines[i].X < xMax)
                    {
                        cur = Cursors.VSplit;
                        hoverMarkerLine = i;
                    }
                }
                Cursor.Current = cur;
            }

            if(refresh)
                this.Refresh();
        }

       

        public void Draw()
        {
            picBox.BackgroundImage = new Bitmap(this.Width, this.Height);

            using (Graphics g = Graphics.FromImage(picBox.BackgroundImage))
            {
                g.Clear(Scope.Settings.BackgroundColor);

                int columns = Scope.Settings.Grid.HorizontalDivisions;
                int hPxPerSub = splitContainer1.Panel1.Width / columns;
                thiswidth = (int)(columns * hPxPerSub);

                int rows = Scope.Settings.Grid.VerticalDivisions;
                int vPxPerSub = splitContainer1.Panel1.Height / rows;
                thisheight = (int)(rows * vPxPerSub);


                //Draw the horizontal lines
                for (int row = 1; row < rows + 1; row++)
                {
                    int y = (int)(row * vPxPerSub);
                    if (row % Scope.Settings.Grid.VerticalSubdiv == 0)
                        g.DrawLine(new Pen(Scope.Settings.Grid.GridColor), 0, y, thiswidth, y);
                    else
                        g.DrawLine(new Pen(Scope.Settings.Grid.GridSubColor), 0, y, thiswidth, y);
                }

                //Draw the vertical lines
                for (int i = 0; i < columns + 1; i++)
                {
                    int x = (int)(i * hPxPerSub);
                    if (i % Scope.Settings.Grid.HorizontalSubdiv == 0)
                        g.DrawLine(new Pen(Scope.Settings.Grid.GridColor), x, 0, x, thisheight);
                    else
                        g.DrawLine(new Pen(Scope.Settings.Grid.GridSubColor), x, 0, x, thisheight);

                }

                double pxPerUnits_hor = thiswidth / (Scope.Settings.TimeBase); // hPxPerSub * grid.Horizontal.SubDivs / (HorUnitsPerDivision /** grid.Horizontal.Divisions*/);

                //Loop through plots
                for (int traceIndex = 0; traceIndex < Scope.Traces.Count; traceIndex++)
                {
                    Trace trace = Scope.Traces[traceIndex];
                    if (trace.Visible)
                    {
                        Pen linePen = new Pen(trace.Colour);
                        double pxPerUnits_ver = thisheight / (Scope.Settings.Grid.VerticalDivisions * trace.Scale);// /** grid.Vertical.Divisions*/);
                                                                                                                   //Draw plot
                        int pointCnt = trace.Points.Count;
                        int inc = pointCnt / thiswidth;
                        if (inc < 1)
                            inc = 1;

                        double xPrev = double.NaN;
                        double yPrev = double.NaN;

                        for (int i = 0; i < pointCnt; i += inc)
                        {
                            double x = (trace.Points[i].X + Scope.Settings.Offset) * pxPerUnits_hor;
                            double y = splitContainer1.Panel1.Height / 2 - trace.Points[i].Y * pxPerUnits_ver - trace.Position * trace.Scale * pxPerUnits_ver;

                            try
                            {
                                if(trace.DrawStyle.HasFlag(Trace.TraceDrawStyle.Points))
                                    g.DrawCross(new Pen(trace.Colour), x, y, 3);

                                if (trace.DrawStyle.HasFlag(Trace.TraceDrawStyle.Lines))
                                {
                                    if(trace.DrawStyle.HasFlag(Trace.TraceDrawStyle.NoInterpolation))
                                    {
                                        if (!double.IsNaN(xPrev))
                                        {
                                            if (trace.DrawStyle.HasFlag(Trace.TraceDrawStyle.ExtendBegin) || i > 1)
                                            {
                                                g.DrawLine(linePen, (int)xPrev, (int)yPrev, (int)x, (int)yPrev);
                                                g.DrawLine(linePen, (int)x, (int)yPrev, (int)x, (int)y);
                                            }     
                                        }
                                    }
                                    else
                                    {
                                        if (!double.IsNaN(xPrev))
                                            g.DrawLine(linePen, (int)xPrev, (int)yPrev, (int)x, (int)y);
                                    }
                                    
                                    if (trace.DrawStyle.HasFlag(Trace.TraceDrawStyle.ExtendBegin))
                                    {
                                        if (i == 0)
                                            g.DrawLine(linePen, 0, (int)y, (int)x, (int)y);
                                    }

                                    if (trace.DrawStyle.HasFlag(Trace.TraceDrawStyle.ExtendEnd))
                                    {
                                        if (i == pointCnt-1)
                                            g.DrawLine(linePen, (int)x, (int)y, thiswidth, (int)y);
                                    }
                                    
                                    xPrev = x;
                                    yPrev = y;
                                }
                            }
                            catch
                            {

                            }

                        }
                        linePen.Dispose();
                    }
                }

                /*
                if(selectionRectangle.Height > 0 && selectionRectangle.Width > 0)
                {
                    g.DrawRectangle(Pens.LightYellow, selectionRectangle);
                }
                */
            }


        }

        private void PicBox_Paint(object sender, PaintEventArgs e)
        {
            double pxPerUnits_hor = thiswidth / (Scope.Settings.TimeBase); // hPxPerSub * grid.Horizontal.SubDivs / (HorUnitsPerDivision /** grid.Horizontal.Divisions*/);

            for (int traceIndex = 0; traceIndex < Scope.Traces.Count; traceIndex++)
            {
                Trace trace = Scope.Traces[traceIndex];
                if (trace.Visible)
                {
                    double pxPerUnits_ver = thisheight / (Scope.Settings.Grid.VerticalDivisions * trace.Scale);// /** grid.Vertical.Divisions*/);
                                                                                                               
                    try
                    {
                        for (int markInd = 0; markInd < trace.Marks.Count; markInd++)
                        {
                            double markY = trace.Marks[markInd].AttachYtoTrace ? trace.GetYValue(trace.Marks[markInd].X) : trace.Marks[markInd].Y;
                            double x = (trace.Marks[markInd].X + Scope.Settings.Offset) * pxPerUnits_hor;
                            double y = splitContainer1.Panel1.Height / 2 - markY * pxPerUnits_ver - trace.Position * trace.Scale * pxPerUnits_ver;
                            string str = trace.Marks[markInd].Text;//ToHumanReadable(markY, 3) + trace.Unit + "\r\n" + ToHumanReadable(trace.Marks[markInd].X, 3, Scope.Settings.HorizontalIsDate) + Scope.Settings.HorizontalUnit;
                            e.Graphics.DrawCross(new Pen(trace.Marks[markInd].Colour), x, y, 3, str, DefaultFont);
                        }
                        /*
                        if (newMarkPlotInd == traceIndex)
                        {
                            double x = (newMark.X + Scope.Settings.Offset) * pxPerUnits_hor;
                            double y = splitContainer1.Panel1.Height / 2 - newMark.Y * pxPerUnits_ver - trace.Position * trace.Scale * pxPerUnits_ver;
                            string str = ToHumanReadable(newMark.Y, 3) + trace.Unit + "\r\n" + ToHumanReadable(newMark.X, 3, Scope.Settings.HorizontalIsDate) + Scope.Settings.HorizontalUnit;
                            e.Graphics.DrawCross(new Pen(trace.Colour), x, y, str, DefaultFont);
                        }
                        */
                    }
                    catch { }
                }
            }

            
            for (int markInd = 0; markInd < Scope.MarkerLines.Count; markInd++)
            {
                double x = (Scope.MarkerLines[markInd].X + Scope.Settings.Offset) * pxPerUnits_hor;
                Pen p = new Pen(Color.Gray);
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                e.Graphics.DrawLine(p, (float)x, (float)0, (float)x, (float)thisheight);
                e.Graphics.DrawString(Scope.MarkerLines[markInd].ID.ToString(), DefaultFont, new SolidBrush(Color.Gray), (float)x, (float)0);
            }

            for(int i=0; i<MathView.MathItems.Count; i++)
            {
                MathItem mi = MathView.MathItems[i];

                if (mi.CalculationType == CalculationType.Linear_regression)
                {
                    if (mi.Marker1 != null && mi.Marker2 != null && mi.Marker1 != mi.Marker2)
                    {
                        //Do linear regression
                        var points = Scope.GetDataBetweenMarkers(mi.Marker1, mi.Marker2, mi.Trace);

                        if(points.Count() > 2)
                        {
                            double[] xdata = (from pt in points
                                              select pt.X).ToArray();

                            double[] ydata = (from pt in points
                                              select pt.Y).ToArray();


                            Tuple<double, double> p = Fit.Line(xdata, ydata);
                            double a = p.Item1; // == 10; intercept
                            double b = p.Item2; // == 0.5; slope

                            mi.SetResult(a, b);

                            PointD p1 = new PointD(mi.Marker1.X, a + b * mi.Marker1.X);
                            PointD p2 = new PointD(mi.Marker2.X, a + b * mi.Marker2.X);


                            double pxPerUnits_ver = thisheight / (Scope.Settings.Grid.VerticalDivisions * mi.Trace.Scale);

                            double x1 = (p1.X + Scope.Settings.Offset) * pxPerUnits_hor;
                            double y1 = splitContainer1.Panel1.Height / 2 - p1.Y * pxPerUnits_ver - mi.Trace.Position * mi.Trace.Scale * pxPerUnits_ver;
                            double x2 = (p2.X + Scope.Settings.Offset) * pxPerUnits_hor;
                            double y2 = splitContainer1.Panel1.Height / 2 - p2.Y * pxPerUnits_ver - mi.Trace.Position * mi.Trace.Scale * pxPerUnits_ver;

                            e.Graphics.DrawLine(new Pen(mi.Colour), (int)x1, (int)y1, (int)x2, (int)y2);
                        }

                        
                    }
                }
            }
        }


        public static string ToHumanReadable(double number, int digits = 3, bool isDate = false)
        {
            string smallPrefix = "mµnpf";
            string largePrefix = "kMGT";

            if(isDate)
            {
                DateTime dt = new DateTime((long)number, DateTimeKind.Local);
                return dt.ToString("dd-MM-yyyy") + " \r\n" + dt.ToString("HH:mm:ss");
            }


            int thousands = (int)Math.Log(Math.Abs(number), 1000);

            if (Math.Log(Math.Abs(number), 1000) < 0)
                thousands--;

            if (number == 0)
                thousands = 0;

            double scaledNumber = number * Math.Pow(1000, -thousands);

            int places = Math.Max(0, digits - (int)Math.Log10(scaledNumber));
            string s = scaledNumber.ToString("F" + places.ToString());



            if (thousands > 0)
                if (thousands < largePrefix.Length)
                    s += largePrefix[thousands - 1];

            if (thousands < 0)
                if (Math.Abs(thousands) < largePrefix.Length)
                    s += smallPrefix[Math.Abs(thousands) - 1];
            return s;
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if(e.ColumnIndex == 4 || e.ColumnIndex == 5)
            {
                double a;
                if(!double.TryParse(e.FormattedValue.ToString(), out a))
                {
                    dataGridView1.EditingControl.BackColor = Color.Red;
                    e.Cancel = true;
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.CellStyle.BackColor = (Color)e.Value;
                e.CellStyle.ForeColor = (Color)e.Value;
            }

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewCell c in dataGridView1.SelectedCells)
            {
                if (c.ReadOnly)
                    c.Selected = false;
            }
        }

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            this.Refresh();
        }
    }

    public static class GraphicsExt
    {
        public static void DrawCross(this Graphics g, Pen p, double x, double y, int size)
        {
            g.DrawLine(p, (float)x - size, (float)y - size, (float)x + size, (float)y + size);
            g.DrawLine(p, (float)x + size, (float)y - size, (float)x - size, (float)y + size);
        }

        public static void DrawCross(this Graphics g, Pen p, double x, double y, int size, string text, Font font)
        {
            g.DrawLine(p, (float)x - size, (float)y - size, (float)x + size, (float)y + size);
            g.DrawLine(p, (float)x + size, (float)y - size, (float)x - size, (float)y + size);
            g.DrawString(text, font, new SolidBrush(p.Color), (float)x, (float)y - 10);
        }
    }
}
