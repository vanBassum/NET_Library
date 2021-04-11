using STDLib.Misc;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;



namespace STDLib.JBVProtocol
{
    

    public class JBVClient
    {
        IConnection _Connection = null;
        public IConnection Connection { get => _Connection; set => SetConnection(value); }
        public event EventHandler<Frame> FrameRecieved;
        SoftwareID softwareID = SoftwareID.Unknown;
        Lease lease = new Lease();
        Task task;
        Framing framing;
        BlockingCollection<Frame> pendingFrames = new BlockingCollection<Frame>();

        ConcurrentDictionary<UInt16, TaskCompletionSource<Frame>> pending = new ConcurrentDictionary<UInt16, TaskCompletionSource<Frame>>();

        void SetConnection(IConnection connection)
        {
            _Connection = connection;
            _Connection.OnDataRecieved += (sender, data) =>
            {
                framing.Unstuff(data);
            };
        }

        
        public JBVClient(SoftwareID softId)
        {
            softwareID = softId;
            framing = new Framing();
            framing.OnFrameCollected += (sender, frame) => pendingFrames.Add(frame);
            task = new Task(Work);
            task.Start();
        }

        public JBVClient(SoftwareID softId, UInt16 staticID)
        {
            softwareID = softId;
            framing = new Framing();
            framing.OnFrameCollected += (sender, frame) => pendingFrames.Add(frame);
            lease = new Lease() { ID = staticID, Expire = DateTime.MaxValue };
            task = new Task(Work);
            task.Start();
        }



        void Work()
        {
            AutoResetEvent leaseExpire = new AutoResetEvent(false);

            while (true)
            {
                if (this.lease.ExpiresIn.TotalMinutes < 5)
                {
                    //RequestLease();
                }

                Frame frame;

                while (pendingFrames.TryTake(out frame, 1000))
                {
                    TaskCompletionSource<Frame> f;
                    if (pending.TryGetValue(frame.Sequence, out f))
                        f.SetResult(frame);
                    else
                    {
                        if(frame.Type == Frame.FrameTypes.ProtocolFrame)
                        {
                            //
                        }
                        else
                        {
                            FrameRecieved?.Invoke(this, frame);
                        }
                    }
                }
            }
        }

        public bool SendFrame(Frame frame, TLOGGER log)
        {
            frame.TxID = lease.ID;
            if (Connection != null)
            {
                if(Connection.ConnectionStatus == ConnectionStatus.Connected)
                {
                    Connection.SendData(framing.Stuff(frame));
                    return true;
                }
                else
                    log.LOGE("Connection invalid " + Connection.ConnectionStatus.ToString());
            }
            else
                log.LOGE("No connection");
                

            return false;
        }


        public async Task<Frame> SendFrameAndWaitForReply(Frame txFrame, TLOGGER log, CancellationToken ct)
        {
            UInt16 seq = 0;
            Stopwatch stopWatch = new Stopwatch();
            
            for (seq = 0; seq < 0xFFFF; seq++)
            {
                if (!pending.ContainsKey(seq))
                    break;
            }

            if (seq == 0xFFFF)
            {
                log.LOGE("No more free sequence id's.");
            }
            else
            {
                txFrame.Sequence = seq;
                TaskCompletionSource<Frame> tcs = new TaskCompletionSource<Frame>();
                ct.Register(() => { tcs.SetCanceled(); });
                stopWatch.Start();
                if (SendFrame(txFrame, log))
                {
                    pending.TryAdd(seq, tcs);
                    try
                    {
                        Frame rx = await tcs.Task;
                        stopWatch.Stop();
                        log.LOGD($"{stopWatch.ElapsedMilliseconds}ms");
                        return rx;
                    }
                    catch (TaskCanceledException)
                    {
                        log.LOGE("Task cancelled");
                    }
                    finally
                    {
                        pending.TryRemove(seq, out tcs);
                    }
                }
                
            }
            return null;
        }
    }
}


