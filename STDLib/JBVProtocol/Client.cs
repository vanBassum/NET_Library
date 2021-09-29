using STDLib.Ethernet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STDLib.Serializers;
using Newtonsoft.Json;

namespace STDLib.JBVProtocol
{
    public class Client
    {
		public List<object> DiscoveredListeners = new List<object>();
		public delegate ResponseFrame RequestReceivedHandler(RequestFrame requestFrame);
		public RequestReceivedHandler OnRequestReceived { get; set; }
		Dictionary<byte, TaskCompletionSource<ResponseFrame>> pending = new Dictionary<byte, TaskCompletionSource<ResponseFrame>>();

		List<Framing> framedConnections = new List<Framing>();
        UInt64 myAddress = 0xAA55AA55AA550000;
		SoftwareID SID = SoftwareID.Unknown;

		static readonly UInt64 BROADCAST = 0xFFFFFFFFFFFFFFFF;
		static readonly UInt64 UNKNOWNADDR = 0x00000000000000000;


		void HandleProtocolFrame(Framing framing, ProtocolFrame frame)
        {
			switch(frame.Command)
            {
				case ProtocolFrame.Commands.DiscoveryReply:
					string s = Encoding.ASCII.GetString(frame.Data);
					var v  = JsonConvert.DeserializeObject(s);
					DiscoveredListeners.Add(v);
					break;
				case ProtocolFrame.Commands.DiscoveryRequest:
					DiscoveryInfo di = new DiscoveryInfo();
					di.Address = myAddress;
					di.SID = SID;
					string ser = JsonConvert.SerializeObject(di);
					ProtocolFrame reply = ProtocolFrame.ASCII(frame.SrcAddress, frame.FrameID, ProtocolFrame.Commands.DiscoveryReply, ser);
					framing.SendFrame(reply);
					//TODO listeners
					break;
				case ProtocolFrame.Commands.ReplyID:
					//TODO: Update routing table
					break;
			}
        }


		void HandleFrame(Framing framing, Frame frame)
        {
            switch (frame)
            {
				case ProtocolFrame pf:
					HandleProtocolFrame(framing, pf);
					break;

				case RequestFrame request:
					ResponseFrame res = OnRequestReceived?.Invoke(request);
					if (res != null)
					{
						res.SrcAddress = myAddress;
						framing.SendFrame(res);
					}
					break;

				case ResponseFrame response:
					TaskCompletionSource<ResponseFrame> tcs;
					if (pending.TryGetValue(response.FrameID, out tcs))
					{
						pending.Remove(response.FrameID);
						tcs.SetResult(response);
					}
					break;
				default:
					throw new NotImplementedException($"{frame.GetType().Name} frames not supported yet.");
			}

        }

		void RouteFrame(object sender, Frame frame)
		{
			if (sender is Framing framing)
			{
				if (frame.DstAddress == BROADCAST)
				{
					HandleFrame(framing, frame);
					if (framing.GetConnectionType() == ConnectionTypes.Direct)
					{
						//Reroute frame
					}
				}
				else if (frame.DstAddress == myAddress || frame.DstAddress == UNKNOWNADDR)
				{
					HandleFrame(framing, frame);
				}
				else
				{
					//Reroute frame
				}
			}
		}

		void SendFrame(Frame frame)
        {
			//Do all routing stuff here!!!
			frame.SrcAddress = myAddress;
			if(frame.DstAddress == BROADCAST)
            {
				foreach (var f in framedConnections)
					f.SendFrame(frame);
            }
        }

		public void AddConnection(IConnection con)
		{
			Framing framing = new Framing();
			framing.SetConnection(con);
            framing.FrameReceived += RouteFrame;
			framedConnections.Add(framing);
		}

        

        public void AddListener()
        {
			throw new NotImplementedException();
        }

		public void SendDiscoveryRequest()
        {
			DiscoveredListeners.Clear();
			ProtocolFrame frame = ProtocolFrame.ASCII(BROADCAST, 0, ProtocolFrame.Commands.DiscoveryRequest, "");
			SendFrame(frame);
		}

		public void Test()
        {
			ProtocolFrame frame = ProtocolFrame.ASCII(BROADCAST, 0, ProtocolFrame.Commands.RequestID, "");
			SendFrame(frame);
		}

		/*
		public Task<ResponseFrame> SendRequest(RequestFrame request)
        {
			byte frameId = 0;
			while (pending.ContainsKey(frameId) && frameId < 255)
				frameId++;
			if (frameId < 255)
			{
				var tcs = new TaskCompletionSource<ResponseFrame>();
				pending[frameId] = tcs;
				request.FrameID = frameId;
				SendFrame(request);
				return tcs.Task;
			}
			else
				throw new Exception("Max concurrent frames reached");
			
		}
		*/
    }

}
