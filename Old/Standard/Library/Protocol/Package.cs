using System;
using System.Collections.Generic;

namespace Protocol
{


    public class Package
    {
        private UInt16 cmd;
        public UInt16 SID { get; set; }
        public UInt16 RID { get; set; }
        public UInt16 TID { get; set; }
        public byte[] Data { get; set; }

        public bool IsReply 
        {
            get { return ((cmd & 0x8000) > 0); }
            set { _ = value ? (cmd |= 0x8000) : (cmd &= 0x7FFF); }
        }

        public bool IsError
        {
            get { return ((cmd & 0x4000) > 0); }
            set { _ = value ? (cmd |= 0x4000) : (cmd &= 0xBFFF); }
        }

        public UInt16 CMD
        {
            get { return (UInt16)(cmd & 0x3FFF); }
            set { cmd = (UInt16)((cmd & 0xC000) | (value & 0x3FFF)); }
        }


        public void Populate(byte[] raw)
        {
            if(raw.Length >= 8)
            {
                SID = BitConverter.ToUInt16(raw, 0);
                RID = BitConverter.ToUInt16(raw, 2);
                TID = BitConverter.ToUInt16(raw, 4);
                cmd = BitConverter.ToUInt16(raw, 6);
                Data = new byte[raw.Length - 8];
                Array.Copy(raw, 8, Data, 0, raw.Length - 8);
            }
        }

        public byte[] GetBytes()
        {
            List<byte> raw = new List<byte>();

            raw.AddRange(BitConverter.GetBytes(SID));
            raw.AddRange(BitConverter.GetBytes(RID));
            raw.AddRange(BitConverter.GetBytes(TID));
            raw.AddRange(BitConverter.GetBytes(cmd));
            if(Data != null)
                raw.AddRange(Data);

            return raw.ToArray();
        }

    }
}
