using System;

namespace STDLib.JBVProtocol.Commands
{
    public class RequestSID : Command
    {
        protected override bool IsBroadcast => true;
        public override UInt32 CommandID => (UInt32)CommandList.RequestSID;
        /// <summary>
        /// Make <see cref="SoftwareID.Unknown"/> to request all SID's otherwise only devices with matching SID will reply
        /// </summary>
        public SoftwareID SID { get; set; } = SoftwareID.Unknown;

        public override void Populate(byte[] data)
        {
            SID = (SoftwareID)BitConverter.ToUInt32(data, 0);
        }

        public override byte[] ToArray()
        {
            return BitConverter.GetBytes((UInt32)SID);
        }
    }
}