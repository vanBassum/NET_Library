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
		public byte FrameID { get; set; }
		//public byte[] Payload { get; set; }

		private static UInt16 FrameSize = 19;


		public abstract byte[] ToByteArray();


		public static Frame FromRAW(byte[] data)
        {
			Types type = (Types)data[2];

			switch (type)
            {
				case Types.ProtocolFrame : return new ProtocolFrame(data);
				case Types.Partial: return new PartialFrame(data);
				//case Types.ApplicationFrame:	return new ApplicationFrame(data);
				case Types.RequestFrame: return new RequestFrame(data);
				case Types.ResponseFrame: return new ResponseFrame(data);
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
			//ApplicationFrame = 3,   //These are send between clients.
			RequestFrame = 4,       //Send from one client to another
			ResponseFrame = 5,      //Response from the other client to a request
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
			CRC = BitConverter.ToUInt16(data, 0);
			Type = (Types)data[2];
			PayloadSize = BitConverter.ToUInt16(data, 3);
			SrcMAC = data.SubArray(4, 6);
			DstMAC = data.SubArray(10, 6);
			Hops = data[17];
			FrameID = data[18];
			Command = (Commands)data[19];
			Data = data.SubArray(20);

		}

		public override byte[] ToByteArray()
        {
			PayloadSize = (UInt16)(Data.Length + 1);
			List<byte> raw = new List<byte>();
			raw.AddRange(BitConverter.GetBytes(CRC));
			raw.Add((byte)Type);
			raw.AddRange(BitConverter.GetBytes(PayloadSize));
			raw.AddRange(SrcMAC);
			raw.AddRange(DstMAC);
			raw.Add(Hops);
			raw.Add(FrameID);
			raw.Add((byte)Command);
			raw.AddRange(Data);
			return raw.ToArray();
        }

        public enum Commands : byte
		{
			Unknown = 0,
			RequestID = 1,
			ReplyID = 2,
			DiscoveryRequest = 3,
			DiscoveryReply = 4,
		};
	}


	public class RequestFrame : Frame
	{
		public byte[] Data;

		public RequestFrame()
        {
			Type = Types.RequestFrame;
        }

		public RequestFrame(byte[] data)
		{
			CRC = BitConverter.ToUInt16(data, 0);
			Type = (Types)data[2];
			PayloadSize = BitConverter.ToUInt16(data, 3);
			SrcMAC = data.SubArray(4, 6);
			DstMAC = data.SubArray(10, 6);
			Hops = data[17];
			FrameID = data[18];
			Data = data.SubArray(19);
			
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
			raw.Add(FrameID);
			raw.AddRange(Data);
			return raw.ToArray();
		}
	}

	public class ResponseFrame : Frame
	{
		public byte[] Data;

		public ResponseFrame()
		{
			Type = Types.ResponseFrame;
		}

		public ResponseFrame(byte[] data)
		{
			CRC = BitConverter.ToUInt16(data, 0);
			Type = (Types)data[2];
			PayloadSize = BitConverter.ToUInt16(data, 3);
			SrcMAC = data.SubArray(4, 6);
			DstMAC = data.SubArray(10, 6);
			Hops = data[17];
			FrameID = data[18];
			Data = data.SubArray(19);

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
			raw.Add(FrameID);
			raw.AddRange(Data);
			return raw.ToArray();
		}
	}
}
