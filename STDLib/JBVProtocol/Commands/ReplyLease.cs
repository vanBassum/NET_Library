using System;

namespace STDLib.JBVProtocol.Commands
{
    public class ReplyLease : Command
    {
        protected override bool IsBroadcast => true;
        public override UInt32 CommandID => (UInt32)CommandList.ReplyLease;
        public Lease Lease { get; set; }

        public override void Populate(byte[] data)
        {
            Lease = new Lease(data);
        }

        public override byte[] ToArray()
        {
            return Lease.ToByteArray();
        }

    }
}