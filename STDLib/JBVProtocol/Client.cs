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

        /// <summary>
        /// Our device id.
        /// </summary>
        public UInt16 ID { get; set; } = 0;

        /// <summary>
        /// Fires when a broadcast has been recieved.
        /// </summary>
        public event EventHandler<Message> OnBroadcastRecieved;

        /// <summary>
        /// Fires when a message has been recieved.
        /// </summary>
        public event EventHandler<Message> OnMessageRecieved;


        public Client(UInt16 id)
        {
            this.ID = id;
        }

        public void SetConnection(Connection con)
        {
            connection = con;
            connection.OnFrameReceived += Connection_OnFrameReceived;
        }

        /// <summary>
        /// Method to send a request frame
        /// </summary>
        /// <param name="RID">The ID of the receiving party.</param>
        /// <param name="payload">The data to be send</param>
        public void SendMessage(byte RID, byte[] payload)
        {
            Frame frame = Frame.CreateMessageFrame(ID, RID, payload);
            connection.SendFrame(frame);
        }

        /// <summary>
        /// Method to send a message to all connected clients when a server is used.
        /// </summary>
        /// <param name="payload">Data to be send</param>
        public void SendBroadcast(byte[] payload)
        {
            Frame frame = Frame.CreateBroadcastFrame(ID, payload);
            connection.SendFrame(frame);
        }

        private void Connection_OnFrameReceived(object sender, Frame e)
        {
            Connection c = sender as Connection;

            if(e.Broadcast)
            {
                OnBroadcastRecieved?.Invoke(this, new Message(e));
            }
            else
            {
                OnMessageRecieved?.Invoke(this, new Message(e));
            }
        }       
    }
}
