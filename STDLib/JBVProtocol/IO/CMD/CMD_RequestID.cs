using System;
using System.Collections.Generic;

namespace STDLib.JBVProtocol.IO.CMD
{
    public class CMD_RequestID : BaseCommand
    {
        public UInt16 RequestedID { get; set; }

        public CMD_RequestID()
        {
        }

        public CMD_RequestID(UInt16 requestID)
        {
            RequestedID = requestID;
        }

        public override void FromArray(byte[] data)
        {
            RequestedID = BitConverter.ToUInt16(data, 1);
        }

        public override byte[] ToArray()
        {
            List<byte> data = new List<byte> { (byte)CommandTable.Reverse[this.GetType()] };
            data.AddRange(BitConverter.GetBytes(RequestedID));
            return data.ToArray();
        }

        public Frame CreateCommandFrame(UInt16 SID)
        {
            Frame frame = new Frame();
            frame.HOP = 0;
            frame.SID = SID;
            frame.RID = 0;
            frame.PAY = ToArray();
            frame.Broadcast = true;
            frame.Command = true;
            return frame;
        }
    }
}

