using STDLib.Ethernet;
using STDLib.JBVProtocol.IO;
using System;
using System.Net.Sockets;

namespace STDLib.JBVProtocol.Connections
{
    public class TCPConnection : Connection
    {
        TcpSocketClient client;


        public TCPConnection(TcpSocketClient client)
        {
            this.client = client;
            client.OnDataRecieved += Client_OnDataRecieved;
        }

        private void Client_OnDataRecieved(object sender, byte[] e)
        {
            HandleData(e);
        }

        protected override void SendData(byte[] data)
        {
            client.SendDataSync(data);
        }

    }
}

