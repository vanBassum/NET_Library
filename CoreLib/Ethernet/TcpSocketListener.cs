using System;
using System.Net;
using System.Net.Sockets;

namespace CoreLib.Ethernet
{
    public class TcpSocketListener
    {
        public event EventHandler<TcpSocketClient> OnClientAccept;

        Socket globalSocket;

        public TcpSocketListener()
        {
            globalSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public void BeginListening(int port)
        {
            BeginListening(new IPEndPoint(IPAddress.Any, port));
        }

        public void BeginListening(IPEndPoint ep)
        {
            IPAddress hostIP = (Dns.Resolve(IPAddress.Any.ToString())).AddressList[0];
            globalSocket.Bind(ep);

            globalSocket.Listen(0);
            globalSocket.BeginAccept(OnConnect, globalSocket);

        }

        private void OnConnect(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            try
            {
                OnClientAccept?.Invoke(this, new TcpSocketClient(handler));
            }
            catch (Exception ex)
            {
                handler.Close();
            }

            globalSocket.BeginAccept(OnConnect, globalSocket);

        }

        public static IPAddress GetLocalIPAddress()
        {
            IPAddress localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address;
            }
            return localIP;
        }
    }
}
