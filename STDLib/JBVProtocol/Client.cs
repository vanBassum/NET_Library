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
		Dictionary<UInt64, Route> routingTable = new Dictionary<ulong, Route>();
		List<Tuple<Framing, Frame>> pendingFrames = new List<Tuple<Framing, Frame>>();


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
					ProtocolFrame reply = ProtocolFrame.ASCII(myAddress, frame.SrcAddress, frame.FrameID, ProtocolFrame.Commands.DiscoveryReply, ser);
					framing.SendFrame(reply);
					//TODO listeners
					break;
				case ProtocolFrame.Commands.RequestID:
					framing.SendFrame(ProtocolFrame.ASCII(myAddress, BROADCAST, frame.FrameID, ProtocolFrame.Commands.ReplyID, ""));
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
			//Some broadcast protocols echo outgoing data, catch these here!
			if (sender is Framing fra)
			{
				if (frame.SrcAddress == myAddress && fra.GetConnectionType() == ConnectionTypes.Broadcast)
					return;
			}


			if (frame.DstAddress == BROADCAST)
			{
				if (sender is Framing framing)
				{
					//Update routing table
					Route newRoute = new Route { Connection = framing, Hops = frame.Hops };
					if (routingTable.TryGetValue(frame.SrcAddress, out Route existingRoute))
						routingTable[frame.SrcAddress] = Route.GetBestRoute(newRoute, existingRoute);
					else
					{
						routingTable[frame.SrcAddress] = newRoute;

						//Retry sending pending frames.
						for (int i = 0; i < pendingFrames.Count; i++)
						{
							Framing fr = pendingFrames[i].Item1;
							Frame f = pendingFrames[i].Item2;
							if (routingTable.TryGetValue(f.DstAddress, out Route route))
							{
								route.Connection.SendFrame(f);
								pendingFrames.RemoveAt(i);
								i--;
							}
						}
					}

					//Resend frame to all except source if source is direct connection
					if (framing.GetConnectionType() == ConnectionTypes.Direct)
					{
						foreach (var f in framedConnections.Where(a => a != framing))
						{
							if (routingTable.TryGetValue(frame.DstAddress, out Route route))
                            {
								if(frame.Hops <= route.Hops)
									f.SendFrame(frame);
							}
							else
                            {
								//Send to all
								f.SendFrame(frame);
							}
						}
					}


					//Broadcast, so we should do something with this frame aswell.
					HandleFrame(framing, frame);
					
				}
				else
				{
					//We send this, so send to all connections
					foreach (var f in framedConnections)
						f.SendFrame(frame);
				}

			}
			else if (frame.DstAddress == myAddress || frame.DstAddress == UNKNOWNADDR)
			{
				//Frame is addressed to us.
				if (sender is Framing framing)
					HandleFrame(framing, frame);
			}
			else
			{
				//Not a broadcast, also not addressed to us so redirect.
				if (routingTable.TryGetValue(frame.DstAddress, out Route route))
					route.Connection.SendFrame(frame);
				else
				{
					//Route not known, store frame in buffer,
					if (sender is Framing framing)
						pendingFrames.Add(new Tuple<Framing, Frame>(framing, frame));
					else if (sender == null)
						pendingFrames.Add(new Tuple<Framing, Frame>(null, frame));

					//Aks for the destination to report in order to build route.
					SendFrame(ProtocolFrame.ASCII(myAddress, BROADCAST, 0, ProtocolFrame.Commands.RequestID, ""));
				}
			}
		}

		
		void SendFrame(Frame frame)
        {
			frame.SrcAddress = myAddress;
			RouteFrame(null, frame);
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
			ProtocolFrame frame = ProtocolFrame.ASCII(myAddress, BROADCAST, 0, ProtocolFrame.Commands.DiscoveryRequest, "");
			SendFrame(frame);
		}

		public void Test()
        {
			ProtocolFrame frame = ProtocolFrame.ASCII(myAddress, BROADCAST, 0, ProtocolFrame.Commands.RequestID, "");
			SendFrame(frame);
		}

		
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
		
    }

}
