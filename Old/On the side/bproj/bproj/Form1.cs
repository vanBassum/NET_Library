using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bproj
{
    public partial class Form1 : Form
    {
        BVProtocol prot = new BVProtocol();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            prot.OnRawDataOut = SendRaw;
            prot.OnCommandRecieved = RequestRecieved;
        }

        void SendRaw(List<byte> data)
        {
            //send via phy
            prot.RawDataIn(data);
        }


        void RequestRecieved(Command cmd)
        {
            prot.SendResponse(Command.ResponseType.Unknown, null);
        }

        void ResponseRecieved(Command cmd)
        {
            Command.ResponseType res = cmd.GetResponse();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            prot.SendRequest(0x00, null, ResponseRecieved);
        }

    }

}
