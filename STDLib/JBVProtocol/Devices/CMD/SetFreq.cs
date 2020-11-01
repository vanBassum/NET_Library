using STDLib.JBVProtocol.Commands;
using System;

namespace STDLib.JBVProtocol.CMD
{
    public class SetFreq : Command
    {
        protected override bool IsBroadcast => false;
        public override UInt32 CommandID => 2;

        public double Frequency { get; set; } = 100;

        public override void Populate(byte[] data)
        {
            Frequency = BitConverter.ToDouble(data, 0);
        }

        public override byte[] ToArray()
        {
            return BitConverter.GetBytes(Frequency);
        }
    }
}