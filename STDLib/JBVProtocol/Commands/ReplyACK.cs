using System;

namespace STDLib.JBVProtocol.Commands
{
    public class ReplyACK : Command
    {
        protected override bool IsBroadcast => false;
        public override UInt32 CommandID => (UInt32)CommandList.ReplyACK;
        public override void Populate(byte[] data)
        {

        }

        public override byte[] ToArray()
        {
            return new byte[0];
        }
    }
}