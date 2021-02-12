using STDLib.Misc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace STDLib.JBVProtocol
{
    /// <summary>
    /// The router acts as a node in the network accepting multiple connections.
    /// Its job is to re-route incomming data to the right client.
    /// A router has no ID in the network because it isn't a client.
    /// In order to reach the device containing the router a communicator has to be used.
    /// A internal connection should be created between the router and the communicator.
    /// </summary>
    public class Router
    {
        //Lease lease = new Lease();
        private const byte MaxHop = 32;
        Task routerTask;
        List<Con> connections = new List<Con>();
        ConcurrentDictionary<UInt16, Route> routingTable = new ConcurrentDictionary<UInt16, Route>();
        BlockingCollection<PendingFrame> pendingFrames = new BlockingCollection<PendingFrame>();

        class PendingFrame
        {
            public Frame Frame { get; set; }
            public int RetryCount { get; set; } = 0;

            public PendingFrame(Frame frame)
            {
                Frame = frame;
            }
        }


        public Router()
        {
            routerTask = new Task(DoRouting);
            routerTask.Start();
        }

        public void AddConnection(IConnection connection)
        {
            Con con = new Con(connection);
            con.OnFrameRecieved += Con_OnFrameRecieved;
            connections.Add(con);
        }

        private void Con_OnFrameRecieved(object sender, Frame frame)
        {
            frame.Hops++;
            //Routingtable update
            if (sender is Con connection)
            {
                Route route = null;

                if (!routingTable.TryGetValue(frame.TxID, out route))
                    routingTable[frame.TxID] = route = new Route { Connection = connection, Hops = frame.Hops };

                if (frame.Hops < route.Hops)
                {
                    //Faster route found, update routinginfo.
                    route.Connection = connection;
                    route.Hops = frame.Hops;
                }

                //if (frame.RxID == lease.ID)
                //{
                //    //Package only for us, dont resend!
                //    HandleFrame(frame);
                //}
                //else if (frame.Options.HasFlag(Frame.OPT.Broadcast))
                //{
                //    //Package also for us, do resend!
                //    if(frame.Hops == route.Hops)
                //    {
                //        HandleFrame(frame);
                //        pendingFrames.Add(frame);
                //    }
                //}
                //else
                //{
                //    //Package not for us, do resend!
                //    pendingFrames.Add(frame);
                //}

                pendingFrames.Add(new PendingFrame(frame));
            }
        }

        //void HandleFrame(Frame frame)
        //{
        //    //This frame should be handled by us.
        //    Command gcmd = Command.Create(frame);
        //    if (gcmd != null)
        //    {
        //        switch (gcmd)
        //        {
        //            case ReplyLease cmd:
        //                if (cmd.Lease.Key == lease.Key)
        //                {
        //                    lease = cmd.Lease;
        //                    Logger.LOGI($"Lease acquired");
        //                }
        //                break;
        //            default:
        //                Logger.LOGW($"Command not handled cmdid = '{gcmd.CommandID}'");
        //                break;
        //        }
        //    }
        //    else
        //        Logger.LOGE($"Couln't convert frame to command cmdid = '{frame.CommandID}'");
        //}

        private void DoRouting()
        {
            while (true)
            {

                //if (lease.ExpiresIn.TotalMinutes < 5)
                //{
                //    RequestLease();
                //}

                PendingFrame frame;

                while (pendingFrames.TryTake(out frame, 1000))
                {
                    if (frame.RetryCount < 3)
                        SendFrame(frame.Frame, frame.RetryCount);
                    else
                    {
                        switch ((CommandList)frame.Frame.CommandID)
                        {
                            case CommandList.RoutingInvalid:
                                Logger.LOGE($"Dropped RoutingInvalid frame, RetryCount = '{frame.RetryCount}'");
                                break;
                            default:
                                Logger.LOGE($"Dropped frame, RetryCount = '{frame.RetryCount}'");
                                Frame f = Frame.RoutingInvalid();
                                f.RxID = frame.Frame.TxID;
                                f.Sequence = frame.Frame.Sequence;
                                pendingFrames.Add(new PendingFrame(f));
                                break;
                        }
                    }
                }
            }
        }


        //void RequestLease()
        //{
        //    Logger.LOGI("RequestLease");
        //    RequestLease cmd = new RequestLease();
        //    cmd.Key = lease.Key;
        //    Frame frame = cmd.GetFrame();
        //    frame.RxID = 0;
        //    SendFrame(frame);
        //}

        void RequestID(UInt16 id)
        {
            Logger.LOGI("RequestID");
            Frame f = Frame.RequestID(id);
            f.RxID = 0;
            SendFrame(f);
        }

        void SendFrame(Frame frame, int retries = 0)
        {
            if (frame.Options.HasFlag(Frame.OPT.Broadcast))
            {
                //Send to all known connections.
                foreach (Con con in connections)
                {
                    con.SendFrame(frame);
                }
            }
            else
            {
                Route route = null;

                bool sucsess = false;

                if (routingTable.TryGetValue(frame.RxID, out route))
                {
                    sucsess = route.Connection.SendFrame(frame);
                    if(!sucsess)
                        routingTable.TryRemove(frame.RxID, out route);
                }

                if(!sucsess)
                {
                    //Unknown route.
                    RequestID(frame.RxID);
                    DoDelayed(() => pendingFrames.Add(new PendingFrame(frame) { RetryCount = retries + 1 }), 1000); //Retry in 1 second.
                }
            }
        }

        void DoDelayed(Action action, int delay)
        {
            new Task(() => { Thread.Sleep(delay); action.Invoke(); }).Start();
        }


        public class Con
        {
            public event EventHandler<Frame> OnFrameRecieved;
            IConnection Connection { get; set; }
            Framing Framing { get; set; } = new Framing();

            public Con(IConnection con)
            {
                Connection = con;
                Framing.OnFrameCollected += (a, b) => OnFrameRecieved?.Invoke(this, b);
                con.OnDataRecieved += (a, b) => Framing.Unstuff(b);
            }

            public bool SendFrame(Frame frame)
            {
                //Logger.LOGI($"Send frame { frame.ToString()}");
                return Connection.SendData(Framing.Stuff(frame));
            }
        }

        public class Route
        {
            public byte Hops { get; set; }
            public Con Connection { get; set; }
        }
    }
}