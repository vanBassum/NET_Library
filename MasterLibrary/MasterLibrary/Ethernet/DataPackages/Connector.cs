using System;
using System.Windows.Forms;
using MasterLibrary.Ethernet.Frames;
using MasterLibrary.Misc;
using System.Collections.Generic;
using MasterLibrary.Datasave.Serializers;
using MasterLibrary.Ethernet;

namespace MasterLibrary.Ethernet
{
    public class TCP_Client<T> where T : TCP_Object
    {
        public event Action<T> OnClientJoined;
        public event Action<T> OnClientLeft;
        public event Action OnDeclined;

        public T myClient = Activator.CreateInstance<T>();
        public ThreadedBindingList<T> otherClients;

        private JSONIgnore serializer = new JSONIgnore();
        TcpSocketClientEscaped socket = new TcpSocketClientEscaped();
        private Timer sendChangesTimer = new Timer();
        

        public TCP_Client()
        {
            otherClients = new ThreadedBindingList<T>();
            myClient.StartRecordingChanges();
            sendChangesTimer.Interval = 500;
            sendChangesTimer.Tick += SendChangesTimer_Tick;
            sendChangesTimer.Start();
        }

        public void Connect()
        {
            socket.BeginConnect("127.0.0.1", 1000, 1000);
            socket.SetTcpKeepAlive(true, 1000, 100);
            socket.OnPackageRecieved += Socket_OnPackageRecieved;
        }

        private void Socket_OnPackageRecieved(object sender, byte[] data)
        {
            IFrame f = serializer.Deserialize<IFrame>(data);
            T other;
            switch (f)
            {
                case SendId frame:
                    //Yaay we got an ID from the server.
                    myClient.ID = frame.ClientID;
                    sendChangesTimer.Start();
                    break;

                case SendClientJoined frame:
                    //Someone joined the party.
                    other = Activator.CreateInstance<T>();
                    other.ID = frame.ClientID;
                    if (!otherClients.Exists(o => o.ID == other.ID))
                    {
                        otherClients.Add(other);
                        OnClientJoined?.Invoke(otherClients[frame.ClientID]);
                    }
                    break;

                case SendClientList frame:
                    //Create clients, if already existed overwrite
                    foreach (int id in frame.Clients)
                    {
                        other = Activator.CreateInstance<T>();
                        other.ID = id;
                        if (!otherClients.Exists(o => o.ID == other.ID))
                        {
                            otherClients.Add(other);
                            OnClientJoined?.Invoke(otherClients[frame.ClientID]);
                        }
                    }
                    break;

                case SendClientLeft frame:
                    OnClientLeft?.Invoke(otherClients[frame.ClientID]);
                    otherClients.RemoveWhere(o=>o.ID == frame.ClientID);
                    break;

                case SendParameterUpdate frame:

                    int ind = otherClients.FindIndex(o => o.ID == frame.ClientID);
                    if (ind != -1)
                    {
                        foreach (KeyValuePair<string, object> kvp in frame.Parameters)
                            typeof(T).GetProperty(kvp.Key).SetValue(otherClients[ind], kvp.Value);
                    }
                    break;
                case SendDecline frame:
                    OnDeclined?.Invoke();
                    break;

                default:
                    throw new NotImplementedException();
                    break;

            }
        }

        private void SendChangesTimer_Tick(object sender, EventArgs e)
        {
            if(myClient.HasChanges)
                SendFrame(new SendParameterUpdate(myClient.ID, myClient.GetAndClearChangedPars()));
        }

        private void SendFrame(IFrame dataFrame)
        {
            socket.SendPackage(serializer.Serialize(dataFrame));
        }

        
    }

}
