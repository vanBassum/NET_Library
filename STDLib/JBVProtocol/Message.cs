using STDLib.JBVProtocol.IO;
using System;

namespace STDLib.JBVProtocol
{
    /// <summary>
    /// A Message is in essention a frame, but only the relevant fields are exposed.
    /// </summary>
    public class Message
    {
        private Frame frame;

        /// <summary>
        /// Data of the message
        /// </summary>
        public byte[] Payload => frame.PAY;

        /// <summary>
        /// The Id of the sender.
        /// </summary>
        public UInt16 SID => frame.SID;

        /// <summary>
        /// Used to retrieve the origional frame.
        /// </summary>
        /// <returns></returns>
        public Frame GetFrame()
        {
            return frame;
        }

        /// <summary>
        /// </summary>
        /// <param name="frame"></param>
        public Message(Frame frame)
        {
            this.frame = frame;
        }

    }
}
