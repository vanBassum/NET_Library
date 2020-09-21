using System;
using System.Collections.Generic;

namespace STDLib.JBVProtocol.IO.CMD
{
    public class CMD_ReplyID : BaseCommand
    {
        public UInt16 ID { get; set; }


        public CMD_ReplyID()
        {
        }

        public CMD_ReplyID(UInt16 id)
        {
            ID = id;
        }

        public override void FromArray(byte[] data)
        {
            ID = BitConverter.ToUInt16(data, 1);
        }

        public override byte[] ToArray()
        {
            List<byte> data = new List<byte> { (byte)CommandTable.Reverse[this.GetType()] };
            data.AddRange(BitConverter.GetBytes(ID));
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

