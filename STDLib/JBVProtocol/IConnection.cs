using System;



namespace STDLib.JBVProtocol
{
    public interface IConnection
    {
        bool SendData(byte[] data);
        event EventHandler<byte[]> OnDataRecieved;
    }

}