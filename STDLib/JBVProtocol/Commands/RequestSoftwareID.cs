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
        public SoftwareID SoftwareID { get; set; }
        protected override byte[] Data { get => BitConverter.GetBytes((UInt32)SoftwareID); set => SoftwareID = (SoftwareID)BitConverter.ToUInt32(value, 0); }

        public override bool IsBroadcast => true;
    }
}
