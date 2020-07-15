using STDLib.JBVProtocol;
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
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            Router r1 = new Router();
            Router r2 = new Router();

            Bridge bridge_r1r2 = new Bridge();
            r1.AddConnection(bridge_r1r2.C1);
            r2.AddConnection(bridge_r1r2.C2);

            Communicator c1 = new Communicator(1);
            Bridge bridge_r1c1 = new Bridge();
            r1.AddConnection(bridge_r1c1.C1);
            c1.SetConnection(bridge_r1c1.C2);
            c1.Name = "C1";

            Communicator c2 = new Communicator(2);
            Bridge bridge_r2c2 = new Bridge();
            r2.AddConnection(bridge_r2c2.C1);
            c2.SetConnection(bridge_r2c2.C2);
            c2.Name = "C2";


            c2.HandleBroadcastCallback = Broadcast;
            c2.HandleRequestCallback = Request;


            

            c1.SendBroadcast(Encoding.ASCII.GetBytes("Hoi"));
            c2.SendBroadcast(Encoding.ASCII.GetBytes("Hoi"));

            string s = Encoding.ASCII.GetString( c1.SendRequest(2, Encoding.ASCII.GetBytes("REQ")));


        }


        void Broadcast(object sender, byte[] data)
        {

        }


        byte[] Request(object sender, byte[] data)
        {
            return Encoding.ASCII.GetBytes("ANS");
        }


    }



    public class Bridge
    {
        public DummyConnection C1 { get; private set; } = new DummyConnection();
        public DummyConnection C2 { get; private set; } = new DummyConnection();

        public Bridge()
        {
            C1.OnSend += (a, b) => C2.Recieve(b);
            C2.OnSend += (a, b) => C1.Recieve(b);
        }


        public class DummyConnection : Connection
        {
            public event EventHandler<byte[]> OnSend;
            protected override void SendData(byte[] data)
            {
                OnSend?.Invoke(this, data);
            }

            public void Recieve(byte[] data)
            {
                HandleData(data);
            }
        }
    }





    

}
