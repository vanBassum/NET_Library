using STDLib.Ethernet;
using System;


namespace STDLib.JBVProtocol
{
    public class TCPConnection : IConnection
    {
        TcpSocketClient client;

        public event EventHandler<byte[]> OnDataRecieved;

        public TCPConnection(TcpSocketClient client)
        {
            this.client = client;
            client.OnDataRecieved += (sender, data) =>
            {
                OnDataRecieved?.Invoke(this, data);
            };
        }

        public bool SendData(byte[] data)
        {
            return data.Length == client.SendDataSync(data);
        }
    }
}