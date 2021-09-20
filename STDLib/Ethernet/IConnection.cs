using System;
using System.ComponentModel;

namespace STDLib.Ethernet
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
