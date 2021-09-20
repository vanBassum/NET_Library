using STDLib.Ethernet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace STDLib.JBVProtocol
{
    public class Client
    {
		public class Message
        {
			public byte[] Source { get; set; }
			public byte[] Data;
        }
        public event EventHandler<Message> OnMessageReceived;

        Framing framing = new Framing();
        byte[] myAddress = { 0xAA, 0x55, 0xAA, 0x55, 0xAA, 0x55 };


		public Client()
        {
            framing.FrameReceived += (s, e) => HandleFrame(e);
        }



		void HandleFrame(Frame frame)
		{
			switch (frame)
			{
				case ApplicationFrame af:
					OnMessageReceived?.Invoke(this, new Message { Source = af.SrcMAC, Data = af.Data });
					break;
				default:
					throw new NotImplementedException($"{frame.GetType().Name} frames not supported yet.");
			}
		}

		public void SetConnection(IConnection con)
		{
			framing.SetConnection(con);
		}

		public Task<Message> SendMessage(byte[] dst, byte[] data)
		{
			if (dst.Length == 6)
			{
				var tcs = new TaskCompletionSource<Message>();
				EventHandler<Message> callback = null;
				callback = (sender, e) =>
				{
					OnMessageReceived -= callback;
					tcs.SetResult(e);
				};
				OnMessageReceived += callback;
				ApplicationFrame frame = new ApplicationFrame();
				frame.DstMAC = dst;
				frame.SrcMAC = myAddress;
				frame.Data = data;
				framing.SendFrame(frame);
				return tcs.Task;
			}
			throw new Exception("Invalid dst length");
		}
    }
}
