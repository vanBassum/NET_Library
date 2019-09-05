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
    public class TcpSocketClient
    {
        Socket globalSocket;

        Timer ConnectionTimeout;
        byte[] rxBuffer = new byte[1024];
        byte[] inOptionValues;

        bool DoKeepAlive = false;

        public delegate void OnDataRecievedHandler(object sender, byte[] data);
        public event OnDataRecievedHandler OnDataRecieved;

        public event EventHandler OnConnected;
        public event EventHandler OnConnectionFailed;
        public event EventHandler OnConnectionTimeout;
        public event EventHandler OnDisconnected;

        public IPEndPoint LocalEndPoint { get { return globalSocket.LocalEndPoint as IPEndPoint; } }
        public IPEndPoint RemoteEndPoint { get { return globalSocket.RemoteEndPoint as IPEndPoint; } }
        public bool IsConnected { get { return globalSocket==null?false:globalSocket.Connected; } }


        //--------------------------------------------------------------------------------
        //                          Constructor and destructor
        //--------------------------------------------------------------------------------
        public TcpSocketClient()
        {
            ConnectionTimeout = new Timer();
            ConnectionTimeout.Tick += ConnectionTimeout_Tick;
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
            //globalSocket.Shutdown(SocketShutdown.Send);     //Shutdown the socket, the recieve event will raise and 0 bytes will be read.

            ConnectionTimeout.Stop();
            //globalSocket.Shutdown(SocketShutdown.Both);
            globalSocket.Close();

            //System.Threading.SynchronizationContext context = System.Threading.SynchronizationContext.Current ?? new System.Threading.SynchronizationContext();
            //context.Send(s =>
            //{
                OnConnectionTimeout?.Invoke(this, null);
            //}, null);
        }

        private void OnConnect(IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;
            OnConnect(socket);
        }


        private void OnConnect(Socket socket)
        {
            ConnectionTimeout?.Stop();
            //System.Threading.SynchronizationContext context = System.Threading.SynchronizationContext.Current ?? new System.Threading.SynchronizationContext();

            try
            {

                if (DoKeepAlive)
                    socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);

                socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, SocketFlags.None, OnRecieve, socket);

                //context.Send(s =>
                //{
                    OnConnected?.Invoke(this, null);
               // }, null);
            }
            catch (Exception ex)
            {
                socket.Close();

                if (!((ex is ObjectDisposedException)
                    || (ex is NullReferenceException)
                    ))
                {
                   // context.Send(s =>
                    //{
                        OnConnectionFailed?.Invoke(this, null);
                    //}, null);
                }
            }
        }

        public void BeginDisconnect()
        {
            if(globalSocket!=null)
                if(globalSocket.Connected)
                    globalSocket.Shutdown(SocketShutdown.Send);     //Shutdown the socket, the recieve event will raise and 0 bytes will be read.
        }

        public void SetTcpKeepAlive(bool enabled, uint retryInterval, uint keepAliveInterval)
        {
            //ServicePointManager.SetTcpKeepAlive();
            uint dummy = 0;
            inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes(enabled).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes(retryInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes(keepAliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);
            DoKeepAlive = true;

        }


        //--------------------------------------------------------------------------------
        //                      Sending and recieving data
        //--------------------------------------------------------------------------------
        /*
        public bool SendDataAsync(byte[] data)
        {
            System.Threading.SynchronizationContext context = System.Threading.SynchronizationContext.Current ?? new System.Threading.SynchronizationContext();

            try
            {
                globalSocket.Send(data);
                return true;
            }
            catch (Exception ex)           //An error occurred when attempting to access the socket.
            {
                context.Send(s =>
                {
                    //OnDisconnected?.Invoke(this, null);
                }, null);
                throw new NotImplementedException(ex.Message);
            }

            //return false;
        }
        */


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
            //System.Threading.SynchronizationContext context = System.Threading.SynchronizationContext.Current ?? new System.Threading.SynchronizationContext();
            SocketError sockError;
            Socket socket = ar.AsyncState as Socket;

            //try
            {
                int bytesRead = socket.EndReceive(ar, out sockError);

                if (bytesRead == 0)     //This happens after a socket shutdown or a keep alive timeout
                {
                    /*
                    if (sockError == SocketError.Success)           //Sucessfully closed the connection, all data left in the buffers have been processed, and no bytes where recieved.
                    {

                    }
                    else if (sockError == SocketError.TimedOut)     //Keepalive timeout
                    {

                    }
                    */

                    socket.Close();     //Dispose the socket
                    //context.Send(s =>
                    //{
                        OnDisconnected?.Invoke(this, null);
                    //}, null);

                }
                else
                {
                    if (sockError == SocketError.Success)
                    {
                        byte[] data = new byte[bytesRead]; //Make a copy of the recieved data so we can continue recieving data.

                        Array.Copy(rxBuffer, data, bytesRead);
                        DataRecieved(data);


                        //context.Send(s =>
                        //{

                        //OnDataRecieved?.Invoke(data);
                        //}, null);

                        //Continue to recieve data
                        socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, SocketFlags.None, OnRecieve, socket);
                    }
                    else    //Some error occured, Close socket and raise disconnect event
                    {
                        socket.Close();
                        //context.Send(s =>
                        //{
                        //OnDisconnected?.Invoke(new Exception(sockError.ToString()));
                        //}, null);
                        throw new NotImplementedException();
                    }
                }

            }
            //catch (Exception ex)
            //{
            //    socket.Close();
            //    
            //    context.Send(s =>
            //    {
            //
            //    }, null);
            //    throw new NotImplementedException();
            //}

        }

        public virtual void DataRecieved(byte[] data)
        {
            OnDataRecieved?.Invoke(this, data);

        }
        
    }
}