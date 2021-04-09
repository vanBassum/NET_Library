using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STDLib.JBVProtocol.Devices
{

    public abstract class Device
    {
        public abstract SoftwareID SoftwareID { get; }
        public UInt16 ID { get; }


        JBVClient client;

        public Device(JBVClient JBVClient, UInt16 ID)
        {
            this.client = JBVClient;
            this.ID = ID;
        }
        protected void SendBroadcast(UInt32 cmd, byte[] data)
        {
            Frame f = new Frame();
            f.CommandID = cmd;
            f.RxID = ID;
            f.SetData(data);
            f.Options |= Frame.OPT.Broadcast;
            client.SendFrame(f);
        }

        protected async Task<Frame> SendRequest(UInt32 cmd, byte[] data)
        {
            Frame f = new Frame();
            f.CommandID = cmd;
            f.RxID = ID;
            f.SetData(data);
            return await client.SendRequest(f);
        }

        public override string ToString()
        {
            return SoftwareID.ToString();
        }
    }
}
