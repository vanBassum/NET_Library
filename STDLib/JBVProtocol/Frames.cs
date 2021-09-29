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
        public UInt64 SrcAddress { get; set; }		
        public UInt64 DstAddress { get; set; }		
        public byte Hops { get; set; }				
		public byte FrameID { get; set; }			

		private static UInt16 FrameSize = 2 + 1 + 2+ 8 + 8+ 1 + 1;


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
			SrcAddress = BitConverter.ToUInt64(data, 5);
			DstAddress = BitConverter.ToUInt64(data, 13);
			Hops = data[21];
			FrameID = data[22];
			Command = (Commands)data[23];
			Data = data.SubArray(24);

		}

		public override byte[] ToByteArray()
        {
			PayloadSize = (UInt16)(Data.Length + 1);
			List<byte> raw = new List<byte>();
			raw.AddRange(BitConverter.GetBytes(CRC));
			raw.Add((byte)Type);
			raw.AddRange(BitConverter.GetBytes(PayloadSize));
			raw.AddRange(BitConverter.GetBytes(SrcAddress));
			raw.AddRange(BitConverter.GetBytes(DstAddress));
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

		public static ProtocolFrame ASCII(UInt64 dst, byte fid, Commands cmd, string msg)
		{
			ProtocolFrame response = new ProtocolFrame();
			response.Data = Encoding.ASCII.GetBytes(msg);
			response.Command = cmd;
			response.DstAddress = dst;
			response.FrameID = fid;
			return response;
		}

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
			SrcAddress = BitConverter.ToUInt64(data, 5);
			DstAddress = BitConverter.ToUInt64(data, 13);
			Hops = data[21];
			FrameID = data[22];
			Data = data.SubArray(23);
		}

		public override byte[] ToByteArray()
		{
			PayloadSize = (UInt16)(Data.Length + 1);
			List<byte> raw = new List<byte>();
			raw.AddRange(BitConverter.GetBytes(CRC));
			raw.Add((byte)Type);
			raw.AddRange(BitConverter.GetBytes(PayloadSize));
			raw.AddRange(BitConverter.GetBytes(SrcAddress));
			raw.AddRange(BitConverter.GetBytes(DstAddress));
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
			SrcAddress = BitConverter.ToUInt64(data, 5);
			DstAddress = BitConverter.ToUInt64(data, 13);
			Hops = data[21];
			FrameID = data[22];
			Data = data.SubArray(23);
		}

		public override byte[] ToByteArray()
		{
			PayloadSize = (UInt16)(Data.Length + 1);
			List<byte> raw = new List<byte>();
			raw.AddRange(BitConverter.GetBytes(CRC));
			raw.Add((byte)Type);
			raw.AddRange(BitConverter.GetBytes(PayloadSize));
			raw.AddRange(BitConverter.GetBytes(SrcAddress));
			raw.AddRange(BitConverter.GetBytes(DstAddress));
			raw.Add(Hops);
			raw.Add(FrameID);
			raw.AddRange(Data);
			return raw.ToArray();
		}
	}
}
