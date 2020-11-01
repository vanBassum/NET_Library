using STDLib.JBVProtocol.Commands;
using STDLib.JBVProtocol.Devices;
using STDLib.JBVProtocol.CMD;
using System.Threading;
using System.Threading.Tasks;

namespace STDLib.JBVProtocol.FunctionGenerator
{
    public class FunctionGenerator : Device
    {
        public override SoftwareID SoftwareID => SoftwareID.FunctionGenerator;

        public async Task<bool> SetLED(bool val, CancellationToken? ct = null)
        {
            SetLED cmd = new SetLED();
            cmd.RxID = ID;
            cmd.Led = val;

            Command rx = await Client.SendRequest(cmd, ct);
            return rx is ReplyACK;
        }

        public async Task<bool> SetFrequency(double val, CancellationToken? ct = null)
        {
            SetFreq cmd = new SetFreq();
            cmd.RxID = ID;
            cmd.Frequency = val;

            Command rx = await Client.SendRequest(cmd, ct);
            return rx is ReplyACK;
        }


    }

}