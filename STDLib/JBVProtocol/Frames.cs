using STDLib.Extentions;
using System;
using System.Collections.Generic;
using System.Text;

namespace STDLib.JBVProtocol
{
    //public static byte[] BROADCAST = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }; 

    


    public abstract class Frame
    {
		
        public UInt16 CRC { get; set; }
        public Types Type { get; set; }
        public UInt16 PayloadSize { get; set; }
        public byte[] SrcMAC { get; set; } = new byte[6];
        public byte[] DstMAC { get; set; } = new byte[6];
        public byte Hops { get; set; }
        //public byte[] Payload { get; set; }

        private static UInt16 FrameSize = 18;


		public abstract byte[] ToByteArray();


		public static Frame FromRAW(byte[] data)
        {
			Types type = (Types)data[2];

			switch (type)
            {
				case Types.ApplicationFrame:	return new ApplicationFrame(data);
				case Types.ProtocolFrame : return new ProtocolFrame(data);
				case Types.Partial: return new PartialFrame(data);
				default:
					throw new NotImplementedException($"{type} not supported.");
			}
        }


		public int GetTotalsize()
		{
			return FrameSize + PayloadSize;
		}

		public void CalcCRC()
		{
			CRC = 0x1234;
			//TODO
		}

		public bool CheckCRC()
		{
			return true;
		}

		public enum Types : byte
		{
			Unknown = 0,            //Undefined, 
			Partial = 1,            //When one large package has to be send as multiple smaller packages, e.g. ESP_NOW max 250 bytes
			ProtocolFrame = 2,      //Used by the protocol to manage the MESH
			ApplicationFrame = 3,   //These are send between clients.
		};


	}

	public class PartialFrame : Frame
	{
		public PartialFrame()
		{
			Type = Types.Partial;
		}

		public PartialFrame(byte[] data)
		{
			throw new NotImplementedException();
		}

		public override byte[] ToByteArray()
		{
			throw new NotImplementedException();
		}

	}

	public class ProtocolFrame : Frame
    {
		public Commands Command { get; set; }
		public byte[] Data { get; set; }

		public ProtocolFrame()
		{
			Type = Types.ProtocolFrame;
		}

		public ProtocolFrame(byte[] data)
		{
			throw new NotImplementedException();
		}

		public override byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }

        public enum Commands : byte
		{
			Unknown = 0,
			RequestID = 1,
			ReplyID = 2,
		};
	}


	public class ApplicationFrame : Frame
	{
		public byte[] Data;


		public ApplicationFrame()
        {
			Type = Types.ApplicationFrame;
        }

		public ApplicationFrame(byte[] data)
		{
			CRC = BitConverter.ToUInt16(data, 0);
			Type = (Types)data[2];
			PayloadSize = BitConverter.ToUInt16(data, 3);
			SrcMAC = data.SubArray(4, 6);
			DstMAC = data.SubArray(10, 6);
			Hops = data[17];
			Data = data.SubArray(18);
		}

        public override byte[] ToByteArray()
        {
			PayloadSize = (UInt16)Data.Length;
			List<byte> raw = new List<byte>();
			raw.AddRange(BitConverter.GetBytes(CRC));
			raw.Add((byte)Type);
			raw.AddRange(BitConverter.GetBytes(PayloadSize));
			raw.AddRange(SrcMAC);
			raw.AddRange(DstMAC);
			raw.Add(Hops);
			raw.AddRange(Data);
			return raw.ToArray();
		}

        public enum Commands : byte
		{
			Unknown = 0,
			RequestID = 1,
			ReplyID = 2,
		};
	}
}
