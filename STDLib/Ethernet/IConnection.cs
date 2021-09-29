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

    public enum ConnectionTypes
    {
        Unknown,
        Direct,     //Connected to max 1 other endpoint
        Broadcast,  //Connected to multiple (unknown) endpoints.
    }

    public interface IConnection : INotifyPropertyChanged
    {
        ConnectionTypes Type { get; set; }
        ConnectionStatus ConnectionStatus { get; }
        bool SendData(byte[] data);
        event EventHandler<byte[]> OnDataRecieved;

        

    }

}
