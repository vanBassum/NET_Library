using System;
using System.Collections.Generic;
using System.Linq;

namespace STDLib.JBVProtocol.IO
{

    /// <summary>
    /// Framing implements a bytestuffing algorithm that converts an incomming bytestream into complete frames and vice-versa.
    /// </summary>
    public class Framing
    {
        enum BS : byte
        {
            SOF = (byte)'&',    //Start of frame
            EOF = (byte)'%',    //End of frame
            ESC = (byte)'\\',   //Escape character
            NOP = (byte)'*',    //Does nothing, used to fill remainder when a static amount of data is required by the I/O.
        }

        bool startFound = false;
        bool esc = false;
        List<byte> dataBuffer = new List<byte>();

        /// <summary>
        /// Fires when a complete frame has been recieved.
        /// </summary>
        public event EventHandler<byte[]> OnFrameCollected;

        /// <summary>
        /// Method to destuff incomming data. 
        /// When a frame is complete <see cref="OnFrameCollected"/> will be fired.
        /// </summary>
        /// <param name="data">The stuffed data to unstuff</param>
        public void Unstuff(byte[] data)
        {
            int len = data.Length;
            for (int i = 0; i < len; i++)
            {
                bool record = false;

                if (esc)
                {
                    record = true;
                    esc = false;
                }
                else
                {
                    switch (data[i])
                    {
                        case (byte)BS.ESC:
                            esc = true;
                            break;
                        case (byte)BS.SOF:
                            startFound = true;
                            dataBuffer.Clear();
                            break;
                        case (byte)BS.EOF:
                            startFound = false;
                            OnFrameCollected(this, dataBuffer.ToArray());
                            break;
                        case (byte)BS.NOP:
                            break;
                        default:
                            record = true;
                            break;
                    }
                }

                if (record && startFound)
                    dataBuffer.Add(data[i]);
            } 
        }


        /// <summary>
        /// Method to stuff a frame into raw data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Stuff(byte[] data)
        {
            List<byte> dataOut = new List<byte>();
            dataOut.Add((byte)BS.SOF);
            foreach (byte b in data)
            {
                if (Enum.IsDefined(typeof(BS), b))
                    dataOut.Add((byte)BS.ESC);
                dataOut.Add(b);
            }

            dataOut.Add((byte)BS.EOF);
            return dataOut.ToArray();
        }
    }
}
