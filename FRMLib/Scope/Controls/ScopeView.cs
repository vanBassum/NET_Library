using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FRMLib.Scope.Controls
{
    public partial class ScopeView : UserControl
    {

        public ScopeViewSettings Settings { get; set; } = new ScopeViewSettings();
        public ScopeController DataSource { get; set; }

        int columns;
        int hPxPerSub;
        int thiswidth;
        int rows;
        int vPxPerSub;
        int thisheight;

        public ScopeView()
        {
            InitializeComponent();
            DrawAll();
        }



        private void ScopeView_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                var parent = this.Parent;
                while (!(parent is Form)) parent = parent.Parent;
                var form = parent as Form;
                form.ResizeEnd += Form_ResizeEnd;
            }

            Settings.PropertyChanged += (a, b) => this.InvokeIfRequired(() => DrawBackground());
            DrawBackground();
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
            this.BackgroundImage = new Bitmap(this.Width, this.Height);
            using (Graphics g = Graphics.FromImage(this.BackgroundImage))
            {
                g.Clear(Settings.BackgroundColor);

                //Draw the horizontal lines
                for (int row = 1; row < rows + 1; row++)
                {
                    int y = (int)(row * vPxPerSub);
                    if (row % (Settings.VerticalDivisions / Settings.VerticalDivisions.LowestDiv()) == 0)
                        g.DrawLine(Settings.GridPen, 0, y, thiswidth, y);
                    else
                        g.DrawLine(Settings.GridSubPen, 0, y, thiswidth, y);
                }

                //Draw the vertical lines
                for (int i = 0; i < columns + 1; i++)
                {
                    int x = (int)(i * hPxPerSub);
                    if (i % (Settings.HorizontalDivisions / Settings.HorizontalDivisions.LowestDiv()) == 0)
                        g.DrawLine(Settings.GridPen, x, 0, x, thisheight);
                    else
                        g.DrawLine(Settings.GridSubPen, x, 0, x, thisheight);

                }
            }
        }

        private void DrawData()
        {
            pictureBox1.BackgroundImage = new Bitmap(this.Width, this.Height);
            using (Graphics g = Graphics.FromImage(pictureBox1.BackgroundImage))
            {
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

                                    p = new Point((int)x, (int)y);


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
                                            }
                                            break;

                                        default:
                                            g.DrawString($"Drawing of '{trace.DrawStyle}' is not supported yet.", Settings.Font, brush, new Point(0, traceNo * Settings.Font.Height));
                                            i = pointCnt;
                                            break;

                                    }

                                    pPrev = p;
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
        }

        private void DrawForeground()
        {
            if (DataSource != null)
            {

            }
        }

        #endregion
    }
}
