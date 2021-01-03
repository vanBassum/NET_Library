using System;


namespace STDLib.JBVProtocol
{
    public class DummyConnection : IConnection
    {
        public event EventHandler<byte[]> DoSendData;
        public event EventHandler<byte[]> OnDataRecieved;

        public void SendData(byte[] data)
        {
            DoSendData?.Invoke(this, data);
        }

        void HandleData(byte[] data)
        {
            OnDataRecieved?.Invoke(this, data);
        }

        public static void CoupleConnections(DummyConnection con1, DummyConnection con2)
        {
            con1.DoSendData += (a, b) => con2.HandleData(b);
            con2.DoSendData += (a, b) => con1.HandleData(b);
        }

        public static void CoupleConnections(IConnection con1, IConnection con2)
        {
            if (con1 is DummyConnection dcon1 && con2 is DummyConnection dcon2)
            {
                CoupleConnections(dcon1, dcon2);
            }
            else
                throw new Exception("Connections aren't dummyconnections");
        }
    }
}