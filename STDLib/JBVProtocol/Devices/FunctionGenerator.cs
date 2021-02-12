using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace STDLib.JBVProtocol.Devices
{

    public class FunctionGenerator : Device
    {
        public override SoftwareID SoftwareID => SoftwareID.FunctionGenerator;

        public FunctionGenerator(JBVClient JBVClient, UInt16 ID) : base(JBVClient, ID)
        {

        }


        bool IsAck(Frame f)
        {
            return f.CommandID == (UInt32)CommandList.ReplyACK;
        }

        /*
        public void SetLED(bool status)
        {
            Frame f = new Frame();
            f.CommandID = (UInt32)CMDS.SetLED;
            f.RxID = ID;
            f.SetData(new byte[] { status ? (byte)1 : (byte)0 });
            JBVClient.SendFrame(f);
        }
        */

        public async Task<bool> FillScreen(Color color, CancellationToken? ct = null)
        {
            List<byte> data = new List<byte>();
            data.Add(color.R);
            data.Add(color.G);
            data.Add(color.B);
            return IsAck(await SendRequest((UInt32)CMDS.FillScreen, data.ToArray()));
        }


        public async Task<bool> DrawLine(UInt16 x1, UInt16 y1, UInt16 x2, UInt16 y2, Color color, CancellationToken? ct = null)
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(x1));
            data.AddRange(BitConverter.GetBytes(y1));
            data.AddRange(BitConverter.GetBytes(x2));
            data.AddRange(BitConverter.GetBytes(y2));
            data.Add(color.R);
            data.Add(color.G);
            data.Add(color.B);
            return IsAck(await SendRequest((UInt32)CMDS.DrawLine, data.ToArray()));
        }




        enum CMDS
        {
            SetLed = 1,
            FillScreen = 2,
            DrawLine = 3,
        }

    }

   

}
