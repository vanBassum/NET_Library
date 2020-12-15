using STDLib.JBVProtocol.CMD;
using STDLib.JBVProtocol.Commands;
using STDLib.JBVProtocol.Devices;
using System.Threading;
using System.Threading.Tasks;

namespace STDLib.JBVProtocol.DSP50xx
{
    public class DPS50xx : Device
    {
        public override SoftwareID SoftwareID => SoftwareID.DPS50xx;

        public async Task<bool> SetLED(bool val, CancellationToken? ct = null)
        {
            SetLED cmd = new SetLED();
            cmd.RxID = ID;
            cmd.Led = val;

            Command rx = await Client.SendRequest(cmd, ct);
            return rx is ReplyACK;
        }
    }

}