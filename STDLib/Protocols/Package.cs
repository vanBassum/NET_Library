using System;
using System.Collections.Generic;

namespace STDLib
{

    public class Package
    {
        public UInt16 SID { get; set; }
        public UInt16 RID { get; set; }
        public UInt16 TID { get; set; }
        private byte Flags { get; set; }
        public Command Command { get; set; }



        public Package Clone()
        {
            Package p = new Package();
            p.Command = Command.Clone();
            p.SID = SID;
            p.TID = TID;
            p.RID = RID;
            p.Flags = Flags;
            return p;
        }

        public bool IsReply
        {
            get { return ((Flags & 0x01) > 0); }
            set { _ = value ? (Flags |= 0x01) : Flags &= 0xFE; }
        }


        public void Populate(byte[] raw)
        {
            if(raw.Length >= 8)
            {
                this.SID = BitConverter.ToUInt16(raw, 0);
                this.RID = BitConverter.ToUInt16(raw, 2);
                this.TID = BitConverter.ToUInt16(raw, 4);
                this.Flags = raw[6];
                this.Command = new Command();
                this.Command.Populate(raw.SubArray(7));
            }
        }

        public byte[] GetBytes()
        {
            List<byte> raw = new List<byte>();

            raw.AddRange(BitConverter.GetBytes(SID));
            raw.AddRange(BitConverter.GetBytes(RID));
            raw.AddRange(BitConverter.GetBytes(TID));
            raw.Add(Flags);
            if (Command != null)
                raw.AddRange(Command.GetBytes());

            return raw.ToArray();
        }

    }


}
