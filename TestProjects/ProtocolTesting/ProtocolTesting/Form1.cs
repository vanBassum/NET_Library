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



        }



    }

    public class Test
    {
        DummyConnection con1;
        DummyConnection con2;

        void RUN()
        {
            con1 = new DummyConnection();
            con2 = new DummyConnection();

            con1.OnSend += Con1_OnSend;
            con2.OnSend += Con2_OnSend;


            Communicator c1 = new Communicator(con1);
            c1.HandleRequestCallback = Handle1;

            Communicator c2 = new Communicator(con2);
            c2.HandleRequestCallback = Handle2;

            byte[] reply = c1.SendRequestToAny(new byte[] { 0xAA });
        }


        private void Con2_OnSend(object sender, byte[] e)
        {
            con1.Recieve(e);
        }

        private void Con1_OnSend(object sender, byte[] e)
        {
            con2.Recieve(e);
        }

        byte[] Handle1(byte[] data)
        {
            return new byte[] { 0x55 };
        }

        byte[] Handle2(byte[] data)
        {
            return new byte[] { 0x44 };
        }
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
