using FormsLib.Extentions;
using FormsLib.Maths;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace FormsLib.Scope.Controls
{
    public partial class ScopeView : UserControl
    {
        //public ScopeViewSettings Settings { get; set; } = new ScopeViewSettings();
        private ScopeController dataSource = new ScopeController();
        public ScopeController DataSource
        {
            get { return dataSource; }
            set
            {
                dataSource = value;
                if (dataSource != null)
                {
                    dataSource.Settings.PropertyChanged += (a, b) => this.InvokeIfRequired(() => DrawBackground());
                    //dataSource.Traces.ListChanged += Traces_ListChanged;
                    dataSource.Markers.ListChanged += Markers_ListChanged;
                    dataSource.DoRedraw += (a, b) => DrawAll();
                }
            }
        }

        private ContextMenuStrip menu;
        private ContextMenuStrip cursorMenu;
        private Point lastClick = Point.Empty;
        private Point lastClickDown = Point.Empty;
        private double horOffsetLastClick = 0;
        private Marker dragMarker = null;
        private Marker hoverMarker = null;
        private Point mousePos = Point.Empty;
        Rectangle viewPort = new Rectangle(0, 0, 0, 0);
        //int pxPerColumn;
        //int pxPerRow;
        int zeroPos;    //Position in px that represents vertical zero


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

            KeyUp += (s, e) => DrawAll();

            pictureBox1.BringToFront();
            pictureBox2.BringToFront();
            pictureBox3.BringToFront();

            menu = new ContextMenuStrip();

            AddMenuItem(menu, "Add marker", () => dataSource.Markers.Add(dragMarker = new Marker() { X = -dataSource.Settings.HorOffset }));
            AddMenuItem(menu, "Zoom", () => Zoom_Click());
            AddMenuItem(menu, "Horizontal scale/Draw/None", () => dataSource.Settings.DrawScalePosHorizontal = DrawPosHorizontal.None);
            AddMenuItem(menu, "Horizontal scale/Draw/Top", () => dataSource.Settings.DrawScalePosHorizontal = DrawPosHorizontal.Top);
            AddMenuItem(menu, "Horizontal scale/Draw/Bottom", () => dataSource.Settings.DrawScalePosHorizontal = DrawPosHorizontal.Bottom);
            AddMenuItem(menu, "Horizontal scale/Fit", () => FitHorizontalInXDivs(dataSource.Settings.HorizontalDivisions));
            AddMenuItem(menu, "Horizontal scale/Day", () => AutoScaleHorizontalTime(TimeSpan.FromDays(1)));
            AddMenuItem(menu, "Horizontal scale/Hour", () => AutoScaleHorizontalTime(TimeSpan.FromHours(1)));
            AddMenuItem(menu, "Vertical scale/Draw/None", () => dataSource.Settings.DrawScalePosVertical = DrawPosVertical.None);
            AddMenuItem(menu, "Vertical scale/Draw/Left", () => dataSource.Settings.DrawScalePosVertical = DrawPosVertical.Left);
            AddMenuItem(menu, "Vertical scale/Draw/Right", () => dataSource.Settings.DrawScalePosVertical = DrawPosVertical.Right);
            AddMenuItem(menu, "Vertical scale/Zero position/Top", () => dataSource.Settings.ZeroPosition = VerticalZeroPosition.Top);
            AddMenuItem(menu, "Vertical scale/Zero position/Middle", () => dataSource.Settings.ZeroPosition = VerticalZeroPosition.Middle);
            AddMenuItem(menu, "Vertical scale/Zero position/Bottom", () => dataSource.Settings.ZeroPosition = VerticalZeroPosition.Bottom);
            AddMenuItem(menu, "Vertical scale/Auto", () => AutoScaleTracesKeepZero());
            AddMenuItem(menu, "Clear", () => dataSource.Clear());
            AddMenuItem(menu, "Screenshot/To clipboard", () => Screenshot_Click(true));
            AddMenuItem(menu, "Screenshot/To file", () => Screenshot_Click(false));


            cursorMenu = new ContextMenuStrip();
            AddMenuItem(cursorMenu, "Remove", () => dataSource.Markers.Remove(hoverMarker));
        }

        void AddMenuItem(ContextMenuStrip menu, string menuPath, Action action)
        {
            string[] split = menuPath.Split('/');

            ToolStripMenuItem item = null;


            if (menu.Items[split[0]] is ToolStripMenuItem tsi)
                item = tsi;
            else
            {
                item = new ToolStripMenuItem(split[0]);
                item.Name = split[0];
                menu.Items.Add(item);
            }

            for (int i = 1; i < split.Length; i++)
            {
                string name = split[i];

                if (item.DropDownItems[name] is ToolStripMenuItem tsii)
                    item = tsii;
                else
                {
                    ToolStripMenuItem newItem = new ToolStripMenuItem(name);
                    newItem.Name = name;
                    item.DropDownItems.Add(newItem);
                    item = newItem;
                }

            }

            if (action != null)
                item.Click += (a, b) => action.Invoke();


        }


        private void Screenshot_Click(bool toClipboard)
        {
            Image img = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            using (Graphics g = Graphics.FromImage(img))
            {
                DrawBackground(g);
                DrawData(g);
                DrawForeground(g);
            }

            if (toClipboard)
            {
                Clipboard.SetImage(img);
            }
            else
            {
                SaveFileDialog diag = new SaveFileDialog();
                diag.Filter = "PNG|*.PNG";
                diag.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                diag.FileName = "Untitled.png";
                diag.RestoreDirectory = true;
                if (diag.ShowDialog() == DialogResult.OK)
                {
                    img.Save(diag.FileName);
                }
            }
        }

        private void Zoom_Click()
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

            dataSource.Settings.HorOffset = -x1;
            dataSource.Settings.HorScale = (x2 - x1) / dataSource.Settings.HorizontalDivisions;
            DrawAll();
        }

        void GetMarkersAdjecentToX(double xPos, ref Marker left, ref Marker right)
        {
            double pxPerUnits_hor = viewPort.Width / (dataSource.Settings.HorizontalDivisions * dataSource.Settings.HorScale);
            double x = (xPos * dataSource.Settings.HorScale * dataSource.Settings.HorizontalDivisions / viewPort.Height) - dataSource.Settings.HorOffset + viewPort.X;

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
                double A = viewPort.Width / (dataSource.Settings.HorizontalDivisions * dataSource.Settings.HorScale);
                double B = dataSource.Settings.HorOffset;
                double percent = (double)(e.X - viewPort.X) / (double)viewPort.Width;   //Relative mouse position.
                double x1px = percent * scroll;
                double x2px = viewPort.Width - (1 - percent) * scroll;

                //Find the actual value of x1 and x2
                double x1 = x1px / A - B;
                double x2 = x2px / A - B;
                double distance = x2 - x1;
                if (distance == 0)
                {
                    dataSource.Settings.HorScale = 1;
                    dataSource.Settings.HorOffset = -x1;
                    return;
                }
                dataSource.Settings.NotifyOnChange = false;
                dataSource.Settings.HorScale = (double)distance / (double)dataSource.Settings.HorizontalDivisions;
                dataSource.Settings.NotifyOnChange = true;
                dataSource.Settings.HorOffset = -(double)(x1);
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
            horOffsetLastClick = dataSource.Settings.HorOffset;
        }

        private void picBox_MouseClick(object sender, MouseEventArgs e)
        {
            lastClick = e.Location;
            if (e.Button == MouseButtons.Right)
            {
                if (dragMarker != null)
                    dragMarker = null;
                if (hoverMarker != null)
                    cursorMenu.Show(this, e.Location);
                else
                    menu.Show(this, e.Location);
            }
        }

        private void picBox_MouseMove(object sender, MouseEventArgs e)
        {
            mousePos = e.Location;
            double pxPerUnits_hor = viewPort.Width / (dataSource.Settings.HorizontalDivisions * dataSource.Settings.HorScale);
            if (dragMarker != null)
            {
                //Drag a marker.
                double x = ((e.X - viewPort.X) / pxPerUnits_hor) - dataSource.Settings.HorOffset;
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
                        double A = viewPort.Width / (dataSource.Settings.HorizontalDivisions * dataSource.Settings.HorScale);
                        double offset = xDif / A + horOffsetLastClick;
                        dataSource.Settings.HorOffset = Math.Round(offset / dataSource.Settings.HorSnapSize) * dataSource.Settings.HorSnapSize;
                    }
                }
                else
                {
                    //Detect cursors.
                    if (DataSource != null)
                    {
                        System.Windows.Forms.Cursor cur = System.Windows.Forms.Cursors.Default;
                        hoverMarker = null;
                        for (int i = 0; i < DataSource.Markers.Count; i++)
                        {
                            Marker cursor = DataSource.Markers[i];
                            int cursorX = (int)((cursor.X + dataSource.Settings.HorOffset) * pxPerUnits_hor) + viewPort.X;
                            int xMin = cursorX - 4;
                            int xMax = cursorX + 4;


                            if (e.X > xMin && e.X < xMax)
                            {
                                cur = Cursors.VSplit;
                                hoverMarker = DataSource.Markers[i];
                            }
                        }
                        System.Windows.Forms.Cursor.Current = cur;
                    }
                }
                if (ModifierKeys.HasFlag(Keys.Control) || dataSource.Settings.Style.AlwaysDetectRadius)
                    DrawForeground();
            }
        }

        #region Calculations

        public void AutoScaleTraces()
        {
            foreach (Trace t in dataSource.Traces)
                AutoScaleTrace(t);
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
            double div = distance / ((double)dataSource.Settings.VerticalDivisions);
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

            t.Offset = -(double)(distance / (dataSource.Settings.ZeroPosition == VerticalZeroPosition.Middle ? 2 : 1) + t.Minimum.Y);
        }

        public void AutoScaleTracesKeepZero()
        {
            foreach (Trace t in dataSource.Traces)
                AutoScaleTraceKeepZero(t);
        }

        public void AutoScaleTraceKeepZero(Trace t)
        {
            if (double.IsNaN(t.Maximum.Y) || double.IsNaN(t.Maximum.X))
            {
                t.Scale = 1f;
                t.Offset = 0f;
                return;
            }

            double distance = Math.Max(Math.Abs(t.Maximum.Y), Math.Abs(t.Minimum.Y));
            double div = distance * (dataSource.Settings.ZeroPosition == VerticalZeroPosition.Middle ? 2 : 1) / ((double)dataSource.Settings.VerticalDivisions);
            double multiplier = 1f;

            if (div == 0)
                return;

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

            t.Offset = 0;
        }

        public void AutoScaleHorizontalTime(TimeSpan scale)
        {
            PointD min = PointD.Empty;
            PointD max = PointD.Empty;
            foreach (Trace t in DataSource.Traces)
            {
                min.KeepMinimum(t.Minimum);
                max.KeepMaximum(t.Maximum);
            }

            dataSource.Settings.HorScale = scale.Ticks;
            dataSource.Settings.HorOffset = -min.X;
        }

        public void SetHorizontalTime(DateTime start, DateTime end)
        {
            TimeSpan scale = end - start;
            dataSource.Settings.HorScale = scale.Ticks / dataSource.Settings.HorizontalDivisions;
            dataSource.Settings.HorOffset = -start.Ticks;
        }

        public void AutoScaleHorizontalTime()
        {
            PointD min = PointD.Empty;
            PointD max = PointD.Empty;

            foreach (Trace t in DataSource.Traces)
            {
                min.KeepMinimum(t.Minimum);
                max.KeepMaximum(t.Maximum);
            }


            DateTime start = new DateTime((long)min.X);
            DateTime end = new DateTime((long)max.X);
            TimeSpan span = end - start;
            if (span.TotalDays >= dataSource.Settings.HorizontalDivisions)
            {
                dataSource.Settings.HorScale = Math.Ceiling(span.TotalDays / dataSource.Settings.HorizontalDivisions) * TimeSpan.TicksPerDay;
                start.AddMilliseconds(-start.Millisecond);
                start.AddSeconds(-start.Second);
                start.AddMinutes(-start.Minute);
                start.AddHours(-start.Hour);
            }
            else if (span.TotalHours >= dataSource.Settings.HorizontalDivisions)
            {
                dataSource.Settings.HorScale = Math.Ceiling(span.TotalHours / dataSource.Settings.HorizontalDivisions) * TimeSpan.TicksPerHour;
                start.AddMilliseconds(-start.Millisecond);
                start.AddSeconds(-start.Second);
                start.AddMinutes(-start.Minute);
            }
            else if (span.TotalMinutes >= dataSource.Settings.HorizontalDivisions)
            {
                dataSource.Settings.HorScale = Math.Ceiling(span.TotalMinutes / dataSource.Settings.HorizontalDivisions) * TimeSpan.TicksPerMinute;
                start.AddMilliseconds(-start.Millisecond);
                start.AddSeconds(-start.Second);
            }
            else if (span.TotalSeconds >= dataSource.Settings.HorizontalDivisions)
            {
                dataSource.Settings.HorScale = Math.Ceiling(span.TotalSeconds / dataSource.Settings.HorizontalDivisions) * TimeSpan.TicksPerSecond;
                start.AddMilliseconds(-start.Millisecond);
                start.AddSeconds(-start.Second);
            }
            else
            {
                dataSource.Settings.HorScale = Math.Ceiling(span.TotalMilliseconds / dataSource.Settings.HorizontalDivisions) * TimeSpan.TicksPerMillisecond;
                start.AddMilliseconds(-start.Millisecond);
            }
            dataSource.Settings.HorOffset = -start.Ticks;
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
                dataSource.Settings.HorScale = 1;
                dataSource.Settings.HorOffset = -min.X;
                return;
            }

            double div = distance / ((double)dataSource.Settings.HorizontalDivisions);
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
                dataSource.Settings.HorScale = (double)(1 * multiplier);
            else if (div <= 2)
                dataSource.Settings.HorScale = (double)(2 * multiplier);
            else if (div <= 5)
                dataSource.Settings.HorScale = (double)(5 * multiplier);
            else
                dataSource.Settings.HorScale = (double)(10 * multiplier);

            dataSource.Settings.HorOffset = -(double)(min.X);
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
                dataSource.Settings.HorScale = 1;
                dataSource.Settings.HorOffset = -min.X;
                return;
            }

            dataSource.Settings.HorScale = (double)distance / (double)divs;
            dataSource.Settings.HorOffset = -(double)(min.X);
        }



        #endregion

        #region Drawing

        private void DrawBackground()
        {
            pictureBox1.InvokeIfRequired(() => pictureBox1.Refresh());
           
        }

        private void DrawBackground(Graphics g)
        {
            var stopwatch = Stopwatch.StartNew();
            viewPort.X = 0;
            viewPort.Y = 0;
            viewPort.Width = pictureBox1.Width - 1;
            viewPort.Height = pictureBox1.Height - 1;

            int spaceForScaleIndicatorsVertical = 80;
            int spaceForScaleIndicatorsHorizontal = 25;

            if (dataSource == null)
                return;

            switch (dataSource.Settings.DrawScalePosVertical)
            {
                case DrawPosVertical.Left:
                    viewPort.X += spaceForScaleIndicatorsVertical;
                    viewPort.Width -= spaceForScaleIndicatorsVertical;
                    break;
                case DrawPosVertical.Right:
                    viewPort.Width -= spaceForScaleIndicatorsVertical;
                    break;
            }

            switch (dataSource.Settings.DrawScalePosHorizontal)
            {
                case DrawPosHorizontal.Bottom:
                    viewPort.Height -= spaceForScaleIndicatorsHorizontal;
                    break;
                case DrawPosHorizontal.Top:
                    viewPort.Y += spaceForScaleIndicatorsHorizontal;
                    viewPort.Height -= spaceForScaleIndicatorsHorizontal;
                    break;
            }


            int columns = dataSource.Settings.HorizontalDivisions;
            int rows = dataSource.Settings.VerticalDivisions;
            int pxPerColumn = viewPort.Width / columns;
            int pxPerRow = viewPort.Height / rows;
            int restWidth = viewPort.Width % columns;
            int restHeight = viewPort.Height % rows;

            spaceForScaleIndicatorsVertical += restWidth / 2;
            spaceForScaleIndicatorsHorizontal += restHeight / 2;

            viewPort.X += restWidth / 2;
            viewPort.Y -= restHeight / 2;
            viewPort.Width = columns * pxPerColumn;
            viewPort.Height = rows * pxPerRow;

            switch (dataSource.Settings.ZeroPosition)
            {
                case VerticalZeroPosition.Middle:
                    zeroPos = viewPort.Y + viewPort.Height / 2;
                    break;
                case VerticalZeroPosition.Top:
                    zeroPos = viewPort.Y;
                    break;
                case VerticalZeroPosition.Bottom:
                    zeroPos = viewPort.Y + viewPort.Height;
                    break;
            }

            g.Clear(dataSource.Settings.Style.BackgroundColor);

            //Draw the viewport
            g.DrawRectangle(dataSource.Settings.Style.GridPen, viewPort);

            //Draw the horizontal lines
            for (int row = 1; row < rows + 0; row++)
            {
                int y = (int)(row * pxPerRow) + viewPort.Y;
                g.DrawLine(dataSource.Settings.Style.GridPen, viewPort.X, y, viewPort.X + viewPort.Width, y);

                if (dataSource != null)
                {
                    if (dataSource.Settings.DrawScalePosVertical != DrawPosVertical.None)
                    {
                        int scaleDrawCount = dataSource.Traces.Where(a => a.DrawOption.HasFlag(Trace.DrawOptions.ShowScale)).Count();
                        int fit = pxPerRow / dataSource.Settings.Style.Font.Height;
                        if (fit > scaleDrawCount)
                            fit = scaleDrawCount;
                        int yy = y - (fit / 2) * dataSource.Settings.Style.Font.Height;
                        if (fit % 2 != 0)
                            yy -= dataSource.Settings.Style.Font.Height / 2;

                        int i = 0;

                        foreach (Trace t in dataSource.Traces)
                        {

                            if(!t.Visible)
                                continue;

                            if (i >= fit)
                                continue;

                            if(t.DrawOption.HasFlag(Trace.DrawOptions.ShowScale))
                            {
                                double yValue = ((dataSource.Settings.VerticalDivisions - row) * t.Scale) - t.Offset;
                                switch (dataSource.Settings.ZeroPosition)
                                {
                                    case VerticalZeroPosition.Middle:
                                        yValue -= (dataSource.Settings.VerticalDivisions / 2) * t.Scale;
                                        break;
                                    case VerticalZeroPosition.Top:
                                        yValue -= (dataSource.Settings.VerticalDivisions) * t.Scale;
                                        break;
                                }

                                Color color = t.Color;
                                Brush b = new SolidBrush(color);
                                int x = dataSource.Settings.DrawScalePosVertical == DrawPosVertical.Left ? 0 : viewPort.X + viewPort.Width;
                                g.DrawString(t.ToHumanReadable(yValue), dataSource.Settings.Style.Font, b, new Rectangle(x, yy, spaceForScaleIndicatorsVertical, dataSource.Settings.Style.Font.Height));
                                yy += dataSource.Settings.Style.Font.Height;
                            }
                            i++;
                        }
                    }
                }
            }

            //Draw the vertical lines
            for (int i = 1; i < columns + 0; i++)
            {
                int x = (int)(i * pxPerColumn) + viewPort.X;
                g.DrawLine(dataSource.Settings.Style.GridPen, x, viewPort.Y, x, viewPort.Y + viewPort.Height);

                if (dataSource != null)
                {
                    if (dataSource.Settings.DrawScalePosHorizontal != DrawPosHorizontal.None)
                    {
                        double pxPerUnits_hor = viewPort.Width / (dataSource.Settings.HorizontalDivisions * dataSource.Settings.HorScale);
                        double xVal = ((x - viewPort.X) / pxPerUnits_hor) - dataSource.Settings.HorOffset;
                        string xString = dataSource.Settings.HorizontalToHumanReadable(xVal);

                        if (xString != null)
                        {
                            Brush b = new SolidBrush(dataSource.Settings.Style.ForegroundColor);
                            int y = dataSource.Settings.DrawScalePosHorizontal == DrawPosHorizontal.Top ? 0 : viewPort.Y + viewPort.Height + 2;
                            StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center };
                            Size textSize = TextRenderer.MeasureText(xString, dataSource.Settings.Style.Font);
                            g.DrawString(xString, dataSource.Settings.Style.Font, b, new Rectangle(x - pxPerColumn / 2, y, pxPerColumn, spaceForScaleIndicatorsHorizontal), sf);
                        }
                    }
                }
            }

            //Draw traceNames
            foreach (Trace t in dataSource.Traces)
            {
                if (!t.Visible)
                    continue;

                if (t.DrawOption.HasFlag(Trace.DrawOptions.DrawNames))
                {
                    double pxPerUnits_ver = viewPort.Height / (dataSource.Settings.VerticalDivisions * t.Scale);

                    Color color = t.Color;
                    Brush b = new SolidBrush(color);
                    int x = dataSource.Settings.DrawScalePosVertical == DrawPosVertical.Left ? 0 : viewPort.X + viewPort.Width;
                    int y = (int)(zeroPos - (1 + t.Offset) * pxPerUnits_ver) - 8;
                    g.DrawString(t.Name, dataSource.Settings.Style.Font, b, new Rectangle(x, y, spaceForScaleIndicatorsVertical, dataSource.Settings.Style.Font.Height));
                }
            }




            //Draw the zero line
            g.DrawLine(dataSource.Settings.Style.GridZeroPen, viewPort.X, zeroPos, viewPort.X + viewPort.Width, zeroPos);

            stopwatch.Stop();
