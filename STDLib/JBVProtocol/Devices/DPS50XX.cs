using System;
using System.Collections.Generic;
using System.Text;

namespace STDLib.JBVProtocol.Devices
{
    /*
    public class DPS50XX : Device
    {
        public override SoftwareID SoftwareID => SoftwareID.DPS50xx;

        public void SetLED(bool status)
        {
            Frame f = new Frame();
            f.CommandID = (UInt32)CMDS.SetLED;
            f.RxID = ID;
            f.SetData(new byte[] { status ? (byte)1 : (byte)0 });
            JBVClient.SendFrame(f);
        }

        public void SendUART(string message)
        {
            Frame f = new Frame();
            f.CommandID = (UInt32)CMDS.SendUart;
            f.RxID = ID;
            f.SetData(Encoding.ASCII.GetBytes(message));
            JBVClient.SendFrame(f);
        }

        public void SendUART(byte[] data)
        {
            Frame f = new Frame();
            f.CommandID = (UInt32)CMDS.SendUart;
            f.RxID = ID;
            f.SetData(data);
            JBVClient.SendFrame(f);
        }

        public void Test(byte testNo)
        {
            Frame f = new Frame();
            f.CommandID = (UInt32)CMDS.Test;
            f.RxID = ID;
            f.SetData(new byte[] { testNo });
            JBVClient.SendFrame(f);
        }


        enum CMDS
        {
            SetLED = 1,
            SendUart = 2,
            Test = 3,
        }

    }
    */
}
