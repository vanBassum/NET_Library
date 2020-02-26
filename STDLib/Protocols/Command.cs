using System;
using System.Collections.Generic;

namespace STDLib
{
    public class Command
    {
        private UInt16 cmd;
        public byte[] Data { get; set; }


        public Command Clone()
        {
            Command cmd = new Command();
            cmd.cmd = this.cmd;
            cmd.Data = (byte[])Data.Clone();
            return cmd;
        }

        public bool IsError
        {
            get { return ((cmd & 0x8000) > 0); }
            set { _ = value ? (cmd |= 0x8000) : (cmd &= 0x7FFF); }
        }

        public UInt16 CMD
        {
            get { return (UInt16)(cmd & 0x7FFF); }
            set { cmd = (UInt16)((cmd & 0x8000) | (value & 0x7FFF)); }
        }

        public void Populate(byte[] raw)
        {
            if (raw.Length >= 2)
            {
                cmd = BitConverter.ToUInt16(raw, 0);
                Data = raw.SubArray(2);
            }
        }

        public byte[] GetBytes()
        {
            List<byte> raw = new List<byte>();
            raw.AddRange(BitConverter.GetBytes(cmd));
            if (Data != null)
                raw.AddRange(Data);
            return raw.ToArray();
        }
    }

}
