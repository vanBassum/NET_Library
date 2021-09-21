using STDLib.Ethernet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace STDLib.JBVProtocol
{
    public class Client
    {
		public delegate ResponseFrame RequestReceivedHandler(RequestFrame requestFrame);
		public RequestReceivedHandler OnRequestReceived { get; set; }

		Dictionary<byte, TaskCompletionSource<ResponseFrame>> pending = new Dictionary<byte, TaskCompletionSource<ResponseFrame>>();
        Framing framing = new Framing();
        byte[] myAddress = { 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55 };


		public Client()
        {
            framing.FrameReceived += (s, e) => HandleFrame(e);
        }

		void SendFrame(Frame frame)
        {
			frame.SrcMAC = myAddress;
			framing.SendFrame(frame);
        }


		void HandleFrame(Frame frame)
		{
			switch (frame)
			{
				case RequestFrame request:
					ResponseFrame res = OnRequestReceived?.Invoke(request);
					if (res != null)
						SendFrame(res);
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

		public void SetConnection(IConnection con)
		{
			framing.SetConnection(con);
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
