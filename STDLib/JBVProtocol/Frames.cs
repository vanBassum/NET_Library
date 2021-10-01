using STDLib.Extentions;
using System;
using System.Collections.Generic;
using System.Text;

namespace STDLib.JBVProtocol
{
	//public static byte[] BROADCAST = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }; 


	public enum FrameTypes : byte
	{
		Unknown = 0,                //Used to catch and discard improper initialized frames.
		SmallRequest = 1,           //Smallest form of frame supported by this protocol.		6 bytes overhead.
		SmallResponse = 2,          //Smallest form of frame supported by this protocol.
		Protocol = 3,               //Used by the clients to maintain the network.
		ApplicationRequest = 4,     //Part of request response on application level.
		ApplicationResponse = 5,    //Part of request response on application level.
	};

	public enum ProtocolCommands : byte
	{
		Unknown          = 0,
		RequestID        = 1,
		ReplyID          = 2,
		DiscoveryRequest = 3,
		DiscoveryReply   = 4,
	};


	public abstract class Frame
    {
		public FrameTypes Type { get; set; }
		public byte Id { get; set; }
		public UInt16 CRC { get; set; }
		public UInt16 PayloadSize { get; set; }

		public virtual bool CheckCRC() { return true; }
		public virtual void CalcCRC() {  }
	}

    

	public class SmallRequestFrame : Frame
	{
		public byte[] Payload { get; set; }

		public override void CalcCRC()
        {
			CRC = Misc.CRC.CRC16_2(Payload, Payload.Length);
        }
	}

	public class SmallResponseFrame : Frame
	{
		public byte[] Payload { get; set; }

		public override bool CheckCRC() 
		{ 
			UInt16 crc =  Misc.CRC.CRC16_2(Payload, Payload.Length);
			return crc == CRC; 
		}
	}



	public class RoutingFrame : Frame
    {
		public UInt16 Quality { get; set; }
		public UInt64 SrcAddr { get; set; }
		public UInt64 NxtAddr { get; set; }
		public UInt64 DstAddr { get; set; }
	}

	public class ProtocolFrame : RoutingFrame
	{
		public ProtocolCommands CMD { get; set; }
		public byte[] Data { get; set; }
	}

	public class AppRequestFrame : RoutingFrame
	{
		public byte[] Data { get; set; }
	}

	public class AppResponseFrame : RoutingFrame
	{
		public byte[] Data { get; set; }
	}
}
