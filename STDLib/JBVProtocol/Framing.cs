using STDLib.Ethernet;
using System;
using System.Collections.Generic;
using System.Text;

namespace STDLib.JBVProtocol
{
    public class Framing
    {
        IConnection _connection;

        public int MaxPackageSize { get; set; } = 0;
        public event EventHandler<Frame> FrameReceived;


        //@TODO: We are assuming we receive complete frames ending in 0x00. Should collect data untill 0x00 is found!
        void HandleIncommingData(IConnection con, byte[] data)
        {
            byte[] decoded = COBS.Decode(data);
            Frame frame = Frame.FromRAW(decoded);
            if(frame.CheckCRC())
            {

                if (frame is PartialFrame pf)
                    throw new NotImplementedException($"{frame.GetType().Name} frames not supported yet.");
                else
                    FrameReceived?.Invoke(this, frame);
            }
        }

        void Send(Frame frame)
        {
            frame.CalcCRC();
            byte[] decoded = frame.ToByteArray();
            byte[] encoded = COBS.Encode(decoded);
            _connection?.SendData(encoded);
        }

        public void SendFrame(Frame frame)
        {
            //int decodedSize = frame.GetTotalsize();
            //int encodedSize = COBS.CalcEncodedSize(decodedSize);
            //
            //if (MaxPackageSize >= encodedSize || MaxPackageSize == 0)
            //{
                Send(frame);
           //}
           //else
           //{
           //    throw new NotImplementedException("Partial frames not supported yet.");
           //}
        }

        public void SetConnection(IConnection connection)
        {
            _connection = connection;
            _connection.OnDataRecieved += (s, e) => HandleIncommingData((IConnection)s, e);
        }

    }
}
