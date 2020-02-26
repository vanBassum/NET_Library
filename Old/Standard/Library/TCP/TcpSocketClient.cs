using System;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace TCP
{
    public class TcpSocketClient
    {
        Socket globalSocket;

        Timer ConnectionTimeout;
        byte[] rxBuffer = new byte[1024];
        byte[] inOptionValues;

        bool DoKeepAlive = false;

        public event EventHandler<byte[]> OnDataRecieved;

        public event EventHandler OnConnected;
        public event EventHandler OnConnectionFailed;
        public event EventHandler OnConnectionTimeout;
        public event EventHandler OnDisconnected;

        //public IPEndPoint LocalEndPoint { get { return globalSocket.LocalEndPoint as IPEndPoint; } }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public bool IsConnected { get { return globalSocket == null ? false : globalSocket.Connected; } }


        //--------------------------------------------------------------------------------
        //                          Constructor and destructor
        //--------------------------------------------------------------------------------
        public TcpSocketClient()
        {
            ConnectionTimeout = new Timer();
            ConnectionTimeout.Elapsed += ConnectionTimeout_Tick;
        }

        public TcpSocketClient(Socket s)
        {
            globalSocket = s;
            OnConnect(globalSocket);
        }

        ~TcpSocketClient()
        {
            if (globalSocket != null)
                globalSocket.Close();
        }

        //--------------------------------------------------------------------------------
        //                              Connection setup
        //--------------------------------------------------------------------------------

        public void BeginConnect(IPEndPoint server, int timeout)
        {
            ConnectionTimeout.Stop();

            if (globalSocket != null)
                globalSocket.Close();

            StartConnecting(server);
            ConnectionTimeout.Interval = timeout;
            ConnectionTimeout.Start();
        }

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

                if (!((ex is ObjectDisposedException)
                    || (ex is NullReferenceException)
                    ))
                {
                    OnConnectionFailed?.Invoke(this, null);
                }
            }
        }

        public void BeginDisconnect()
        {
            if (globalSocket != null)
                if (globalSocket.Connected)
                    globalSocket.Shutdown(SocketShutdown.Send);     //Shutdown the socket, the recieve event will raise and 0 bytes will be read.
        }

        /*
        public void SetTcpKeepAlive(bool enabled, uint retryInterval, uint keepAliveInterval)
        {
            //ServicePointManager.SetTcpKeepAlive();
            uint dummy = 0;
            inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes(enabled).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes(retryInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes(keepAliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);
            DoKeepAlive = true;

        }*/


        //--------------------------------------------------------------------------------
        //                      Sending and recieving data
        //--------------------------------------------------------------------------------
        
        
        /// <summary>
        ///  
        /// </summary>
        /// <param name="data">The data to be send</param>
        /// <returns>true if number of send bytes equals the number of bytes in data</returns>
        public bool SendDataSync(byte[] data)
        {
            return (globalSocket.Send(data) == data.Length);
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
                        DataRecieved(data);
                        socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, SocketFlags.None, OnRecieve, socket);
                    }
                    else    //Some error occured, Close socket and raise disconnect event
                    {
                        socket.Close();
                        throw new NotImplementedException();
                    }
                }
        }

        public virtual void DataRecieved(byte[] data)
        {
            OnDataRecieved?.Invoke(this, data);
        }

    }
}
