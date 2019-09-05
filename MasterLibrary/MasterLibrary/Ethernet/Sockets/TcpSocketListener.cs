using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterLibrary.Ethernet
{
    public class TcpSocketListener
    {
        public delegate void OnClientAcceptHandler(TcpSocketClient client);
        public event OnClientAcceptHandler OnClientAccept;

        Socket globalSocket;

        public TcpSocketListener()
        {
            globalSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public void BeginListening(int port)
        {
            BeginListening( new IPEndPoint(IPAddress.Any, port));
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

            //System.Threading.SynchronizationContext context = System.Threading.SynchronizationContext.Current ?? new System.Threading.SynchronizationContext();

            try
            {

                TcpSocketClient tcpclient = (TcpSocketClient)Activator.CreateInstance(typeof(TcpSocketClient), handler);
                //context.Send(s =>
                //{
                    OnClientAccept?.Invoke(tcpclient);
                //}, null);
            }
            catch (Exception ex)
            {
                handler.Close();
                /*
                if (!((ex is ObjectDisposedException)
                    || (ex is NullReferenceException)
                    ))
                {
                    context.Send(s =>
                    {
                        OnConnectionFailed?.Invoke(this, null);
                    }, null);
                }
                */
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
