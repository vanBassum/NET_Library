using STDLib.Ethernet;
using STDLib.Extentions;
using System;
using System.Collections.Generic;
using System.Text;

namespace STDLib.JBVProtocol
{


    public class Framing
    {
        List<byte> buffer = new List<byte>();
        public event EventHandler<Frame> FrameCollected;

        void ParseBuffer()
        {
            var data = COBS.Decode(buffer.ToArray());
            FrameTypes type = (FrameTypes)data[0];

            switch (type)
            {
                case FrameTypes.Protocol:
                    {
                        ProtocolFrame frame = new ProtocolFrame();
                        frame.Type = type;
                        frame.Id = data[1];
                        frame.CRC = BitConverter.ToUInt16(data, 2);
                        frame.PayloadSize = BitConverter.ToUInt16(data, 4);
                        frame.Quality = BitConverter.ToUInt16(data, 6);
                        frame.SrcAddr = BitConverter.ToUInt16(data, 8);
                        frame.NxtAddr = BitConverter.ToUInt16(data, 16);
                        frame.DstAddr = BitConverter.ToUInt16(data, 24);
                        frame.CMD = (ProtocolCommands)data[32];
                        frame.Data = data.SubArray(33, frame.PayloadSize);
                        FrameCollected?.Invoke(this, frame);
                    }
                    break;

                case FrameTypes.SmallResponse:
                    {
                        SmallResponseFrame frame = new SmallResponseFrame();
                        frame.Type = type;
                        frame.Id = data[1];
                        frame.CRC = BitConverter.ToUInt16(data, 2);
                        frame.PayloadSize = BitConverter.ToUInt16(data, 4);
                        frame.Payload = data.SubArray(6, frame.PayloadSize);
                        FrameCollected?.Invoke(this, frame);
                    }
                    break;

                case FrameTypes.SmallRequest:
                    break;

                default:
                    throw new NotImplementedException($"Frametype {type} not supported");
            }
        }

        public void HandleByte(byte b)
        {
            buffer.Add(b);
            if(b == 0)
            {
                ParseBuffer();
                buffer.Clear();
            }
        }

        public void HandleByte(byte[] d)
        {
            foreach (byte b in d)
                HandleByte(b);
        }

        public static byte[] ToBytes(Frame frame)
        {
            List<byte> buf = new List<byte>();

            switch (frame)
            {
                case ProtocolFrame f:
                    buf.Add((byte)f.Type);
                    buf.Add((byte)f.Id);
                    buf.AddRange(BitConverter.GetBytes(f.CRC));
                    buf.AddRange(BitConverter.GetBytes(f.PayloadSize));
                    buf.AddRange(BitConverter.GetBytes(f.Quality));
                    buf.AddRange(BitConverter.GetBytes(f.SrcAddr));
                    buf.AddRange(BitConverter.GetBytes(f.NxtAddr));
                    buf.AddRange(BitConverter.GetBytes(f.DstAddr));
                    buf.Add((byte)f.CMD);
                    buf.AddRange(f.Data);
                    break;

                case SmallRequestFrame f:
                    buf.Add((byte)f.Type);
                    buf.Add((byte)f.Id);
                    buf.AddRange(BitConverter.GetBytes(f.CRC));
                    buf.AddRange(BitConverter.GetBytes(f.PayloadSize));
                    buf.AddRange(f.Payload);
                    break;

                default:
                    throw new NotImplementedException($"Frametype {frame.GetType().Name} not supported");
            }
            return COBS.Encode(buf.ToArray());
        }
    }

    public class FramedConnection
    {
        Framing framing = new Framing();
        IConnection con;
        public event EventHandler<Frame> FrameCollected;

        public FramedConnection()
        {
            framing.FrameCollected += Framing_FrameCollected;
        }

        public void SetConnection(IConnection connection)
        {
            if (con != null)
                con.OnDataRecieved -= Con_OnDataRecieved;
            con = connection;
            con.OnDataRecieved += Con_OnDataRecieved;
        }

        public void SendFrame(Frame frame)
        {
            if(con != null)
            {
                con.SendData(Framing.ToBytes(frame));
            }
        }

        private void Framing_FrameCollected(object sender, Frame e)
        {
            FrameCollected?.Invoke(this, e);
        }

        private void Con_OnDataRecieved(object sender, byte[] e)
        {
            framing.HandleByte(e);
        }
    }
}
