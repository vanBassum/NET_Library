using STDLib.Ethernet;
using System;
using System.Collections.Generic;
using System.Net;

namespace STDLib.JBVProtocol
{


    public class DiscoveryService
    {
        protected Framing framing = new Framing();
        public event EventHandler<ProtocolFrame> OnDiscoveryReply;



        public DiscoveryService()
        {
            framing.FrameReceived += Framing_FrameReceived;
        }


        private void Framing_FrameReceived(object sender, Frame e)
        {
            switch(e)
            {
                case ProtocolFrame frame:
                    if (frame.Command == ProtocolFrame.Commands.DiscoveryReply)
                        OnDiscoveryReply?.Invoke(this, frame);
                    break;

            }
        }

        public void SendFrame(ProtocolFrame frame)
        {
            framing.SendFrame(frame);
        }
    }

    public class UDPDiscoveryService : DiscoveryService
    {
        UdpSocketClient client = new UdpSocketClient();

        public UDPDiscoveryService()
        {
            client.Connect(new IPEndPoint(IPAddress.Broadcast, 51100));
            framing.SetConnection(client);
        }


    }


    public class Discovery
    {
        List<DiscoveryService> services = new List<DiscoveryService>();
        public void AddService(DiscoveryService service)
        {
            services.Add(service);
            service.OnDiscoveryReply += Service_OnDiscoveryReply;
        }

        private void Service_OnDiscoveryReply(object sender, ProtocolFrame e)
        {
            
        }


        public void Test()
        {
            ProtocolFrame frame = new ProtocolFrame();
            frame.Command = ProtocolFrame.Commands.DiscoveryRequest;
            frame.Data = new byte[0];
            services[0].SendFrame(frame);
        }
    }

}
