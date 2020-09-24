using STDLib.JBVProtocol.IO;
using System;
using System.Collections.Generic;

namespace STDLib.JBVProtocol
{
    /// <summary>
    /// Send this as a reply to the <see cref="RequestSoftwareID"/>
    /// </summary>
    public class ReplySoftwareID : CMD
    {
        /// <summary>
        /// The software id of the client
        /// </summary>
        public SoftwareID SoftwareID { get; set; }

        public UInt16 RID { get; set; }

        public override Frame GetFrame()
        {
            List<byte[]> data = new List<byte[]>
            {
                BitConverter.GetBytes(CMDID),
                BitConverter.GetBytes((UInt32)SoftwareID),
            };

            Frame frame = new Frame();
            frame.HOP = 0;
            frame.SID = 0;
            frame.RID = RID;
            frame.PAY = Framing.Stuff(data);
            frame.Broadcast = false;
            frame.Command = false;
            return frame;
        }

        public override void Populate(List<byte[]> arg)
        {
            SoftwareID = (SoftwareID)BitConverter.ToUInt32(arg[1], 0);
        }
    }
}
