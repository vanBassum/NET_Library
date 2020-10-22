using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace STDLib.JBVProtocol.Devices
{
    public class DPS50xx : Device
    {
        public override SoftwareID SoftwareID => SoftwareID.DPS50xx;



        public async Task<bool> SetLED(bool val)
        {
            RequestSetLED cmd = new RequestSetLED();
            cmd.LedStatus = val;
            
            CMD result = await ExecuteCommand(cmd, 10000);

            return result is ReplyACK;
        }


    }
}

