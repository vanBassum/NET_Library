using STDLib.Misc;
using System;
using System.ComponentModel;

namespace STDLib.JBVProtocol
{
    public enum ConnectionStatus
    {
        Disconnected,
        Connected,
        Connecting,
        Canceled,
        Error,
    }

    public interface IConnection : INotifyPropertyChanged
    {
        ConnectionStatus ConnectionStatus { get; }
        bool SendData(byte[] data);
        event EventHandler<byte[]> OnDataRecieved;
    }

}