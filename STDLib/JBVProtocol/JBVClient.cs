using STDLib.JBVProtocol.IO;
using STDLib.JBVProtocol.IO.CMD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
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
        /// <summary>
        /// Our device id.
        /// </summary>
        public UInt16 ID { get { return lease.ID; } }

        /// <summary>
        /// Fires when a request command has been recieved.
        /// </summary>
        public event EventHandler<CMD> OnRequestRecieved;

        /// <summary>
        /// Fires when a reply command has been recieved.
        /// </summary>
        public event EventHandler<CMD> OnReplyRecieved;

        /// <summary>
        /// Returns wether the client has a valid lease
        /// </summary>
        public bool HasLease { get { return lease.Expire > DateTime.Now; } }

        public SoftwareID SoftwareID { get; set; } = SoftwareID.Unknown;

        public JBVClient(SoftwareID softwareID)
        {
            SoftwareID = softwareID;
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

        public void Send(CMD cmd)
        {
            Frame tx = cmd.GetFrame();
            tx.SID = ID;
            SendFrame(tx);
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
                            lease = cmd.Lease;
                        break;
                }
            }
            else
            {
                CMD cmd = CMD.FromFrame(e);
                if (cmd.IsRequest)
                    OnRequestRecieved?.Invoke(this, cmd);
                else 
                    OnReplyRecieved?.Invoke(this, cmd);
            }
        }
    }
}
