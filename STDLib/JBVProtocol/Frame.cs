using STDLib.Extentions;
using System;
using System.Collections.Generic;

namespace STDLib.JBVProtocol
{

    /// <summary>
    /// The frame contains all information nessesary to get the frame to the right client.
    /// </summary>
    public class Frame
    {
        private byte OPT = 0;
        /// <summary>
        /// Version, Used to indicate the version of the protocol frame used incase we want to change something later and keep things compatible.
        /// </summary>
        public byte VER { get; private set; } = 1;

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
        /// Frame Id, Used to differentiate between frames. E.G. when multiple are send and the order of reply isn't guaranteed.
        /// </summary>
        public byte FID { get; set; } = 0;

        /// <summary>
        /// Payload, The payload of the frame.
        /// </summary>
        public byte[] PAY { get; set; } = new byte[0];

        /// <summary>
        /// Whether the package is a reply to a request send earlier, If false the package is a request itself.
        /// </summary>
        public bool Reply { get { return optGet(0); } set { optSet(0, value); } }

        /// <summary>
        /// Indicates whether an error has occured, for example when the command isn't recognized by the recieving party.
        /// </summary>
        public bool Error { get { return optGet(1); } set { optSet(1, value); } }

        /// <summary>
        /// Indicates wheter this frame should be broadcasted to all potential recievers.
        /// </summary>
        public bool Broadcast { get { return optGet(2); } set { optSet(2, value); } }





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
            FID = raw[3];
            SID = BitConverter.ToUInt16(raw, 4);
            RID = BitConverter.ToUInt16(raw, 6);
            PAY = raw.SubArray(8);
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
            raw.Add(FID);
            raw.AddRange(BitConverter.GetBytes(SID));
            raw.AddRange(BitConverter.GetBytes(RID));
            raw.AddRange(PAY);

            return raw.ToArray();
        }
    }
}
