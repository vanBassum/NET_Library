using System;
using System.ComponentModel;

namespace CoreLib.Ethernet
{
    public enum ConnectionStates
    {
        Disconnected,
        Connected,
        Connecting,
        Canceled,
        Error,
    }
}
