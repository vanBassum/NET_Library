using System.Collections.Generic;

namespace STDLib.JBVProtocol
{

    public enum ProtTypes
    {
        TCP,
    };

    public class ListenerInfo
    {
        public ProtTypes ProtocolType { get; set; }

    }

    class TCPListenerInfo : ListenerInfo
    {
        
        public string Ip { get; set; }
        public int Port { get; set; }
    }


    public class DiscoveryInfo
    {
        public SoftwareID SID { get; set; }
        public string Address { get; set; }
        public List<ListenerInfo> Listeners { get; set; }
    }

}
