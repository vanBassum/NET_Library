using STDLib.JBVProtocol.IO;
using System;
using System.Collections.Generic;

namespace STDLib.JBVProtocol
{
    /// <summary>
    /// Sends a broadcast over the network requesting a response from clients with a specific SID.
    /// </summary>
    public class RequestSoftwareID : CMD
    {
        /// <summary>
        /// When <see cref="SoftwareID.Unknown"/> all clients will respond with their SID.
        /// Otherwise only the clients with matching SID will respond.
        /// </summary>
        public SoftwareID SofwareID { get; set; }

        public override Frame GetFrame()
        {
            List<byte[]> data = new List<byte[]>
            {
                BitConverter.GetBytes(CMDID),
                BitConverter.GetBytes((UInt32)SofwareID),
            };

            Frame frame = new Frame();
            frame.RID = 0;
            frame.PAY = Framing.Stuff(data);
            frame.Broadcast = true;
            frame.Command = false;
            return frame;
        }

        public override void Populate(List<byte[]> arg)
        {
            SofwareID = (SoftwareID)BitConverter.ToUInt32(arg[1], 0);
        }
    }
}
