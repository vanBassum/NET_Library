using Oscilloscope;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Log_temp_verloop_warmwelkom
{
    public partial class Form1 : Form
    {
        ScopeControl scope;
        LogFile<LogItemGateway> gatewayLog = new LogFile<LogItemGateway>();
        LogFile<LogItemDoor> doorLog = new LogFile<LogItemDoor>();

        Trace temp;
        Trace setPt;
        Trace events;
        Trace relais1;
        Trace heatreq;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            scope = new ScopeControl();
            textBox1.AllowDrop = true;
            textBox2.AllowDrop = true;
            scope.AllowDrop = true;
            textBox1.DragDrop += Gateway_DragDrop;
            textBox2.DragDrop += Deurslot_DragDrop;
            scope.DragDrop += GatewayLoad_DragDrop;
            scope.DragEnter += DragEnter;
            textBox1.DragEnter += DragEnter;
            textBox2.DragEnter += DragEnter;
            scope.Scope.Settings.HorizontalIsDate = true;
            splitContainer1.Panel1.Controls.Add(scope);
            scope.Dock = DockStyle.Fill;
            scope.Scope.Traces.Add(events = new Trace { Name = "Events", Scale = 10, Unit = "C", Colour = Color.White, DrawStyle = Trace.TraceDrawStyle.Points });
            scope.Scope.Traces.Add(temp = new Trace { Name = "Temp", Scale = 10, Unit = "C", Colour = Color.Yellow, DrawStyle = Trace.TraceDrawStyle.Lines });
            scope.Scope.Traces.Add(setPt = new Trace { Name = "Setpt", Scale = 10, Unit = "C", Colour = Color.Red,  DrawStyle = Trace.TraceDrawStyle.Points | Trace.TraceDrawStyle.Lines | Trace.TraceDrawStyle.NoInterpolation | Trace.TraceDrawStyle.ExtendBegin | Trace.TraceDrawStyle.ExtendEnd });
            scope.Scope.Traces.Add(relais1 = new Trace { Name = "Relais 1", Position = -3, Scale = 2, Unit = "", Colour = Color.DeepSkyBlue, DrawStyle = Trace.TraceDrawStyle.Points | Trace.TraceDrawStyle.Lines | Trace.TraceDrawStyle.NoInterpolation | Trace.TraceDrawStyle.ExtendBegin | Trace.TraceDrawStyle.ExtendEnd });
            scope.Scope.Traces.Add(heatreq = new Trace { Name = "Heating request", Position = -2, Scale = 2, Unit = "", Colour = Color.Violet, DrawStyle = Trace.TraceDrawStyle.Points | Trace.TraceDrawStyle.Lines | Trace.TraceDrawStyle.NoInterpolation | Trace.TraceDrawStyle.ExtendBegin | Trace.TraceDrawStyle.ExtendEnd });

            scope.MarkerLineView.Dock = DockStyle.Fill;
            splitContainer2.Panel1.Controls.Add(scope.MarkerLineView);

            scope.MathView.Dock = DockStyle.Fill;
            splitContainer2.Panel2.Controls.Add(scope.MathView);

            scope.MathView.MathItems.Add(new LinearRegressionHours());

        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void Gateway_DragDrop(object sender, DragEventArgs e)
        {
            var dropped = ((string[])e.Data.GetData(DataFormats.FileDrop));
            var files = dropped.ToList();

            if (!files.Any())
                return;

            textBox1.Text = files.First();
        }

        private void GatewayLoad_DragDrop(object sender, DragEventArgs e)
        {
            var dropped = ((string[])e.Data.GetData(DataFormats.FileDrop));
            var files = dropped.ToList();

            if (!files.Any())
                return;

            textBox1.Text = files.First();
            OpenGatewayLog(textBox1.Text);
            DrawLogs();
        }

        private void Deurslot_DragDrop(object sender, DragEventArgs e)
        {
            var dropped = ((string[])e.Data.GetData(DataFormats.FileDrop));
            var files = dropped.ToList();

            if (!files.Any())
                return;

            textBox2.Text = files.First();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenGatewayLog(textBox1.Text);
            OpenDeurslotLog(textBox2.Text);
            DrawLogs();
        }

        void OpenGatewayLog(string file)
        {
            if (File.Exists(file))
                gatewayLog = LogFile<LogItemGateway>.Open(file);
        }

        void OpenDeurslotLog(string file)
        {
           if (File.Exists(file))
                doorLog = LogFile<LogItemDoor>.Open(file);
        }


        void DrawLogs()
        {
            scope.Scope.MarkerLines.Clear();
            foreach (Trace trace in scope.Scope.Traces)
                trace.Points.Clear();
            

            DateTime first = DateTime.MaxValue;
            DateTime last = DateTime.MinValue;

            foreach (var logItem in gatewayLog.Items.Concat(doorLog.Items))
            {
                if (logItem.Timestamp < DateTime.Parse("01-Jan-2020 12:00:00"))
                    continue;

                if (logItem.GetType().BaseType == typeof(LogItemGateway))
                {
                    if (logItem.Timestamp < first)
                        first = logItem.Timestamp;

                    if (logItem.Timestamp > last)
                        last = logItem.Timestamp;
                }

                switch (logItem)
                {
                    case LOG_RoomTempActualChanged li:
                        temp.Points.Add(new PointD(li.Timestamp.Ticks, li.Temperature));
                        break;

                    case LOG_RoomTempSetpointChanged li:
                        setPt.Points.Add(new PointD(li.Timestamp.Ticks, li.Setpoint));

                        break;
                    case LOG_RoomTempSetpointOverride li:
                        events.Marks.Add(new Mark(li.Timestamp.Ticks, li.Setpoint, "OVE"));
                        break;

                    case LOG_Restarted li:
                        events.Marks.Add(new Mark(li.Timestamp.Ticks, 0, "RES", Color.Green));
                        break;

                    case LOG_DoorOpened li:
                        events.Marks.Add(new Mark(li.Timestamp.Ticks, -10, "DOOR", Color.Green));
                        break;

                    case LOG_CurrentReservationChanged li:
                        events.Marks.Add(new Mark(li.Timestamp.Ticks, -10, "LRS", Color.Green));
                        break;

                    case LOG_Relais1 li:
                        relais1.Points.Add(new PointD(li.Timestamp.Ticks, li.Active?1:0));
                        break;

                    case LOG_HeatingRequest li:
                        heatreq.Points.Add(new PointD(li.Timestamp.Ticks, li.Active ? 1 : 0));
                        break;
                }
            }

            //scope.Scope.AutoScale(true);

            scope.Scope.Settings.TimeBase = last.Ticks - first.Ticks;
            scope.Scope.Settings.Offset = - first.Ticks;

            //setPt.Points.Add(new PointD(last.Ticks, setPt.Points.Last().Y));
            //events.Scale = 10;
            scope.Draw();
            //scope.Scope.AddMarkerLine(DateTime.Parse("25-1-2020 22:49:31"));
        }
        
    }

    public class LinearRegressionHours : MathItem
    {
        public override void SetResult(params object[] data)
        {
            if (this.CalculationType == CalculationType.Linear_regression)
            {
                if(data.Length == 2)
                {
                    Result = string.Format("{0} °C per hour", ((double)data[1] * 10000 * 1000 * 60 * 60).ToString("0.####"));


                    return;
                }
            }
            base.SetResult(data);
        }
    }
}
