using STDLib.Commands;
using STDLib.Ethernet;
using STDLib.JBVProtocol;
using STDLib.JBVProtocol.Commands;
using System.Collections;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace STDLib.JBVProtocol
{

    public class ConnectionServer
    {
        public JBVClient Client { get; }
        Router router;
        TcpSocketListener listener;
        LeaseServer leaseServer;

        public ConnectionServer()
        {
            Command.InitList();

            Client = new JBVClient(SoftwareID.ConnectionServer);
            router = new Router();
            leaseServer = new LeaseServer();

            DummyConnection client_con1 = new DummyConnection();
            DummyConnection client_con2 = new DummyConnection();
            DummyConnection.CoupleConnections(client_con1, client_con2);

            Client.SetConnection(client_con1);
            router.AddConnection(client_con2);

            DummyConnection lease_con1 = new DummyConnection();
            DummyConnection lease_con2 = new DummyConnection();
            DummyConnection.CoupleConnections(lease_con1, lease_con2);

            leaseServer.SetConnection(lease_con1);
            router.AddConnection(lease_con2);

            listener = new TcpSocketListener();
            listener.OnClientAccept += (sender, client) => router.AddConnection(new TCPConnection(client));
            listener.BeginListening(1000);
        }
    }
}
