using STDLib.JBVProtocol.Devices;
using STDLib.Misc;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;



namespace STDLib.JBVProtocol
{

    public class JBVClient
    {

        public IConnection Connection { get; private set; }
        public event EventHandler<Frame> FrameRecieved;
        public event EventHandler<Lease> LeaseRecieved;
        public event EventHandler<Device> OnDeviceFound;
        SoftwareID softwareID = SoftwareID.Unknown;
        Lease lease = new Lease();
        Task task;
        Framing framing;
        BlockingCollection<Frame> pendingFrames = new BlockingCollection<Frame>();

        
        public JBVClient(SoftwareID softId)
        {
            softwareID = softId;
            framing = new Framing();
            framing.OnFrameCollected += (sender, frame) => pendingFrames.Add(frame);

            task = new Task(Work);
            task.Start();
            SetConnection(new DummyConnection());
        }

        public JBVClient(SoftwareID softId, UInt16 staticID)
        {
            softwareID = softId;
            framing = new Framing();
            framing.OnFrameCollected += (sender, frame) => pendingFrames.Add(frame);
            lease = new Lease() { ID = staticID, Expire = DateTime.MaxValue };
            task = new Task(Work);
            task.Start();
            SetConnection(new DummyConnection());
        }

        public void SetConnection(IConnection con)
        {
            Connection = con;
            con.OnDataRecieved += (sender, data) =>
            {
                framing.Unstuff(data);
            };


        }

        void Work()
        {
            AutoResetEvent leaseExpire = new AutoResetEvent(false);

            while (true)
            {
                if (this.lease.ExpiresIn.TotalMinutes < 5)
                {
                    RequestLease();
                }

                Frame frame;

                while (pendingFrames.TryTake(out frame, 1000))
                {
                    /*
                    if (Enum.IsDefined(typeof(CommandList), frame.CommandID))
                    {

                    }
                    */

                    CommandList cmd = (CommandList)frame.CommandID;

                    switch (cmd)
                    {
                        case CommandList.RequestID:
                            UInt16 id = BitConverter.ToUInt16(frame.Data, 0);
                            if (id == this.lease.ID)
                            {
                                Frame f = Frame.ReplyID(this.lease.ID);
                                SendFrame(f);
                            }
                            break;
                        case CommandList.RequestSID:
                            SoftwareID sid = (SoftwareID)BitConverter.ToUInt32(frame.Data, 0);
                            if (sid == SoftwareID.Unknown || sid == softwareID)
                            {
                                Frame f = Frame.ReplySID(this.softwareID);
                                SendFrame(f);
                            }
                            break;
                        case CommandList.ReplyLease:
                            Lease rxLease = new Lease(frame.Data);
                            if (rxLease.Key == this.lease.Key)
                            {
                                this.lease = rxLease;
                                Logger.LOGI($"Lease acquired");
                                LeaseRecieved?.Invoke(this, lease);
                            }
                            break;
                        //case CommandList.RequestLease:
                        //    break;
                        //case CommandList.INVALID:
                        //    break;
                        //case CommandList.ReplyACK:
                        //    break;
                        case CommandList.ReplySID:
                            DeviceFound(frame);
                            break;
                        default:
                            FrameRecieved?.Invoke(this, frame);
                            break;
                    }
                }
            }
        }

        void DeviceFound(Frame frame)
        {
            SoftwareID id = (SoftwareID)BitConverter.ToUInt32(frame.Data, 0);
            switch (id)
            {
                case SoftwareID.FunctionGenerator:
                    OnDeviceFound?.Invoke(this, new FunctionGenerator(this, frame.TxID));
                    break;
                default:
                    break;
            }
        }

        void RequestLease()
        {
            Logger.LOGI(softwareID.ToString());
            Frame f = Frame.RequestLease(lease.Key);
            SendFrame(f);
        }

        public void SendFrame(Frame frame)
        {
            if (lease.IsValid || frame.Options.HasFlag(Frame.OPT.Broadcast))
            {
                frame.TxID = lease.ID;
                if (Connection != null)
                    Connection.SendData(framing.Stuff(frame));
                else
                    Logger.LOGE("No connection");
            }
        }

        
        Sequencer sequencer = new Sequencer();

        public async Task<Frame> SendRequest(Frame txFrame, CancellationToken? ct = null)
        {
            UInt16 sequence = sequencer.RequestSequenceID();

            TaskCompletionSource<Frame> tcs = new TaskCompletionSource<Frame>();
            ct?.Register(() =>
            {
                sequencer.FreeSequenceID(sequence);
                tcs.TrySetResult(null);
            });

            FrameRecieved += (sender, rxFrame) =>
            {
                if (rxFrame.Sequence == sequence)
                {
                    sequencer.FreeSequenceID(sequence);
                    tcs.TrySetResult(rxFrame);
                }
            };

            txFrame.Sequence = sequence;
            SendFrame(txFrame);

            return await tcs.Task;
        }

        public void SearchDevices(SoftwareID sid = SoftwareID.Unknown)
        {
            Frame f = new Frame();
            f.CommandID = (UInt32) CommandList.RequestSID;
            f.RxID = 0;
            f.SetData(BitConverter.GetBytes((UInt32)sid));
            f.Options |= Frame.OPT.Broadcast;
            SendFrame(f);
        }
    }
}


