using STDLib.JBVProtocol.Commands;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;


namespace STDLib.JBVProtocol
{
    public class LeaseServer
    {
        SoftwareID softwareID = SoftwareID.LeaseServer;
        Lease lease = new Lease();
        ConcurrentDictionary<Guid, Lease> leases = new ConcurrentDictionary<Guid, Lease>();
        Framing framing = new Framing();
        IConnection connection;
        Task task;
        BlockingCollection<Frame> pendingFrames = new BlockingCollection<Frame>();

        public LeaseServer()
        {
            lease.ID = 0;                       //The leaseserver is always at id 0.
            lease.Expire = DateTime.MaxValue;   //The leaseserver's lease never expires.
            leases[lease.Key] = lease;

            framing.OnFrameCollected += (sender, frame) => pendingFrames.Add(frame);

            task = new Task(Work);
            task.Start();
        }

        public void SetConnection(IConnection con)
        {
            connection = con;
            connection.OnDataRecieved += (sender, data) => framing.Unstuff(data);
        }

        void Work()
        {
            while(true)
            {
                Frame frame = pendingFrames.Take();
                if(frame != null)
                {
                    Command gcmd = Command.Create(frame);
                    if (gcmd == null)
                    {
                        Logger.LOGE($"Command not found '{frame.CommandID}'");
                    }
                    else
                    {
                        switch (gcmd)
                        {
                            case RequestLease cmd:
                                HandleRequestLease(cmd);
                                break;
                            case RequestID cmd:
                                if (cmd.ID == lease.ID)
                                    ReplyID();
                                break;
                            case RequestSID cmd:
                                if (cmd.SID == SoftwareID.Unknown || cmd.SID == softwareID)
                                    ReplySID(cmd);
                                break;
                            case ReplyLease cmd:
                                break;
                            default:
                                Logger.LOGW($"Command not assigned '{gcmd.CommandID}'");
                                break;
                        }
                    }
                }
            }
        }


        public void HandleRequestLease(RequestLease cmd)
        {
            Lease lease = null;
            if(leases.TryGetValue(cmd.Key, out lease))
            {
                //Extend lease
                lease.Expire = DateTime.Now.AddMinutes(10);
            }
            else
            {
                lease = new Lease();
                lease.Expire = DateTime.Now.AddMinutes(10);
                lease.Key = cmd.Key;

                //TODO: Reuse expired leases.
                //TODO: Optimize this!
                UInt16 id = 0;
                while (leases.Any(a => a.Value.ID == id))
                    id++;
                lease.ID = id;

                leases[lease.Key] = lease;
            }
            ReplyLease(lease);
            Logger.LOGI($"New lease accepted {lease.ToString()}");
        }


        void ReplyLease(Lease lease)
        {
            ReplyLease cmd = new ReplyLease();
            cmd.Lease = lease;
            SendCMD(cmd);
        }

        void ReplyID()
        {
            ReplyID cmd = new ReplyID();
            cmd.ID = lease.ID;
            SendCMD(cmd);
        }

        void ReplySID(Command rxCmd)
        {
            ReplySID cmd = new ReplySID();
            cmd.RxID = rxCmd.TxID;
            cmd.SID = softwareID;
            SendCMD(cmd);
        }

        public void SendCMD(Command cmd)
        {
            Frame frame = cmd.GetFrame();
            SendFrame(frame);
        }

        public void SendFrame(Frame frame)
        {
            frame.TxID = lease.ID;
            if (connection != null)
                connection.SendData(framing.Stuff(frame));
            else
                Logger.LOGE("No connection");
        }

    }

}