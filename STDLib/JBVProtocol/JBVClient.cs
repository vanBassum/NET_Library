using STDLib.JBVProtocol.IO;
using STDLib.JBVProtocol.IO.CMD;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class JBVClient
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

        /// <summary>
        /// Fires when the lease is accepted
        /// </summary>
        public event EventHandler OnLeaseAccepted;

        /// <summary>
        /// Fires as result of <see cref="RequestSoftwareID"/>
        /// </summary>
        public event EventHandler<Frame> OnSoftwareIDRecieved;

        /// <summary>
        /// Returns wether the client has a valid lease
        /// </summary>
        public bool HasLease { get { return lease.Expire > DateTime.Now; } }

        public JBVClient(SoftwareID softID)
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
                leaseTimer.Interval = 1000;
            }
            else
            {
                if (lease.Expire > DateTime.Now)
                {
                    //Not expired
                    TimeSpan expiresIn = lease.Expire - DateTime.Now;
                    if (expiresIn < TimeSpan.FromMinutes(5))
                    {
                        //Expires witin 5 minutes
                        RequestLease();
                    }
                    else
                    {
                        leaseTimer.Interval = expiresIn.TotalMinutes - 4;   //TODO uum times 60000?
                        //Next interval should be 4 minutes before expirering.
                    }
                }
            }
        }

        private void RequestLease()
        {
            CMD_RequestLease cmd = new CMD_RequestLease(lease.Key);
            Frame frame = cmd.CreateCommandFrame(ID);
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
            CMD_RequestSoftwareID cmd = new CMD_RequestSoftwareID(softId);
            Frame frame = cmd.CreateCommandFrame(ID);
            SendFrame(frame);
        }


        private void SendFrame(Frame frame)
        {
            if(frame.Command)   //Specifically when command is a request for a lease.
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

            if (e.Command)
            {
                BaseCommand bcmd = BaseCommand.GetCommand(e.PAY);

                switch (bcmd)
                {
                    case CMD_ReplyLease cmd:            //We have gotten a lease
                        if (cmd.Lease.Key == lease.Key)
                        {
                            lease = cmd.Lease;
                            OnLeaseAccepted?.Invoke(this, null);
                        }
                        break;

                    case CMD_RequestID cmd:             //Someone wants to know our ID
                        if(cmd.RequestedID == ID)
                        {
                            CMD_ReplyID rcmd = new CMD_ReplyID(ID);
                            c.SendFrame(rcmd.CreateCommandFrame(ID));
                        }
                        break;

                    case CMD_RequestSoftwareID cmd:      //Someone wants to know if we have software id xxx
                        if(cmd.Sid == softwareId)
                        {
                            CMD_ReplySoftwareID rcmd = new CMD_ReplySoftwareID(softwareId);
                            c.SendFrame(rcmd.CreateCommandFrame(ID));
                        }
                        break;

                    case CMD_ReplySoftwareID cmd:
                        OnSoftwareIDRecieved?.Invoke(this, e);
                        break;
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
