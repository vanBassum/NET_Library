using System;

namespace STDLib.JBVProtocol.Commands
{
    public class RequestID : Command
    {
        protected override bool IsBroadcast => true;
        public override UInt32 CommandID => (UInt32)Commands.RequestID;
        public UInt16 ID { get; set; }

        public override void Populate(byte[] data)
        {
            ID = BitConverter.ToUInt16(data, 0);
        }

        public override byte[] ToArray()
        {
            return BitConverter.GetBytes(ID);
        }
    }
}