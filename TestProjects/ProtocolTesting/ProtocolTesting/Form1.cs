using STDLib.Ethernet;
using STDLib.JBVProtocol;
using STDLib.JBVProtocol.Connections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FRMLib;

namespace ProtocolTesting
{
    public partial class Form1 : Form
    {
        Client client1;
        SerialConnection connection;
 
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
            return;
            connection = new SerialConnection("COM5", 115200);
            client1 = new Client(1);
            client1.SetConnection(connection);
            client1.OnMessageRecieved += Client1_OnMessageRecieved;


            button1.Click += (a, b) => client1.SendMessage(0, Add(0, BitConverter.GetBytes(float.Parse(textBox1.Text))));
            button2.Click += (a, b) => client1.SendMessage(0, Add(1, BitConverter.GetBytes(float.Parse(textBox2.Text))));
            button3.Click += (a, b) => client1.SendMessage(0, Add(2, BitConverter.GetBytes(float.Parse(textBox3.Text))));
            button4.Click += (a, b) => client1.SendMessage(0, Add(3, BitConverter.GetBytes(UInt16.Parse(textBox4.Text))));
            button5.Click += (a, b) => client1.SendMessage(0, Add(4, BitConverter.GetBytes(float.Parse(textBox5.Text))));
            button6.Click += (a, b) => client1.SendMessage(0, Add(5, BitConverter.GetBytes(float.Parse(textBox6.Text))));
        }

        private void Client1_OnMessageRecieved(object sender, STDLib.JBVProtocol.Message e)
        {
            string msg = Encoding.ASCII.GetString(e.Payload);
            richTextBox1.InvokeIfRequired(() => richTextBox1.AppendText(msg + "\r\n"));
        }


        byte[] Add(byte b, byte[] bt)
        {
            byte[] o = new byte[bt.Length + 1];
            o[0] = b;
            bt.CopyTo(o, 1);
            return o;
        }


    }









    

}
