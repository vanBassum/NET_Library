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
        protected override byte[] Data { get => BitConverter.GetBytes((UInt32)SoftwareID); set => SoftwareID = (SoftwareID)BitConverter.ToUInt32(value, 0); }

        public override bool IsBroadcast => false;
    }
}
