using FRMLib.Scope;
using STDLib.Ethernet;
using STDLib.JBVProtocol;
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
        Client client;
        ScopeController scope = new ScopeController();
        Trace trace = new Trace() { Offset = 0, Scale = 2500 };

        public Form2()
        {
            InitializeComponent();
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            TcpSocketClient tcpClient = new TcpSocketClient();
            await tcpClient.ConnectAsync("127.0.0.1:1000");
            //await tcpClient.ConnectAsync("192.168.0.50:1000");
            TCPConnection connection = new TCPConnection(tcpClient);

            client = new Client(20);
            client.SetConnection(connection);

            client.OnMessageRecieved += Client_OnMessageRecieved;
            client.OnBroadcastRecieved += Client_OnBroadcastRecieved;

            scopeView1.DataSource = scope;
            traceView1.DataSource = scope;
            markerView1.DataSource = scope;
            scope.Traces.Add(trace);

        }

        private void Client_OnBroadcastRecieved(object sender, STDLib.JBVProtocol.Message e)
        {

        }

        private void Client_OnMessageRecieved(object sender, STDLib.JBVProtocol.Message e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            client.SendMessage(10, new byte[] { 0 });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes(" Meh");
            data[0] = 0;
            client.SendMessage(0xFFFE, data);
        }
    }
}
