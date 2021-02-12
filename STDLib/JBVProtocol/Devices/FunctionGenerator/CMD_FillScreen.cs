using STDLib.JBVProtocol.Commands;
using System.Drawing;
using System;

namespace STDLib.JBVProtocol.FunctionGenerator
{
    public partial class FunctionGenerator
    {
        /*
        public async Task<bool> SetLED(bool val, CancellationToken? ct = null)
        {
            SetLED cmd = new SetLED();
            cmd.RxID = ID;
            cmd.Led = val;

            Command rx = await Client.SendRequest(cmd, ct);
            return rx is ReplyACK;
        }
        */

        public class CMD_FillScreen : Command
        {
            protected override bool IsBroadcast => false;
            public override UInt32 CommandID => 2;

            public Color Color { get; set; }


            public override void Populate(byte[] data)
            {
                Color = Color.FromArgb(data[0], data[1], data[2]);
            }

            public override byte[] ToArray()
            {
                return new byte[] { Color.R, Color.G, Color.B };
            }
        }


    }

}