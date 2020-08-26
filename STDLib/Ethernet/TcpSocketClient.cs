using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace STDLib.Ethernet
{
    public class TcpSocketClient
    {
        Socket globalSocket;
        readonly byte[] rxBuffer = new byte[1024];
        //readonly byte[] inOptionValues;

        /// <summary>
        /// Fires when data is recieved by the socket.
        /// </summary>
        public event EventHandler<byte[]> OnDataRecieved;

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

        //--------------------------------------------------------------------------------
        //                              Connection setup
        //--------------------------------------------------------------------------------
        public async Task<bool> ConnectAsync(string host, CancellationTokenSource cts = null)
        {
            TaskCompletionSource<Socket> taskCompletionSource = new TaskCompletionSource<Socket>();
            IPEndPoint ep = DNSExt.Parse(host);

            if (globalSocket != null)
                globalSocket.Close();

            globalSocket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            if (cts == null)
                (cts = new CancellationTokenSource()).CancelAfter(2500);

            cts.Token.Register(() => { taskCompletionSource.TrySetCanceled(); });

            bool result = false;
            try
            {
                globalSocket.BeginConnect(ep, (res) => taskCompletionSource.SetResult(res.AsyncState as Socket), globalSocket);
                Socket sock = await taskCompletionSource.Task;
                result = HandleNewConnection(sock);
            }
            catch(Exception ex)
            {
                result = false;
            }
            finally
            {
                cts.Cancel();
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
            if (globalSocket != null)
                if (globalSocket.Connected)
                    return globalSocket.Send(data);
            return 0;
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
