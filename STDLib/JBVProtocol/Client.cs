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
        System.Timers.Timer leaseTimer = new System.Timers.Timer();
        SoftwareID softwareId = SoftwareID.Unknown;
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

        public event EventHandler<Frame> OnSoftwareIDRecieved;

        public Client(SoftwareID softID)
        {
            softwareId = softID;
            lease.Key = Guid.NewGuid();
            leaseTimer.Interval = 100;
            leaseTimer.Start();
            leaseTimer.Elapsed += LeaseTimer_Elapsed;
        }

        private void LeaseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (lease.Expire == null)
            {
                RequestLease();

                leaseTimer.Interval *= 2;
                if (leaseTimer.Interval > 10000)
                    leaseTimer.Interval = 10000;
            }
            else
            {
                if (lease.Expire > DateTime.Now)
                {
                    //Not expired
                    TimeSpan expiresIn = lease.Expire.Value - DateTime.Now;
                    if (expiresIn < TimeSpan.FromMinutes(5))
                    {
                        //Expires witin 5 minutes
                        RequestLease();
                    }
                    else
                    {
                        leaseTimer.Interval = expiresIn.TotalMinutes - 4;
                        //Next interval should be 4 minutes before expirering.
                    }
                }
            }
        }

        private void RequestLease()
        {
            Frame frame = Frame.CreateBroadcastFrame(ID, Encoding.ASCII.GetBytes(lease.Key.ToString()));
            frame.IDInfo = true;
            SendFrame(frame);
        }


        public void SetConnection(Connection con)
        {
            connection = con;
            connection.OnFrameReceived += Connection_OnFrameReceived;
            RequestLease();
        }

        

        /// <summary>
        /// Method to send a request frame
        /// </summary>
        /// <param name="RID">The ID of the receiving party.</param>
        /// <param name="payload">The data to be send</param>
        public void SendMessage(UInt16 RID, byte[] payload)
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

        /// <summary>
        /// Sends broadcast with the request for a specific softwareID
        /// </summary>
        /// <param name="softId"></param>
        public void RequestSoftwareID(SoftwareID softId)
        {
            Frame frame = Frame.RequestSpecificSoftwareID(ID, softId);
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
                        return;
                    }
                }
                throw new Exception("No valid lease");
            }
            
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
                Lease l;
                if (Lease.TryParse(Encoding.ASCII.GetString(e.PAY), out l))
                {
                    if(l.Key == lease.Key)
                        lease = l;
                }
            }
            else if(e.SIDInfo)
            {
                if(e.PAY[0] == 0)
                {
                    //When a request for sid is received
                    if (e.LEN == 5)
                    {
                        //Someone is asking for a device with a specific softwareid
                        SoftwareID searchID = (SoftwareID)BitConverter.ToUInt32(e.PAY, 1);
                        if (softwareId == searchID)
                        {
                            Frame reply = new Frame();
                            reply.SIDInfo = true;
                            reply.SID = lease.ID;
                            reply.RID = e.SID;
                            reply.PAY = BitConverter.GetBytes((UInt32)softwareId);
                            c.SendFrame(reply);
                        }
                    }
                    else
                    {
                        Frame reply = new Frame();
                        reply.SIDInfo = true;
                        reply.SID = lease.ID;
                        reply.RID = e.SID;
                        reply.PAY = BitConverter.GetBytes((UInt32)softwareId);
                        c.SendFrame(reply);
                    }
                }
                else
                {
                    OnSoftwareIDRecieved?.Invoke(this, e);
                }
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
