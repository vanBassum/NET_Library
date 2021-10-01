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
        UInt64 myAddress;
        const UInt64 BROADCASTADDR = 0xFFFFFFFFFFFFFFFF;
        const UInt64 UNKNOWNADDR = 0x0000000000000000;

        List<FramedConnection> connections = new List<FramedConnection>();
		public delegate byte[] RequestReceivedHandler(Client client, byte[] requestData);
		public RequestReceivedHandler HandleRequest { get; set; }

        Dictionary<byte, TaskCompletionSource<byte[]>> pendingFrames = new Dictionary<byte, TaskCompletionSource<byte[]>>();
        byte frameIdCounter = 0;

        public void AddConnection(IConnection connection)
        {
            FramedConnection fCon = new FramedConnection();
            fCon.SetConnection(connection);
            fCon.FrameCollected += FrameReceived;
            connections.Add(fCon);
        }


        public async Task<string> SendSmallRequest(string data)
        {
            byte fid = frameIdCounter++;
            TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>();

            if (!pendingFrames.ContainsKey(fid))
            {
                pendingFrames.Add(fid, tcs);

                SmallRequestFrame frame = new SmallRequestFrame();
                frame.Type = FrameTypes.SmallRequest;
                frame.Id = fid;
                frame.Payload = Encoding.ASCII.GetBytes(data);
                frame.PayloadSize = (UInt16)frame.Payload.Length;
                frame.CalcCRC();
                SendFrame(frame);


                return Encoding.ASCII.GetString(await tcs.Task);
            }
            else
                throw new NotImplementedException("Id in use");

        }


        void SendFrame(Frame f)
        {
            connections[0].SendFrame(f);
        }


        private void FrameReceived(object sender, Frame frame)
        {
            if(sender is FramedConnection con)
            {

                if(frame.CheckCRC())
                {
                    switch(frame)
                    {
                        case SmallRequestFrame f:
                            HandleFrame(con, f);
                            break;
                        case SmallResponseFrame f:
                            HandleFrame(con, f);
                            break;
                        case ProtocolFrame pf:
                            RouteFrame(con, pf);
                            break;
                        default:
                            throw new Exception("Frame type not supported");
                    }
                }
            }
        }

        private void RouteFrame(FramedConnection con, RoutingFrame frame)
        {
            //Check if we should handle this frame.
            if (frame.NxtAddr == BROADCASTADDR || frame.NxtAddr == UNKNOWNADDR || frame.NxtAddr == myAddress)
            {
                if (frame.DstAddr == BROADCASTADDR)
                {
                    //Re-route frame to others! (check quality etc...)
                    //And handle frame ourselves.
                }
                else if (frame.DstAddr == UNKNOWNADDR && frame.DstAddr == myAddress)
                {
                    //Addressed to us, so handle frame.
                    switch (frame)
                    {
                        case ProtocolFrame pf:
                            HandleFrame(con, pf);
                            break;

                        default:
                            throw new Exception("Frame type not supported");
                    }
                }
                else
                {
                    //Not addressed to us, so reroute.

                }
            }
        }

        private void HandleFrame(FramedConnection con, ProtocolFrame frame)
        {
           
        }

        private void HandleFrame(FramedConnection con, SmallRequestFrame frame)
        {
            byte[] data = HandleRequest?.Invoke(this, frame.Payload);
            SmallResponseFrame response = new SmallResponseFrame();
            response.Type = FrameTypes.SmallResponse;
            response.Id = frame.Id;
            response.PayloadSize = (UInt16)data.Length;
            response.Payload = data;
            response.CalcCRC();
        }

        private void HandleFrame(FramedConnection con, SmallResponseFrame frame)
        {
            if (pendingFrames.TryGetValue(frame.Id, out TaskCompletionSource<byte[]> tcs))
            {
                tcs.SetResult(frame.Payload);
            }
        }
    }
}
