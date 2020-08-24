using STDLib.JBVProtocol.IO;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace STDLib.JBVProtocol
{


    /// <summary>
    /// Manages connections with other devices and routes incomming and outgoing data to the right connection.
    /// </summary>
    public class Client
    {
        Connection connection;
        Lease lease = new Lease();

        /// <summary>
        /// Our device id.
        /// </summary>
        public UInt16 ID { get { return lease.ID; } }

        /// <summary>
        /// Fires when a broadcast has been recieved.
        /// </summary>
        public event EventHandler<Frame> OnBroadcastRecieved;

        /// <summary>
        /// Fires when a message has been recieved.
        /// </summary>
        public event EventHandler<Frame> OnMessageRecieved;


        public Client(UInt16 id = 0)
        {
            lease.ID = id;
            lease.Key = Guid.NewGuid();
        }

        public void SetConnection(Connection con)
        {
            connection = con;
            connection.OnFrameReceived += Connection_OnFrameReceived;
        }

        public void RequestLease()
        {
            Frame frame = Frame.CreateBroadcastFrame(ID, Encoding.ASCII.GetBytes(lease.Key.ToString()));
            frame.IDInfo = true;
            SendFrame(frame);
        }

        /// <summary>
        /// Method to send a request frame
        /// </summary>
        /// <param name="RID">The ID of the receiving party.</param>
        /// <param name="payload">The data to be send</param>
        public void SendMessage(byte RID, byte[] payload)
        {
            Frame frame = Frame.CreateMessageFrame(ID, RID, payload);
            SendFrame(frame);
        }

        /// <summary>
        /// Method to send a message to all connected clients when a server is used.
        /// </summary>
        /// <param name="payload">Data to be send</param>
        public void SendBroadcast(byte[] payload)
        {
            Frame frame = Frame.CreateBroadcastFrame(ID, payload);
            SendFrame(frame);
        }

        private void SendFrame(Frame frame)
        {
            if (frame.IDInfo)
            {
                connection.SendFrame(frame);
            }
            else
            {
                if (lease.Expire != null)
                {
                    if (lease.Expire > DateTime.Now)
                    {
                        //We have a valid lease.
                        connection.SendFrame(frame);
                    }
                }
            }
            throw new Exception("No valid lease");
        }

        private void Connection_OnFrameReceived(object sender, Frame e)
        {
            Connection c = sender as Connection;

            if(e.RoutingInfo)
            {
                if(e.RID == ID)
                {
                    Frame tx = Frame.CreateReplyToRequestID(ID);
                    c.SendFrame(tx);
                }
            }
            else if(e.IDInfo)
            {
                lease = Lease.FromString(Encoding.ASCII.GetString(e.PAY));
            }
            else
            {
                if (e.Broadcast)
                    OnBroadcastRecieved?.Invoke(this, e);
                else
                    OnMessageRecieved?.Invoke(this, e);
            }
        }       
    }
}