#if DEBUG
            g.DrawString($"B: {stopwatch.ElapsedMilliseconds.ToString().PadRight(5)} ms", dataSource.Settings.Style.Font, Brushes.White, new Point(viewPort.Width, 2 * dataSource.Settings.Style.Font.Height), new StringFormat() { Alignment = StringAlignment.Far });
#endif
        }

        private void DrawData()
        {

            pictureBox2.InvokeIfRequired(() => pictureBox2.Refresh());
        }

        private void DrawData(Graphics g)
        {
            var stopwatch = Stopwatch.StartNew();
            if (DataSource == null)
            {
                g.DrawString("No datasource bound", DefaultFont, Brushes.White, new Point(this.Width / 2 - 50, this.Height / 2));
            }
            else
            {
                int errNo = 0;
                Brush errBrush = new SolidBrush(Color.Red);
                double pxPerUnits_hor = viewPort.Width / (dataSource.Settings.HorizontalDivisions * dataSource.Settings.HorScale);
                var sortedTraces = from trace in DataSource.Traces
                                   select trace;

                PointD min = PointD.Empty;
                PointD max = PointD.Empty;

                foreach (Trace t in sortedTraces.Where(t => t.Visible))
                {
                    min.KeepMinimum(t.Minimum);
                    max.KeepMaximum(t.Maximum);
                }

                double xLeft = dataSource.Settings.HorOffset;
                double xRight = ((viewPort.Width) / pxPerUnits_hor) - dataSource.Settings.HorOffset;

                //Loop through traces
                foreach (Trace trace in sortedTraces)
                {
                    double pxPerUnits_ver = viewPort.Height / (dataSource.Settings.VerticalDivisions * trace.Scale);

                    Func<PointD, Point> convert = (p) => new Point(
                        (int)((p.X + dataSource.Settings.HorOffset) * pxPerUnits_hor) + viewPort.X,
                        (int)(zeroPos - (p.Y + trace.Offset) * pxPerUnits_ver));
                    try
                    {
                        trace.Draw(g, dataSource.Settings.Style, viewPort, convert, min.X, max.X, xLeft, xRight);
                    }
                    catch (Exception ex)
                    {
                        g.DrawString(ex.Message, dataSource.Settings.Style.Font, errBrush, new Point(0, (errNo++) * dataSource.Settings.Style.Font.Height));
                    }
                }

                try
                {
                    foreach (Label marker in dataSource.Labels)
                    {
                        if (marker is LinkedLabel lm)
                        {
                            if (dataSource.Traces.Contains(lm.Trace))
                            {
                                double pxPerUnits_ver = viewPort.Height / (dataSource.Settings.VerticalDivisions * lm.Trace.Scale);
                                Func<PointD, Point> convert = (p) => new Point(
                                    (int)((p.X + dataSource.Settings.HorOffset) * pxPerUnits_hor) + viewPort.X,
                                    (int)(zeroPos - (p.Y + lm.Trace.Offset) * pxPerUnits_ver));

                                marker.Draw(g, dataSource.Settings.Style, viewPort, convert);
                            }
                        }
                        else if (marker is FreeLabel fm)
                        {
                            double pxPerUnits_ver = viewPort.Height / (dataSource.Settings.VerticalDivisions * 1);
                            Func<PointD, Point> convert = (p) => new Point(
                                (int)((p.X + dataSource.Settings.HorOffset) * pxPerUnits_hor) + viewPort.X,
                                (int)(zeroPos - (p.Y + 0) * pxPerUnits_ver));

                            marker.Draw(g, dataSource.Settings.Style, viewPort, convert);
                        }

                    }
                }
                catch (Exception ex)
                {
                    g.DrawString(ex.Message, dataSource.Settings.Style.Font, errBrush, new Point(0, (errNo++) * dataSource.Settings.Style.Font.Height));
                }

                try
                {
                    foreach (IScopeDrawable drawable in dataSource.Drawables)
                    {
                        Func<PointD, Point> convert = (p) => new Point((int)((p.X + dataSource.Settings.HorOffset) * pxPerUnits_hor), (int)(viewPort.Height / 2 - p.Y));
                        drawable.Draw(g, convert);

                    }
                }
                catch (Exception ex)
                {
                    g.DrawString(ex.Message, dataSource.Settings.Style.Font, errBrush, new Point(0, (errNo++) * dataSource.Settings.Style.Font.Height));
                }
            }

            stopwatch.Stop();
#if DEBUG
            g.DrawString($"D: {stopwatch.ElapsedMilliseconds.ToString().PadRight(5)} ms", dataSource.Settings.Style.Font, Brushes.White, new Point(viewPort.Width, 1 * dataSource.Settings.Style.Font.Height), new StringFormat() { Alignment = StringAlignment.Far });
#endif

        }

        private void DrawForeground()
        {
            pictureBox3.InvokeIfRequired(() => pictureBox3.Refresh());
        }
        void DrawForeground(Graphics g)
        {
            var stopwatch = Stopwatch.StartNew();
            if (DataSource != null)
            {
                double pxPerUnits_hor = viewPort.Width / (dataSource.Settings.HorizontalDivisions * dataSource.Settings.HorScale); // hPxPerSub * grid.Horizontal.SubDivs / (HorUnitsPerDivision /** grid.Horizontal.Divisions*/);

                int cursorNo = 0;
                //Loop through markers
                foreach (Marker cursor in DataSource.Markers)  // (int traceIndex = 0; traceIndex < Scope.Traces.Count; traceIndex++)
                {
                    Pen pen = cursor.Pen;
                    Brush brush = new SolidBrush(pen.Color);
                
                    try
                    {
                        float x = (float)((cursor.X + dataSource.Settings.HorOffset) * pxPerUnits_hor) + viewPort.X;
                        g.DrawLine(pen, x, viewPort.Y, x, viewPort.Y + viewPort.Height);
                        if(!string.IsNullOrEmpty(cursor.Name))
                            g.DrawString($"{cursor.Name}", dataSource.Settings.Style.Font, brush, new PointF(x, 0));
                    }
                    catch (Exception ex)
                    {
                        g.DrawString(ex.Message, dataSource.Settings.Style.Font, brush, new Point(0, cursorNo * dataSource.Settings.Style.Font.Height));
                    }
                    cursorNo++;
                }

                //Draw popup with list of marksers and trace values
                if (ModifierKeys.HasFlag(Keys.Control) || dataSource.Settings.Style.AlwaysDetectRadius)
                {
                    int radius = dataSource.Settings.Style.DetailDetectRadius;
                    g.DrawCircle(Pens.White, mousePos.X, mousePos.Y, radius);
                    List<Label> toDo = new List<Label>();
                    foreach (Label marker in dataSource.Labels)
                    {
                        Func<PointD, PointD> convert = null;
                        if (marker is LinkedLabel lm)
                        {
                            if (!lm.Trace.Visible)
                                continue;
                            if (dataSource.Traces.Contains(lm.Trace))
                            {
                                double pxPerUnits_ver = viewPort.Height / (dataSource.Settings.VerticalDivisions * lm.Trace.Scale);
                                convert = (p) => new PointD(
                                    ((p.X + dataSource.Settings.HorOffset) * pxPerUnits_hor) + viewPort.X,
                                    (zeroPos - (p.Y + lm.Trace.Offset) * pxPerUnits_ver));
                            }
                        }
                        else if (marker is FreeLabel fm)
                        {
                            double pxPerUnits_ver = viewPort.Height / (dataSource.Settings.VerticalDivisions * 1);
                            convert = (p) => new PointD(
                                ((p.X + dataSource.Settings.HorOffset) * pxPerUnits_hor) + viewPort.X,
                                (zeroPos - (p.Y + 0) * pxPerUnits_ver));
                        }
                    
                        if (convert != null)
                        {
                            PointD screenPt = convert.Invoke(marker.Point);
                            double distance = PointD.Distance(screenPt, mousePos);
                            if (distance < radius)
                            {
                                toDo.Add(marker);
                            }
                        }
                    }

                    List<Tuple<Trace, double>> toDoTraces = new();
                    foreach(var trace in DataSource.Traces)
                    {
                        if (!trace.Visible)
                            continue;

                        if (trace.DrawStyle == Trace.DrawStyles.Lines
                            || trace.DrawStyle == Trace.DrawStyles.NonInterpolatedLine)
                        {
                            double pxPerUnits_hor2 = viewPort.Width / (dataSource.Settings.HorizontalDivisions * dataSource.Settings.HorScale);
                            double worldX = ((mousePos.X - viewPort.X) / pxPerUnits_hor2) - dataSource.Settings.HorOffset;
                            double val = trace.GetYValue(worldX);
                            double pxPerUnits_ver = viewPort.Height / (dataSource.Settings.VerticalDivisions * trace.Scale);
                            double screenVal = (zeroPos - (val + trace.Offset) * pxPerUnits_ver);
                            if (screenVal > mousePos.Y - radius && screenVal < mousePos.Y + radius)
                                toDoTraces.Add(new Tuple<Trace, double>(trace, val));
                        }
                    }

                    int linesCount = toDo.Sum(a=>a.Text.Count(c=>c == '\n') + (a.Text.EndsWith('\n')?0:1)) + toDoTraces.Count;
                    if (linesCount > 0)
                    {
                        int width = dataSource.Settings.Style.DetailWindowWidth;
                        int lineHeight = DataSource.Settings.Style.Font.Height;
                        int lineNo = 0;
                        int tempRad = radius + 2;
                        if (tempRad > 25)
                            tempRad = 25;
                        Rectangle window = new Rectangle(mousePos, new Size(width, lineHeight * linesCount));
                        KeepInWindow(ref window, viewPort, mousePos, tempRad);
                        SolidBrush brush = new SolidBrush(DataSource.Settings.Style.BackgroundColor);
                        Pen border = new Pen(DataSource.Settings.Style.ForegroundColor);

                        g.FillRectangle(brush, window); //Draw window background
                        foreach (var marker in toDo)
                        {
                            SolidBrush pen = new SolidBrush(marker.Color);

                            var lines = marker.Text.Split('\n');
                            int line = 0;
                            for (line = 0; line < lines.Length; line++)
                            {
                                Rectangle rowRectangle = new Rectangle(window.X, window.Y + ((lineNo + line) * lineHeight), window.Width, lineHeight);
                                string text = line == 0 ? $"- {lines[line]}" : $"  {lines[line]}";
                                g.DrawString(text, DataSource.Settings.Style.Font, pen, rowRectangle);
                            }
                            lineNo += line;
                        }
                        foreach (var val in toDoTraces)
                        {
                            SolidBrush pen = new SolidBrush(val.Item1.Color);
                            Rectangle rowRectangle = new Rectangle(window.X, window.Y + (lineNo * lineHeight), window.Width, lineHeight);
                            g.DrawString(val.Item1.Name, DataSource.Settings.Style.Font, pen, rowRectangle);
                            g.DrawString(val.Item1.ToHumanReadable.Invoke(val.Item2), DataSource.Settings.Style.Font, pen, rowRectangle, new StringFormat { Alignment = StringAlignment.Far });
                            lineNo++;
                        
                        }
                        g.DrawRectangle(border, window);  //Draw border
                    }
                }
            }

            stopwatch.Stop();
#if DEBUG
            g.DrawString($"F: {stopwatch.ElapsedMilliseconds.ToString().PadRight(5)} ms", dataSource.Settings.Style.Font, Brushes.White, new Point(viewPort.Width, 0 * dataSource.Settings.Style.Font.Height), new StringFormat() { Alignment = StringAlignment.Far });
#endif
        }

        void KeepInWindow(ref Rectangle rectangle, Rectangle window, Point mouse, int radius)
        {
            if (Collides(window, mouse, radius))    //Window is to bottom right of the mouse
                rectangle.X = mouse.X + radius;

            if (rectangle.X + rectangle.Width > window.Width)
            {
                rectangle.X = window.Width - rectangle.Width;   //Move left
                if (Collides(window, mouse, radius))
                {
                    rectangle.X = mouse.X - rectangle.Width - radius;
                }
            }

            if(rectangle.Y + rectangle.Height > window.Height)
            {
                rectangle.Y = window.Height - rectangle.Height; //Move up
                if (Collides(window, mouse, radius))
                {
                    rectangle.Y = mouse.Y - rectangle.Height - radius;
                }
            }

            if (rectangle.X < 0)
                rectangle.X = 0;
            if (rectangle.Y < 0)
                rectangle.Y = 0;

        }

        bool Collides(Rectangle rectangle, Point point, int radius) => point.X - radius > rectangle.X && point.Y - radius > rectangle.Y && point.X + radius < rectangle.X + rectangle.Width && point.Y + radius < rectangle.Y + rectangle.Height;

        #endregion


        private void DrawAll()
        {
            DrawBackground();
            DrawData();
            DrawForeground();
        }

        //Draw the background
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawBackground(g);
        }

        private void PictureBox2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawData(g);
        }

        private void PictureBox3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawForeground(g);
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
    }
}
