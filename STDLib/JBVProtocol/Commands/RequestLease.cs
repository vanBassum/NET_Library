using System;
using System.Linq;
using STDLib.Extentions;

namespace STDLib.JBVProtocol.Commands
{
    public class RequestLease : Command
    {
        protected override bool IsBroadcast => true;
        public override UInt32 CommandID => (UInt32)Commands.RequestLease;
        public Guid Key { get; set; }

        public override void Populate(byte[] data)
        {
            Key = new Guid(data);
        }

        public override byte[] ToArray()
        {
            return Key.ToByteArray();
        }
    }
}