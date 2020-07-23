using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        //Contains all active connections to this router.
        List<Connection> connections = new List<Connection>();

        //Contains all information how to reroute data
        List<Route> routingTable = new List<Route>();

        /// <summary>
        /// Maximum number of times a frame will be resend, if its reached this number it will die.
        /// </summary>
        public int MaxHop { get; set; } = 32;

        /// <summary>
        /// Add a connection to this router.
        /// </summary>
        /// <param name="connection"></param>
        public void AddConnection(Connection connection)
        {
            connections.Add(connection);
            connection.OnFrameReceived += Connection_OnFrameReceived;
            connection.OnDisconnected += Connection_OnDisconnected;
        }

        private void Connection_OnDisconnected(object sender, EventArgs e )
        {
            //Let the others know that we cant reach any devices that where routed via this connection

            List<Route> invalidRoutes = new List<Route>();

            lock (routingTable)
            {
                foreach (Route r in routingTable)
                    if (r.con == sender)
                        invalidRoutes.Add(r);
            }


            foreach(Route r in invalidRoutes)
            {
                lock (routingTable)
                    routingTable.Remove(r);

                Frame frame = new Frame();
                frame.SID = 0;
                frame.Broadcast = true;
                //frame.RoutingError = true;
                frame.RID = 0;
                frame.HOP = 0;
                frame.PAY = BitConverter.GetBytes(r.id);

                //Send to all nodes that the route to this id is now invalid
                //When the broadcast arrives at the id trough another route the client should reply with a broadcast announcing itself again.
                //@TODO: How do we ensure that all routers have recieved the invalid before the client re-anounces itself?

                foreach(Connection con in connections)
                    con.SendFrame(frame);
            }
        }

        private void Connection_OnFrameReceived(object sender, Frame e)
        {
            Connection connection = sender as Connection;

            if (e.HOP == MaxHop)
                return;

            e.HOP++;
            if (e.Broadcast)
            {

                //if(e.RoutingError)
                //{
                //    //An invalid route
                //
                //    UInt16 id = BitConverter.ToUInt16(e.PAY, 0);
                //    int removed = 0;
                //    lock (routingTable)
                //        removed = routingTable.RemoveAll(r => r.id == id);
                //    
                //    if(removed > 0)
                //    {
                //        //Resend the broadcast.
                //        foreach (Connection con in connections)
                //        {
                //            if (con != connection)
                //                con.SendFrame(e);
                //        }
                //    }
                //}
                //else
                {
                    //Broadcasts are also used to build the routing table
                    Route route;
                    lock (routingTable)
                    {
                        route = routingTable.FirstOrDefault(r => r.id == e.RID);
                        if (route == null)
                        {
                            //Create new route, add to table
                            route = new Route() { con = connection, hops = e.HOP, id = e.SID };
                            routingTable.Add(route);
                        }
                        else
                        {
                            if (e.HOP < route.hops)
                            {
                                //Better route found, update table
                                route.con = connection;
                                route.hops = e.HOP;
                            }
                        }
                    }

                    if (e.HOP == route.hops)
                    {
                        //Resend package to all connections except for sender.

                        foreach (Connection con in connections)
                        {
                            if (con != connection)
                                con.SendFrame(e);
                        }
                    }
                }
                
            }
            else
            {
                //if(e.RoutingError)
                //{
                //    //This router has a route that is no longer available.
                //    UInt16 unreachableID = BitConverter.ToUInt16(e.PAY, 0);
                //    lock (routingTable)
                //        routingTable.RemoveAll(r => r.id == unreachableID);
                //}

                //Try to route the package.
                Route route;
                lock (routingTable)
                {
                    route = routingTable.FirstOrDefault(r => r.id == e.RID);
                }

                if (route == null)
                {
                    //Route not found, reply with an error to the sender.

                    //@TODO, We could send a broadcast requesting for the specific ID, if it isnt recieved within x time we send the reply.

                    Frame reply = new Frame();
                    reply.SID = e.RID;
                    reply.Broadcast = false;
                    //reply.RoutingError = true;
                    //reply.FID = e.FID;
                    reply.RID = e.SID;
                    reply.HOP = 0;
                    reply.PAY = BitConverter.GetBytes(e.RID);
                    connection.SendFrame(reply);

                    //Also, send a broadcast telling the other routers that the route of this sid has been compromised.
                    //Still need to figure out the details of this.

                }
                else
                {
                    e.HOP++;
                    route.con.SendFrame(e);
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
