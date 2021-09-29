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
        byte[] myAddress = { 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55 };
		SoftwareID SID = SoftwareID.Unknown;

		static readonly byte[] BROADCAST = new byte[]{ 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
		static readonly byte[] UNKNOWNADDR = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };


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
					di.Address = BitConverter.ToString(myAddress).Replace("-", ":");
					di.SID = SID;
					string ser = JsonConvert.SerializeObject(di);
					ProtocolFrame reply = ProtocolFrame.ASCII(frame.SrcMAC, frame.FrameID, ProtocolFrame.Commands.DiscoveryReply, ser);
					framing.SendFrame(reply);
					//TODO listeners
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
						res.SrcMAC = myAddress;
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
				if (Enumerable.SequenceEqual(frame.DstMAC, BROADCAST))
				{
					HandleFrame(framing, frame);
					if (framing.GetConnectionType() == ConnectionTypes.Direct)
					{
						//Reroute frame
					}
				}
				else if (Enumerable.SequenceEqual(frame.DstMAC, myAddress)
						|| Enumerable.SequenceEqual(frame.DstMAC, UNKNOWNADDR))
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
			frame.SrcMAC = myAddress;
			if(frame.DstMAC == BROADCAST)
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
