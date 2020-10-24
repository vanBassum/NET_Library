using System;

namespace STDLib.JBVProtocol.Commands
{
    public class ReplySID : Command
    {
        protected override bool IsBroadcast => false;
        public override UInt32 CommandID => (UInt32)CommandList.ReplySID;
        public SoftwareID SID { get; set; }

        public override void Populate(byte[] data)
        {
            SID = (SoftwareID)BitConverter.ToUInt32(data, 0);
        }

        public override byte[] ToArray()
        {
            return BitConverter.GetBytes((UInt32)SID);
        }
    }
}