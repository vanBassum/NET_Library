using STDLib.JBVProtocol.Commands;
using STDLib.JBVProtocol.Devices;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System;
using System.Collections.Generic;

namespace STDLib.JBVProtocol.FunctionGenerator
{
    public partial class FunctionGenerator : Device
    {
        public override SoftwareID SoftwareID => SoftwareID.FunctionGenerator;


        public async Task<bool> FillScreen(Color color, CancellationToken? ct = null)
        {
            byte[] data = { color.R, color.G, color.B};

            CMD_FillScreen cmd = new CMD_FillScreen();
            cmd.Color = color;

            Command rx = await Send(cmd, ct);
            return rx is ReplyACK;
        }


        public async Task<bool> DrawLine(UInt16 x1, UInt16 y1, UInt16 x2, UInt16 y2,  Color color, CancellationToken? ct = null)
        {
            CMD_DrawLine cmd = new CMD_DrawLine();
            cmd.Color = color;
            cmd.X1 = x1;
            cmd.Y1 = y1;
            cmd.X2 = x2;
            cmd.Y2 = y2;
            Command rx = await Send(cmd, ct);
            return rx is ReplyACK;
        }




        /*
        public async Task<bool> SetFrequency(double val, CancellationToken? ct = null)
        {
            SetFreq cmd = new SetFreq();
            cmd.RxID = ID;
            cmd.Frequency = val;

            Command rx = await Client.SendRequest(cmd, ct);
            return rx is ReplyACK;
        }
        */

    }

}