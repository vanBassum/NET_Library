using Microsoft.VisualStudio.TestTools.UnitTesting;
using STDLib.JBVProtocol;
using STDLib.JBVProtocol.Connections;
using STDLib.JBVProtocol.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JBV_Protocol_test
{

    [TestClass]
    public class RouterStarTest
    {
        List<Message> recievedBroadcasts = new List<Message>();
        List<Message> recievedMessages = new List<Message>();
        Router router;
        Client client1;
        Client client2;
        Client client3;
        Client client4;
        Client client5;

        Bridge bridge1;
        Bridge bridge2;
        Bridge bridge3;
        Bridge bridge4;
        Bridge bridge5;

        void SetupConnections()
        {

            bridge1 = new Bridge();
            bridge2 = new Bridge();
            bridge3 = new Bridge();
            bridge4 = new Bridge();
            bridge5 = new Bridge();

            router = new Router(20);
            router.AddConnection(bridge1.Con1);
            router.AddConnection(bridge2.Con1);
            router.AddConnection(bridge3.Con1);
            router.AddConnection(bridge4.Con1);
            router.AddConnection(bridge5.Con1);

            client1 = new Client(1);
            client1.SetConnection(bridge1.Con2);
            client1.OnBroadcastRecieved += OnBroadcastRecieved;
            client1.OnMessageRecieved += OnMessageRecieved;

            client2 = new Client(2);
            client2.SetConnection(bridge2.Con2);
            client2.OnBroadcastRecieved += OnBroadcastRecieved;
            client2.OnMessageRecieved += OnMessageRecieved;

            client3 = new Client(3);
            client3.SetConnection(bridge3.Con2);
            client3.OnBroadcastRecieved += OnBroadcastRecieved;
            client3.OnMessageRecieved += OnMessageRecieved;

            client4 = new Client(4);
            client4.SetConnection(bridge4.Con2);
            client4.OnBroadcastRecieved += OnBroadcastRecieved;
            client4.OnMessageRecieved += OnMessageRecieved;

            client5 = new Client(5);
            client5.SetConnection(bridge5.Con2);
            client5.OnBroadcastRecieved += OnBroadcastRecieved;
            client5.OnMessageRecieved += OnMessageRecieved;

            recievedBroadcasts.Clear();
            recievedMessages.Clear();
        }

        private void OnMessageRecieved(object sender, Message e)
        {
            Client c = sender as Client;
            Assert.AreEqual(c.ID, e.GetFrame().RID);
            recievedMessages.Add(e);
        }

        private void OnBroadcastRecieved(object sender, Message e)
        {
            recievedBroadcasts.Add(e);
        }

        [TestMethod]
        public void TestSingleBroadcast()
        {
            SetupConnections();

            string testMessage = "BroadcastData 1";
            byte[] txPayload = Encoding.ASCII.GetBytes(testMessage);


            client1.SendBroadcast(txPayload);

            //Stuff is done on another thread, so wait for the broadcasts.
            for(int retry = 0; retry < 100; retry++)
            {
                if (recievedBroadcasts.Count() == 4)
                    break;
                System.Threading.Thread.Sleep(10);
            }

            
            //No messages should have been recieved.
            Assert.AreEqual(0, recievedMessages.Count(), "Messages where recieved while no messages where send.");

            //The sender should't have recieved the broadcast while the recievers should have.
            Assert.AreEqual(4, recievedBroadcasts.Count(), "Not the right amount of broadcasts recieved.");

            foreach(Message rx in recievedBroadcasts)
            {
                //Check all broadcasts for payload and SID.
                Assert.AreEqual(1, rx.SID, "Wrong SID recieved.");
                Assert.AreEqual(testMessage, Encoding.ASCII.GetString(rx.Payload), "Payload was corrupted");
            }
        }



        [TestMethod]
        public void TestSingleMessage()
        {
            SetupConnections();

            string testMessage = "BroadcastData 1";
            byte[] txPayload = Encoding.ASCII.GetBytes(testMessage);


            client1.SendMessage(2, txPayload);

            //Stuff is done on another thread, so wait for the broadcasts.
            for (int retry = 0; retry < 100; retry++)
            {
                if (recievedMessages.Count() == 1)
                    break;
                System.Threading.Thread.Sleep(10);
            }


            //No broadcasts should have been recieved.
            Assert.AreEqual(0, recievedBroadcasts.Count(), "Broadcasts where recieved while no broadcasts where send.");

            //The sender should't have recieved the broadcast while the reciever should have.
            Assert.AreEqual(1, recievedMessages.Count(), "There one message send and more or less message have been recieved.");

            Message rxMessage = recievedMessages.FirstOrDefault();
            Assert.IsNotNull(rxMessage, "Message not recieved.");
            Assert.AreEqual(1, rxMessage.SID, "Wrong SID recieved.");
            Assert.AreEqual(testMessage, Encoding.ASCII.GetString(rxMessage.Payload), "Payload was corrupted");
        }

    }
}
