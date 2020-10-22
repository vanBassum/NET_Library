using System;
using System.Collections.Generic;
using System.IO;

namespace STDLib.JBVProtocol.IO
{
    /// <summary>
    /// Manages a single connetion.
    /// </summary>
    public abstract class Connection
    {

        /// <summary>
        /// Event will fire ones a complete frame has been recieved.
        /// </summary>
        public event EventHandler<Frame> OnFrameReceived;

        /// <summary>
        /// This event will fire if the connection is lost.
        /// </summary>
        public event EventHandler OnDisconnected;

        readonly Framing framing = new Framing();

        string member = "nan";
        /// <summary>
        /// 
        /// </summary>
        public Connection( [System.Runtime.CompilerServices.CallerFilePath] string memberName = "")
        {
            member = Path.GetFileNameWithoutExtension(memberName);
            framing.OnFrameCollected += Framing_OnFrameCollected;
        }

        /// <summary>
        /// Method to send a frame
        /// </summary>
        /// <param name="frame">Frame to send</param>
        public void SendFrame(Frame frame, [System.Runtime.CompilerServices.CallerFilePath] string memberName = "")
        {
            member = Path.GetFileNameWithoutExtension(memberName);
            byte[] raw = frame.GetBytes();
            byte[] data = framing.Stuff(raw);

            Console.Write("\t" + member + " > ");
            foreach(byte b in raw)
                Console.Write($"{b.ToString("X2")} ");
            Console.WriteLine("");
            SendData(data);
        }

        protected abstract void SendData(byte[] data);

        private void Framing_OnFrameCollected(object sender, byte[] e)
        {
            Frame frame = new Frame();
            frame.Populate(e);

            Console.Write("\t" + member + " < ");
            foreach (byte b in e)
                Console.Write($"{b.ToString("X2")} ");
            Console.WriteLine("");

            OnFrameReceived?.Invoke(this, frame);
        }

        protected void HandleData(byte[] data)
        {
            framing.Unstuff(data);
        }

        protected void Disconnected()
        {
            OnDisconnected?.Invoke(this, null);
        }
    }
}
