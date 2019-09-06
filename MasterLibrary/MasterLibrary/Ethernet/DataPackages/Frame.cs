using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.Ethernet.Frames
{
    public interface IFrame
    {
        int ClientID { get; } //-1 is reserved for the server, all others are clients
        bool Relay { get; } //if true, resend data to all clients
    }


    public class SendId : IFrame
    {
        public int ClientID { get; }
        public bool Relay { get; } = false;

        public SendId(int clientId)
        {
            ClientID = clientId;
        }
    }

    public class SendDecline : IFrame
    {
        public int ClientID { get; }
        public bool Relay { get; } = false;

        public SendDecline(int clientId)
        {
            ClientID = clientId;
        }
    }

    public class SendClientJoined : IFrame
    {
        public bool Relay { get; } = false;
        public int ClientID { get; }

        public SendClientJoined(int clientId)
        {
            ClientID = clientId;
        }
    }

    public class SendClientList : IFrame
    {
        public int ClientID { get; }
        public bool Relay { get; } = false;
        public List<int> Clients { get; }
        public SendClientList(int clientId, List<int> clients)
        {
            ClientID = clientId;
            Clients = clients;
        }
    }

    public class SendClientLeft : IFrame
    {
        public bool Relay { get; } = false;
        public int ClientID { get; }

        public SendClientLeft(int clientId)
        {
            ClientID = clientId;
        }
    }

    public class SendParameterUpdate : IFrame
    {
        public bool Relay { get; } = true;
        public int ClientID { get; }

        public Dictionary<string, object> Parameters { get; set; }

        public SendParameterUpdate(int clientId, Dictionary<string, object> parameters)
        {
            ClientID = clientId;
            Parameters = parameters;
        }
    }

    public class SendObject : IFrame
    {
        public bool Relay { get; } = true;
        public int ClientID { get; }
        public byte[] serializedObject { get; set; }

        public SendObject(int clientId, byte[] serObj)
        {
            ClientID = clientId;
            serializedObject = serObj;
        }
    }


}
