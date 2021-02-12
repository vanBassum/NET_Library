using System;

namespace STDLib.JBVProtocol.Commands
{
    public class RoutingInvalid : Command
    {
        protected override bool IsBroadcast => false;
        public override UInt32 CommandID => (UInt32)CommandList.RoutingInvalid;
        public override void Populate(byte[] data)
        {

        }

        public override byte[] ToArray()
        {
            return new byte[0];
        }
    }


}