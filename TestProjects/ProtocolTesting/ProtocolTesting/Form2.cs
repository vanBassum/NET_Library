using FRMLib.Scope;
using STDLib.Ethernet;
using STDLib.JBVProtocol.Connections;
using STDLib.JBVProtocol.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProtocolTesting
{
    public partial class Form2 : Form
    {

        TCPConnection connection;
        TcpSocketClient client = new TcpSocketClient();

        ScopeController scope = new ScopeController();

        Trace trace = new Trace() { Offset = 0, Scale = 2500 };

        public Form2()
        {
            InitializeComponent();
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            await client.ConnectAsync("127.0.0.1:1000");
            //await client.ConnectAsync("192.168.0.10:31600");
            connection = new TCPConnection(client);
            connection.OnFrameReceived += Connection_OnFrameReceived;

            scopeView1.DataSource = scope;
            traceView1.DataSource = scope;
            markerView1.DataSource = scope;
            scope.Traces.Add(trace);

            timer1.Interval = 250;
            //timer1.Start();

        }

        int i = 0;
        private void Connection_OnFrameReceived(object sender, Frame e)
        {
            UInt32 scale = BitConverter.ToUInt32(e.PAY, 0);

            double x1 = 8533928;
            double x2 = 8551186;
            double y1 = 0;
            double y2 = 2926.48;

            double a = (y2 - y1) / (x2 - x1);
            double b = -a * x1 + y1;



            double val = a * scale + b;

            trace.Points.Add(i++, val);
            scopeView1.AutoScaleHorizontal();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            byte[] payload = new byte[] { 0 };
            Frame tx = Frame.CreateMessageFrame(0, 0, payload);

            connection.SendFrame(tx);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            byte[] payload = new byte[] { 1, (byte)numericUpDown1.Value, (byte)numericUpDown2.Value };
            Frame tx = Frame.CreateMessageFrame(0, 0, payload);

            connection.SendFrame(tx);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ble();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            ble();
        }

        void ble()
        {
            byte[] payload = new byte[] { 1, (byte)numericUpDown1.Value, (byte)numericUpDown2.Value };
            Frame tx = Frame.CreateMessageFrame(0, 0, payload);

            connection.SendFrame(tx);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] payload = new byte[] { 0 };
            Frame tx = Frame.CreateMessageFrame(0, 0, payload);

            connection.SendFrame(tx);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int scale = 9055626;

            double x1 = 8533928;
            double x2 = 8551186;
            double y1 = 0;
            double y2 = 2926.48;

            double a = (y2 - y1) / (x2 - x1);
            double b = -a * x1 + y1;
            double val = a * scale + b;
        }
    }
}
