﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDLib.JBVProtocol.IO
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
        private const byte MaxHop = 32;
        Task routerTask;
        //Contains all active connections to this router.
        List<Connection> connections = new List<Connection>();
        //Contains all information how to reroute data
        List<Route> routingTable = new List<Route>();

        public UInt16 ID { get; set; } = 0;


        public Router(UInt16 id)
        {
            ID = id;
            routerTask = new Task(DoRouting);
            routerTask.Start();
        }

        /// <summary>
        /// Add a connection to this router.
        /// </summary>
        /// <param name="connection"></param>
        public void AddConnection(Connection connection)
        {
            lock(connections)
                connections.Add(connection);
            connection.OnFrameReceived += Connection_OnFrameReceived;
            connection.OnDisconnected += Connection_OnDisconnected;
        }

        

        private void Connection_OnDisconnected(object sender, EventArgs e)
        {
            if (sender is Connection con)
            {
                lock (routingTable)
                    routingTable.RemoveAll(r => r.con == con);
            }

            //@TODO: We don't inform the others. Its probably not nessesairy because the network will recover automatically.
        }




        BlockingCollection<Tuple<Connection, Frame>> frameBuffer = new BlockingCollection<Tuple<Connection, Frame>> ();
        List<Frame> unknownRouteFrames = new List<Frame>();//Frames that coulnt be rerouted are stored here untill further notice.

        private void Connection_OnFrameReceived(object sender, Frame e)
        {
            if (sender is Connection con)
                frameBuffer.Add(new Tuple<Connection, Frame>(con, e));
        }

        private void DoRouting()
        {
            //@TODO: CancellationToken something something...
            while(true)
            {
                Tuple<Connection, Frame> item = frameBuffer.Take();
                Connection rxCon = item.Item1;
                Frame rxFrame = item.Item2;

                rxFrame.HOP++;
                if (rxFrame.HOP <= MaxHop)
                {
                    //@TODO: Maybe solve this by using a threadsafe collection???
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
        }



        void HandleBroadcast(Connection rxCon, Frame rxFrame)
        {
            //First, check if we know this route already.
            Route rxRoute = routingTable.FirstOrDefault(r => r.id == rxFrame.SID);

            if(rxRoute == null)
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

            if (rxFrame.RoutingInfo)
                HandleRoutingInfo(rxCon, rxFrame, rxRoute);


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

            if(txRoute == null)
            {

                lock (unknownRouteFrames)
                    unknownRouteFrames.Add(rxFrame);

                Frame txFrame = Frame.CreateRequestID(ID, rxFrame.RID);

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

        void HandleRoutingInfo(Connection rxCon, Frame rxFrame, Route newRoute)
        {
            lock (unknownRouteFrames)
            {
                List<Frame> resolvedFrames = unknownRouteFrames.Where(f => f.RID == rxFrame.SID).ToList();
                foreach (Frame f in resolvedFrames)
                {
                    newRoute.con.SendFrame(f);
                    unknownRouteFrames.Remove(f);
                }
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
            public UInt16 id;

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