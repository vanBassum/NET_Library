using STDLib.Extentions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace STDLib.JBVProtocol.IO
{
    
    /// <summary>
    /// The frame contains all information nessesary to get the frame to the right client.
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// The version of this protocol.
        /// </summary>
        public const byte PROTOCOLVERSION = 1;

        private byte OPT = 0;
        /// <summary>
        /// Version, Used to indicate the version of the protocol frame used incase we want to change something later and keep things compatible.
        /// </summary>
        public byte VER { get; private set; } = PROTOCOLVERSION;

        /// <summary>
        /// The number of hops the frame was rerouted between nodes in the network.
        /// </summary>
        public byte HOP { get; set; } = 0;

        /// <summary>
        /// Sender ID, The id of the sender.
        /// </summary>
        public UInt16 SID { get; set; } = 0;

        /// <summary>
        /// Receiver ID, The id of the Receiver. Where 0x00 is reserved for the first device found and 0xFF for a broadcast.
        /// </summary>
        public UInt16 RID { get; set; } = 0;

        /// <summary>
        /// Payload, The payload of the frame.
        /// </summary>
        public byte[] PAY { get; set; } = new byte[0];

        /// <summary>
        /// Indicates wheter this frame should be broadcasted to all potential recievers.
        /// </summary>
        public bool Broadcast { get { return optGet(0); } set { optSet(0, value); } }

        /// <summary>
        /// When true one of the routers is requesting one of the clients to send a broadcast so the router can update its routingtable.
        /// RID will be used to indicate witch client is supposed to send the broadcast.
        /// </summary>
        public bool RoutingInfo { get { return optGet(1); } set { optSet(1, value); } }



        bool optGet(int bit)
        {
            return (OPT & (1 << bit)) > 0;
        }

        void optSet(byte bit, bool val)
        {
            if (val)
                OPT |= (byte)(1 << bit);
            else
                OPT &= (byte)~(1 << bit);
        }


        /// <summary>
        /// Fills this object from a byte array.
        /// </summary>
        /// <param name="raw"></param>
        public void Populate(byte[] raw)
        {
            OPT = raw[0];
            VER = raw[1];
            HOP = raw[2];
            SID = BitConverter.ToUInt16(raw, 3);
            RID = BitConverter.ToUInt16(raw, 5);
            PAY = raw.SubArray(7);
        }

        /// <summary>
        /// Creates a byte array from this object.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            List<byte> raw = new List<byte>();

            raw.Add(OPT);
            raw.Add(VER);
            raw.Add(HOP);
            raw.AddRange(BitConverter.GetBytes(SID));
            raw.AddRange(BitConverter.GetBytes(RID));
            raw.AddRange(PAY);
            return raw.ToArray();
        }


        public static Frame CreateMessageFrame(UInt16 SID, UInt16 RID, byte[] payload)
        {
            Frame frame = new Frame();
            frame.VER = PROTOCOLVERSION;
            frame.HOP = 0;
            frame.SID = SID;
            frame.RID = RID;
            frame.PAY = payload;
            frame.Broadcast = false;
            frame.RoutingInfo = false;
            return frame;
        }


        /// <summary>
        /// Method to send a message to all connected clients when a server is used.
        /// </summary>
        /// <param name="payload">Data to be send</param>
        public static Frame CreateBroadcastFrame(UInt16 SID, byte[] payload)
        {
            Frame frame = new Frame();
            frame.VER = PROTOCOLVERSION;
            frame.HOP = 0;
            frame.SID = SID;
            frame.RID = 0;
            frame.PAY = payload;
            frame.Broadcast = true;
            frame.RoutingInfo = false;
            return frame;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SID">The ID of the router requesting the id.</param>
        /// <param name="requestedID">The ID of the client that has to send the broadcast.</param>
        /// <returns></returns>
        public static Frame CreateRequestID(UInt16 SID, UInt16 requestedID)
        {
            Frame frame = new Frame();
            frame.VER = PROTOCOLVERSION;
            frame.HOP = 0;
            frame.SID = SID;
            frame.RID = requestedID;
            frame.PAY = new byte[0];
            frame.Broadcast = true;
            frame.RoutingInfo = true;
            return frame;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SID">The ID of the client responding to the router</param>
        /// <returns></returns>
        public static Frame CreateReplyToRequestID(UInt16 SID)
        {
            Frame frame = new Frame();
            frame.VER = PROTOCOLVERSION;
            frame.HOP = 0;
            frame.SID = SID;
            frame.RID = 0;
            frame.PAY = new byte[0];
            frame.Broadcast = true;
            frame.RoutingInfo = true;
            return frame;
        }
    }
}
