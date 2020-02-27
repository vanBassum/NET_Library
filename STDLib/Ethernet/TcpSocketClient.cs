using System;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace STDLib.Ethernet
{
    /// <summary>
    /// A TCP client created around the .net <see cref="System.Net.Sockets.Socket"> Socket. </see>
    /// </summary>
    public class TcpSocketClient
    {
        Socket globalSocket;
        Timer ConnectionTimeout;
        byte[] rxBuffer = new byte[1024];
        byte[] inOptionValues;
        bool DoKeepAlive = false;

        /// <summary>
        /// Fires when data was recieved by the socket.
        /// </summary>
        public event EventHandler<byte[]> OnDataRecieved;

        /// <summary>
        /// Fires when the socket has sucessfully connected.
        /// </summary>
        public event EventHandler OnConnected;

        /// <summary>
        /// Fires when beginconnect was timeout.
        /// </summary>
        public event EventHandler OnConnectionTimeout;

        /// <summary>
        /// Fires when the connection is closed.
        /// </summary>
        public event EventHandler OnDisconnected;

        /// <summary>
        /// The IP endpoint of the client to witch the socket is connected.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; private set; }

        /// <summary>
        /// Returns true if the socket is connected.
        /// </summary>
        public bool IsConnected { get { return globalSocket == null ? false : globalSocket.Connected; } }


        //--------------------------------------------------------------------------------
        //                          Constructor and destructor
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Creates a new client.
        /// </summary>
        public TcpSocketClient()
        {
            ConnectionTimeout = new Timer();
            ConnectionTimeout.Elapsed += ConnectionTimeout_Tick;
        }

        /// <summary>
        /// Creates a new client based upon an existing socket.
        /// </summary>
        /// <param name="s">The socket to be used by this client.</param>
        public TcpSocketClient(Socket s)
        {
            globalSocket = s;
            OnConnect(globalSocket);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~TcpSocketClient()
        {
            if (globalSocket != null)
                globalSocket.Close();
        }

        //--------------------------------------------------------------------------------
        //                              Connection setup
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Async method to connect to a host.
        /// </summary>
        /// <param name="server">The server to connect to.</param>
        /// <param name="timeout">Timeout defined in miliseconds.</param>
        public void BeginConnect(IPEndPoint server, int timeout)
        {
            ConnectionTimeout.Stop();

            if (globalSocket != null)
                globalSocket.Close();

            StartConnecting(server);
            ConnectionTimeout.Interval = timeout;
            ConnectionTimeout.Start();
        }

        /// <summary>
        /// Async method to connect to a host.
        /// </summary>
        /// <param name="ip">The IP address of the host.</param>
        /// <param name="port">The port of the host.</param>
        /// <param name="timeout">Timeout defined in miliseconds.</param>
        public void BeginConnect(string ip, int port, int timeout)
        {
            BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), timeout);
        }

        private void StartConnecting(IPEndPoint server)
        {
            globalSocket = new Socket(server.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            globalSocket.BeginConnect(server, OnConnect, globalSocket);
        }

        private void ConnectionTimeout_Tick(object sender, EventArgs e)
        {
            ConnectionTimeout.Stop();
            globalSocket.Close();
            OnConnectionTimeout?.Invoke(this, null);
        }

        private void OnConnect(IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;
            OnConnect(socket);
        }


        private void OnConnect(Socket socket)
        {
            ConnectionTimeout?.Stop();
            try
            {
                if (DoKeepAlive)
                    socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);

                RemoteEndPoint = globalSocket.RemoteEndPoint as IPEndPoint;

                socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, SocketFlags.None, OnRecieve, socket);
                OnConnected?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                socket.Close();
            }
        }

        /// <summary>
        /// Async method to close the connection.
        /// </summary>
        public void BeginDisconnect()
        {
            if (globalSocket != null)
                if (globalSocket.Connected)
                    globalSocket.Shutdown(SocketShutdown.Send);     //Shutdown the socket, the recieve event will raise and 0 bytes will be read.
        }


        //--------------------------------------------------------------------------------
        //                      Sending and recieving data
        //--------------------------------------------------------------------------------
        
        
        /// <summary>
        /// Method to send data.
        /// </summary>
        /// <param name="data">The data to be send</param>
        /// <returns>Number of bytes that where send.</returns>
        public int SendDataSync(byte[] data)
        {
            return globalSocket.Send(data);
        }


        private void OnRecieve(IAsyncResult ar)
        {
            SocketError sockError;
            Socket socket = ar.AsyncState as Socket;

                int bytesRead = socket.EndReceive(ar, out sockError);

                if (bytesRead == 0)     //This happens after a socket shutdown or a keep alive timeout
                {

                    socket.Close();     //Dispose the socket               
                    OnDisconnected?.Invoke(this, null);
                }
                else
                {
                    if (sockError == SocketError.Success)
                    {
                        byte[] data = new byte[bytesRead]; //Make a copy of the recieved data so we can continue recieving data.

                        Array.Copy(rxBuffer, data, bytesRead);
                        OnDataRecieved?.Invoke(this, data);
                        socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, SocketFlags.None, OnRecieve, socket);
                    }
                    else    //Some error occured, Close socket and raise disconnect event
                    {
                        socket.Close();
                        throw new NotImplementedException();
                    }
                }
        }
    }
}
