using System;

namespace STDLib.JBVProtocol
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

        readonly Framing framing = new Framing();


        /// <summary>
        /// 
        /// </summary>
        public Connection()
        {
            framing.OnFrameCollected += Framing_OnFrameCollected;
        }

        /// <summary>
        /// Method to send a frame
        /// </summary>
        /// <param name="frame">Frame to send</param>
        public void SendFrame(Frame frame)
        {
            byte[] data = framing.Stuff(frame.GetBytes());
            SendData(data);
        }

        protected abstract void SendData(byte[] data);

        private void Framing_OnFrameCollected(object sender, byte[] e)
        {
            Frame frame = new Frame();
            frame.Populate(e);
            OnFrameReceived?.Invoke(this, frame);
        }

        protected void HandleData(byte[] data)
        {
            framing.Unstuff(data);
        }
    }
}
