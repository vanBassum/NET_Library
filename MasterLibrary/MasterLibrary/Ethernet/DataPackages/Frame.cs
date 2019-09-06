using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.Ethernet.Frames
{
    public interface IFrame
    {
        int SenderID { get; }
        bool Relay { get; } //if true, resend data to all clients
    }



    public class SendId : IFrame
    {
        public int SenderID { get; }
        public bool Relay { get; } = false;

        public SendId(int senderId)
        {
            SenderID = senderId;
        }
    }

    public class SendDecline : IFrame
    {
        public int SenderID { get;  }
        public bool Relay { get; } = false;

        public SendDecline(int senderId)
        {
            SenderID = senderId;
        }
    }

    public class SendClientJoined : IFrame
    {
        public int SenderID { get;  }
        public bool Relay { get; } = false;

        public SendClientJoined(int senderId)
        {
            SenderID = senderId;
        }
    }

    public class SendClientList : IFrame
    {
        public int SenderID { get;  }
        public bool Relay { get; } = false;
        public List<int> Clients { get; }
        public SendClientList(int senderId, List<int> clients)
        {
            SenderID = senderId;
            Clients = clients;
        }
    }

    public class SendClientLeft : IFrame
    {
        public int SenderID { get;  }
        public bool Relay { get; } = false;

        public SendClientLeft(int senderId)
        {
            SenderID = senderId;
        }
    }


    public class SendParameterUpdate : IFrame
    {
        public bool Relay { get; } = true;
        public int SenderID { get;  }

        public Dictionary<string, object> Parameters { get; set; }

        public SendParameterUpdate(int senderId, Dictionary<string, object> parameters)
        {
            this.SenderID = senderId;
            Parameters = parameters;
        }
    }

    public class SendObject : IFrame
    {
        public bool Relay { get; } = true;
        public int SenderID { get;  }
        public byte[] serializedObject { get; set; }

        public SendObject(int senderId, byte[] serObj)
        {
            this.SenderID = senderId;
            serializedObject = serObj;
        }
    }
}
