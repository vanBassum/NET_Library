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
            client.OnDataRecieved += (sender, data) => { 
                OnDataRecieved?.Invoke(this, data); };
        }

        public void SendData(byte[] data)
        {
            client.SendDataSync(data);
        }
    }
}