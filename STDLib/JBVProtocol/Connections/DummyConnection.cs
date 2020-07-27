using STDLib.JBVProtocol.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace STDLib.JBVProtocol.Connections
{
    public class DummyConnection : Connection
    {
        public event EventHandler<byte[]> DoSendData;
        protected override void SendData(byte[] data)
        {
            DoSendData?.Invoke(this, data);
        }

        public static void CoupleConnections(DummyConnection con1, DummyConnection con2)
        {
            con1.DoSendData += (a, b) => con2.HandleData(b);
            con2.DoSendData += (a, b) => con1.HandleData(b);
        }
    }
}
