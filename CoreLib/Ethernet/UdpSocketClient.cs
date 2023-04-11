using CoreLib.Misc;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLib.Ethernet
{

    public class UdpSocketClient
    {
        public event EventHandler<byte[]> OnDataRecieved;
        public event PropertyChangedEventHandler PropertyChanged;
        readonly byte[] rxBuffer = new byte[1024];
        public ConnectionStates ConnectionStatus => throw new NotImplementedException();
        UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 51100));
        IPEndPoint endpoint;


        public bool SendData(byte[] data)
        {
            return client.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, 51100)) == data.Length;
        }

        public void Connect(IPEndPoint ep)
        {
            endpoint = ep;
            client.BeginReceive(new AsyncCallback(ReceiveCallback), this);
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            UdpSocketClient u = ((UdpSocketClient)(ar.AsyncState));

            byte[] receiveBytes = u.client.EndReceive(ar, ref u.endpoint);
            u.OnDataRecieved?.Invoke(u, receiveBytes);
            u.client.BeginReceive(new AsyncCallback(ReceiveCallback), u);
        }
    }


    /*
    public class UdpSocketClient : PropertySensitive, IConnection
    {
        Socket globalSocket;
        readonly byte[] rxBuffer = new byte[1024];
        //readonly byte[] inOptionValues;

        /// <summary>
        /// Fires when data is recieved by the socket.
        /// </summary>
        public event EventHandler<byte[]> OnDataRecieved;

        /// <summary>
        /// The IP endpoint of the client to witch the socket is connected.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get => GetPar<IPEndPoint>(); private set => SetPar<IPEndPoint>(value); }

        /// <summary>
        /// Tells the current connection status.
        /// </summary>
        public ConnectionStatus ConnectionStatus { get => GetPar<ConnectionStatus>(ConnectionStatus.Disconnected); private set => SetPar<ConnectionStatus>(value); }

        #region Constructor and destructor
        /// <summary>
        /// Creates a new client.
        /// </summary>
        public UdpSocketClient()
        {
        }


        /// <summary>
        /// Destructor.
        /// </summary>
        ~UdpSocketClient()
        {
            if (globalSocket != null)
                globalSocket.Close();
        }
        #endregion


        #region Connection setup


        public async Task<bool> ConnectAsync(string host, CancellationTokenSource cts = null)
        {
            IPEndPoint ep = DNSExt.Parse(host);
            return await ConnectAsync(ep, cts);
        }

        public async Task<bool> ConnectAsync(string host, int port, CancellationTokenSource cts = null)
        {
            IPEndPoint ep = DNSExt.Parse(host, port);
            return await ConnectAsync(ep, cts);
        }

        public async Task<bool> ConnectAsync(IPEndPoint ep, CancellationTokenSource cts = null)
        {
            ConnectionStatus = ConnectionStatus.Connecting;
            TaskCompletionSource<Socket> taskCompletionSource = new TaskCompletionSource<Socket>();

            if (globalSocket != null)
                globalSocket.Close();

            globalSocket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            if (cts == null)
                (cts = new CancellationTokenSource()).CancelAfter(2500);

            cts.Token.Register(() => { 
                taskCompletionSource.TrySetCanceled(); 
            });

            bool result = false;
            try
            {
                globalSocket.BeginConnect(ep, (res) => taskCompletionSource.SetResult(res.AsyncState as Socket), globalSocket);
                Socket sock = await taskCompletionSource.Task;
                result = HandleNewConnection(sock);
                ConnectionStatus = ConnectionStatus.Connected;
            }
            catch (OperationCanceledException ex)
            {
                ConnectionStatus = ConnectionStatus.Canceled;
                result = false;
            }
            catch (Exception ex)
            {
                ConnectionStatus = ConnectionStatus.Error;
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
                {
                    globalSocket.Shutdown(SocketShutdown.Send);     //Shutdown the socket, the recieve event will raise and 0 bytes will be read.
                    globalSocket.Close();
                }
            ConnectionStatus = ConnectionStatus.Disconnected;
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
            catch(Exception ex)
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
                    ConnectionStatus = ConnectionStatus.Disconnected;
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
                        ConnectionStatus = ConnectionStatus.Error;
                        throw new NotImplementedException();
                    }
                }
            }
            catch(Exception ex)
            {

            }
            
        }

        public bool SendData(byte[] data)
        {
            return data.Length == SendDataSync(data);
        }
        #endregion
    }
    */
}
