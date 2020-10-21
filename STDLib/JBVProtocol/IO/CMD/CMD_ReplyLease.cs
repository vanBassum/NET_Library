using STDLib.Extentions;
using System;
using System.Collections.Generic;

namespace STDLib.JBVProtocol.IO.CMD
{
    public class CMD_ReplyLease : BaseCommand
    {
        public Lease Lease { get; set; }

        public CMD_ReplyLease()
        {
        }

        public CMD_ReplyLease(Lease lease)
        {
            Lease = lease;
        }


        public override void FromArray(byte[] data)
        {
            Lease = new Lease(data.SubArray(1));
        }

        public override byte[] ToArray()
        {
            List<byte> data = new List<byte> { (byte)CommandTable.Reverse[this.GetType()] };
            data.AddRange(Lease.ToByteArray());
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
