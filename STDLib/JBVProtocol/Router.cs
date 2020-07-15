using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        //Contains all active connections to this router.
        List<Connection> connections = new List<Connection>();

        //Contains all information how to reroute data
        List<Route> routingTable = new List<Route>();

        /// <summary>
        /// Add a connection to this router.
        /// </summary>
        /// <param name="con"></param>
        public void AddConnection(Connection con)
        {
            connections.Add(con);
            con.OnFrameReceived += Connection_OnFrameReceived;
        }



        private void Connection_OnFrameReceived(object sender, Frame e)
        {
            Connection connection = sender as Connection;
            e.HOP++;
            if (e.Broadcast)
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
            else
            {
                //Try to route the package.
                Route route;
                lock (routingTable)
                {
                    route = routingTable.FirstOrDefault(r => r.id == e.RID);
                }

                if (route == null)
                {
                    //Route not found, reply with an error to the sender.

                    Frame reply = new Frame();
                    reply.SID = 0;
                    reply.Reply = true;
                    reply.Broadcast = false;
                    reply.Error = true;
                    reply.FID = e.FID;
                    reply.RID = e.SID;
                    reply.HOP = 0;
                    reply.PAY = Encoding.ASCII.GetBytes("Route not found.");
                    connection.SendFrame(reply);
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
