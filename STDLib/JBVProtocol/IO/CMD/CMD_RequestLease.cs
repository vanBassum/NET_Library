using STDLib.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STDLib.JBVProtocol.IO.CMD
{
    public class CMD_RequestLease : BaseCommand
    {
        public Guid Key { get; set; }


        public CMD_RequestLease()
        {
        }

        public CMD_RequestLease(Guid guid)
        {
            Key = guid;
        }


        public override void FromArray(byte[] data)
        {
            Key = new Guid(data.SubArray(1, 16));
        }

        public override byte[] ToArray()
        {
            List<byte> data = new List<byte> { (byte)CommandTable.Reverse[this.GetType()] };
            data.AddRange(Key.ToByteArray());
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
