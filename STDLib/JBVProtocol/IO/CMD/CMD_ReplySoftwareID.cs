using System;
using System.Collections.Generic;

namespace STDLib.JBVProtocol.IO.CMD
{
    public class CMD_ReplySoftwareID : BaseCommand
    {
        public SoftwareID Sid { get; set; }

        public CMD_ReplySoftwareID()
        {
        }

        public CMD_ReplySoftwareID(SoftwareID sid)
        {
            Sid = sid;
        }


        public override void FromArray(byte[] data)
        {
            Sid = (SoftwareID)BitConverter.ToUInt32(data, 1);
        }

        public override byte[] ToArray()
        {
            List<byte> data = new List<byte> { (byte)CommandTable.Reverse[this.GetType()] };
            data.AddRange(BitConverter.GetBytes((UInt32)Sid));
            return data.ToArray();
        }

        public Frame CreateCommandFrame(UInt16 SID)
        {
            Frame frame = new Frame();
            frame.HOP = 0;
            frame.SID = SID;
            frame.RID = 0;
            frame.PAY = ToArray();
            frame.Broadcast = false;
            frame.Command = true;
            return frame;
        }
    }

}
