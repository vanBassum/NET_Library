using STDLib.Ethernet;
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
        //JBVClient com1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {



        }


        void Broadcast(object sender, byte[] data)
        {

        }


        byte[] Request(object sender, byte[] data)
        {
            return Encoding.ASCII.GetBytes("ANS");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string s = Encoding.ASCII.GetString(com1.SendRequest(2, Encoding.ASCII.GetBytes("t")));
        }
    }









    

}
