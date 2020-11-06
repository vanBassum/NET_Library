using System;



namespace STDLib.JBVProtocol
{
    public interface IConnection
    {
        void SendData(byte[] data);
        event EventHandler<byte[]> OnDataRecieved;
    }

}