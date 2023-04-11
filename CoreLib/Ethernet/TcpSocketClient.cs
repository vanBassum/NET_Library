using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLib.Ethernet
{
    public class TcpSocketClient
    {
        Socket globalSocket;
        private IPEndPoint remoteEndPoint;
        private ConnectionStates connectionState;
        readonly byte[] rxBuffer = new byte[1024];
        //readonly byte[] inOptionValues;

        /// <summary>
        /// Fires when data is recieved by the socket.
        /// </summary>
        public event EventHandler<byte[]> OnDataRecieved;

        public event EventHandler<IPEndPoint> OnRemoteEndPointChanged;
        public event EventHandler<ConnectionStates> OnConnectionStateChanged;

        /// <summary>
        /// The IP endpoint of the client to witch the socket is connected.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get => remoteEndPoint; set { remoteEndPoint = value; OnRemoteEndPointChanged?.Invoke(this, value); } }

        /// <summary>
        /// Tells the current connection status.
        /// </summary>
        public ConnectionStates ConnectionState { get => connectionState; set { connectionState = value; OnConnectionStateChanged?.Invoke(this, value); } }

        #region Constructor and destructor
        /// <summary>
        /// Creates a new client.
        /// </summary>
        public TcpSocketClient()
        {
        }

        /// <summary>
        /// Creates a new client based upon an existing socket.
        /// </summary>
        /// <param name="s">The socket to be used by this client.</param>
        public TcpSocketClient(Socket s)
        {
            globalSocket = s;
            HandleNewConnection(globalSocket);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~TcpSocketClient()
        {
            if (globalSocket != null)
                globalSocket.Close();
        }
        #endregion


        #region Connection setup


        public async Task<bool> ConnectAsync(string host, CancellationToken token = default)
        {
            IPEndPoint ep = DNSExt.Parse(host);
            return await ConnectAsync(ep, token);
        }

        public async Task<bool> ConnectAsync(string host, int port, CancellationToken token = default)
        {
            IPEndPoint ep = DNSExt.Parse(host, port);
            return await ConnectAsync(ep, token);
        }

        public async Task<bool> ConnectAsync(IPEndPoint ep, CancellationToken token = default)
        {
            ConnectionState = ConnectionStates.Connecting;
            TaskCompletionSource<Socket> taskCompletionSource = new TaskCompletionSource<Socket>();

            if (globalSocket != null)
                globalSocket.Close();

            globalSocket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            token.Register(() =>
            {
                taskCompletionSource.TrySetCanceled();
            });

            bool result = false;
            try
            {
                globalSocket.BeginConnect(ep, (res) => taskCompletionSource.SetResult(res.AsyncState as Socket), globalSocket);
                Socket sock = await taskCompletionSource.Task;
                result = HandleNewConnection(sock);
                ConnectionState = ConnectionStates.Connected;
            }
            catch (OperationCanceledException ex)
            {
                ConnectionState = ConnectionStates.Canceled;
                result = false;
            }
            catch (Exception ex)
            {
                ConnectionState = ConnectionStates.Error;
                result = false;
            }
            return result;
        }


        private bool HandleNewConnection(Socket socket)
        {
            bool result = false;
            try
            {
                RemoteEndPoint = globalSocket.RemoteEndPoint as IPEndPoint;
                socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, SocketFlags.None, OnRecieve, socket);
                result = true;
            }
            catch (Exception ex)
            {
                socket.Close();
            }
            return result;
        }

        /// <summary>
        /// Method to close the connection.
        /// </summary>
        public void Disconnect()
        {
            if (globalSocket != null)
                if (globalSocket.Connected)
                {
                    globalSocket.Shutdown(SocketShutdown.Send);     //Shutdown the socket, the recieve event will raise and 0 bytes will be read.
                    globalSocket.Close();
                }
            ConnectionState = ConnectionStates.Disconnected;
        }

        #endregion


        #region Sending and recieving data

        /// <summary>
        /// Method to send data.
        /// </summary>
        /// <param name="data">The data to be send</param>
        /// <returns>Number of bytes that where send.</returns>
        public int SendDataSync(byte[] data)
        {
            try
            {
                if (globalSocket != null)
                    if (globalSocket.Connected)
                        return globalSocket.Send(data);
            }
            catch (Exception ex)
            {

            }
            return 0;
        }


        private void OnRecieve(IAsyncResult ar)
        {
            SocketError sockError;
            Socket socket = ar.AsyncState as Socket;
            try
            {
                int bytesRead = socket.EndReceive(ar, out sockError);

                if (bytesRead == 0)     //This happens after a socket shutdown or a keep alive timeout
                {
                    socket.Close();     //Dispose the socket  
                    ConnectionState = ConnectionStates.Disconnected;
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
                        ConnectionState = ConnectionStates.Error;
                        throw new NotImplementedException();
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public bool SendData(byte[] data)
        {
            return data.Length == SendDataSync(data);
        }
        #endregion
    }

}
