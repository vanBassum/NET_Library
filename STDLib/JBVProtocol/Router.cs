using STDLib.JBVProtocol.IO;
using STDLib.JBVProtocol.IO.CMD;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
        Lease lease;
        private const byte MaxHop = 32;
        Task routerTask;
        //Contains all active connections to this router.
        List<Connection> connections = new List<Connection>();
        //Contains all information how to reroute data
        List<Route> routingTable = new List<Route>();
        System.Timers.Timer leaseTimer = new System.Timers.Timer();
        public ushort ID { get { return lease.ID; } }

        public Router(ushort id = 0)
        {
            lease = new Lease();
            lease.ID = id;
            lease.Key = Guid.NewGuid();
            routerTask = new Task(DoRouting);
            routerTask.Start();
            leaseTimer.Interval = 100;
            leaseTimer.Start();
            leaseTimer.Elapsed += LeaseTimer_Elapsed;
        }

        private void LeaseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
                    leaseTimer.Interval = expiresIn.TotalMinutes - 4;
                    //Next interval should be 4 minutes before expirering.
                }
            }
            else
            {
                RequestLease();
                leaseTimer.Interval *= 2;
                if (leaseTimer.Interval > 10000)
                    leaseTimer.Interval = 10000;
            }
        }



        /// <summary>
        /// Add a connection to this router.
        /// </summary>
        /// <param name="connection"></param>
        public void AddConnection(Connection connection)
        {
            lock (connections)
                connections.Add(connection);
            connection.OnFrameReceived += Connection_OnFrameReceived;
            connection.OnDisconnected += Connection_OnDisconnected;
        }

        private void RequestLease()
        {
            CMD_RequestLease cmd = new CMD_RequestLease(lease.Key);
            Frame frame = cmd.CreateCommandFrame(ID);
            lock (connections)
            {
                foreach (Connection txCon in connections)
                    txCon.SendFrame(frame);
            }
        }

        private void Connection_OnDisconnected(object sender, EventArgs e)
        {
            if (sender is Connection con)
            {
                lock (routingTable)
                    routingTable.RemoveAll(r => r.con == con);
            }

            //@TODO: We don't inform the others. Its probably not nessesary because the network will recover automatically.
        }




        BlockingCollection<Tuple<Connection, Frame>> frameBuffer = new BlockingCollection<Tuple<Connection, Frame>>();
        List<Frame> unknownRouteFrames = new List<Frame>();//Frames that coulnt be rerouted are stored here untill further notice.

        private void Connection_OnFrameReceived(object sender, Frame e)
        {
            if (sender is Connection con)
            {
                bool handleFrame = true;

                if (e.Command)
                {
                    BaseCommand bcmd = BaseCommand.GetCommand(e.PAY);

                    switch (bcmd)
                    {
                        case CMD_ReplyLease cmd:
                            if(cmd.Lease.Key == lease.Key)
                            {
                                lease = cmd.Lease;
                                handleFrame = false;
                            }
                            break;


                    }
                }

                if(handleFrame)
                    frameBuffer.Add(new Tuple<Connection, Frame>(con, e));
            }
        }

        

        private void DoRouting()
        {
            //@TODO: CancellationToken something something...
            while (true)
            {
                Tuple<Connection, Frame> item = frameBuffer.Take();
                Connection rxCon = item.Item1;
                Frame rxFrame = item.Item2;


                if (rxFrame.RID == ID || rxFrame.Broadcast)
                {
                    //Frame is addressed to us.
                    if(!rxFrame.Command)
                    {
                        CMD cmd = CMD.FromFrame(rxFrame);
                        if (cmd != null)
                        {
                            CMD txCmd = HandleOwnFrame(cmd, rxCon);
                            if (txCmd != null)
                            {
                                Frame txFrame = txCmd.ToFrame();
                                txFrame.SID = ID;
                                txFrame.RID = rxFrame.SID;
                                rxCon.SendFrame(txFrame);
                            }
                        }
                    }
                }

                if (rxFrame.RID != ID)
                {
                    rxFrame.HOP++;
                    if (rxFrame.HOP <= MaxHop)
                    {
                        lock (routingTable) //Do everything within the lock, we dont want the colletion to be changed by another process in the meanwhile.
                        {
                            if (rxFrame.Broadcast)
                            {
                                HandleBroadcast(rxCon, rxFrame);
                            }
                            else
                            {
                                HandleMessage(rxCon, rxFrame);
                            }
                        }
                    }
                    else
                    {
                        //TODO: Should we do something here???
                    }
                }
                Console.Write("A");
            }
        }

        CMD HandleOwnFrame(CMD cmd, Connection con)
        {
            switch(cmd)
            {
                case RequestSoftwareID rxcmd:
                    if(rxcmd.SoftwareID == SoftwareID.Unknown || rxcmd.SoftwareID == SoftwareID.Router)
                    {
                        ReplySoftwareID txcmd = new ReplySoftwareID();
                        txcmd.SoftwareID = SoftwareID.Router;
                        return txcmd;
                    }
                    break;
                default:
                    throw new NotImplementedException("Command not suported");
            }
            return null;
        }



        void HandleBroadcast(Connection rxCon, Frame rxFrame)
        {
            //First, check if we know this route already.
            Route rxRoute = routingTable.FirstOrDefault(r => r.id == rxFrame.SID);

            if (rxRoute == null)
            {
                rxRoute = new Route();
                rxRoute.con = rxCon;
                rxRoute.hops = rxFrame.HOP;
                rxRoute.id = rxFrame.SID;
                routingTable.Add(rxRoute);
            }

            if (rxFrame.HOP < rxRoute.hops)
            {
                //A better shorter route was found, update the route.
                rxRoute.con = rxCon;
                rxRoute.hops = rxFrame.HOP;
            }

            if (rxFrame.HOP == rxRoute.hops)
            {
                //Resend the broadcast.
                lock (connections)
                {
                    foreach (Connection txCon in connections.Where(r => r != rxCon))
                        txCon.SendFrame(rxFrame);
                }
            }

            //if (rxFrame.HOP > rxRoute.hops) => let the frame die.
        }

        void HandleMessage(Connection rxCon, Frame rxFrame)
        {
            Route txRoute = routingTable.FirstOrDefault(r => r.id == rxFrame.RID);

            if (txRoute == null)
            {

                lock (unknownRouteFrames)
                    unknownRouteFrames.Add(rxFrame);

                CMD_RequestID cmd = new CMD_RequestID(rxFrame.RID);
                Frame txFrame = cmd.CreateCommandFrame(ID);

                lock (connections)
                {
                    foreach (Connection txCon in connections.Where(r => r != rxCon))
                        txCon.SendFrame(txFrame);
                }

                //@TODO Implement a timeout for the frames in the waitinglist 'unknownRouteFrames'
            }
            else
            {
                //route is known so send the frame to the next client.
                txRoute.con.SendFrame(rxFrame);
            }
        }



        /// <summary>
        /// Entry in the routing table, it contains information how to access a client within a certain ammount of hops.
        /// </summary>
        public class Route
        {
            /// <summary>
            /// The ID of the client
            /// </summary>
            public ushort id;

            /// <summary>
            /// Number of hops the client is removed from this router
            /// </summary>
            public byte hops;

            /// <summary>
            /// The connection to route the frame to.
            /// </summary>
            public Connection con;
        }
    }
    


}
