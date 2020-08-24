using Microsoft.VisualStudio.TestTools.UnitTesting;
using STDLib.JBVProtocol;
using STDLib.JBVProtocol.Connections;
using STDLib.JBVProtocol.IO;
using System.Collections.Generic;
using System.Text;

namespace JBV_Protocol_test
{
    [TestClass]
    public class IDServerTest
    {
        List<Message> recievedBroadcasts = new List<Message>();
        List<Message> recievedMessages = new List<Message>();
        Router router;
        Client client1;
        Client client2;
        DummyConnection con1;
        DummyConnection con2;
        DummyConnection con3;
        DummyConnection con4;

        void SetupConnections()
        {
            DummyConnection con1 = new DummyConnection();
            DummyConnection con2 = new DummyConnection();
            DummyConnection con3 = new DummyConnection();
            DummyConnection con4 = new DummyConnection();
            DummyConnection.CoupleConnections(con1, con2);
            DummyConnection.CoupleConnections(con3, con4);

            router = new Router(3);
            router.AddConnection(con2);
            router.AddConnection(con3);

            client1 = new Client(1);
            client1.SetConnection(con1);

            client2 = new Client(2);
            client2.SetConnection(con4);

            recievedBroadcasts.Clear();
            recievedMessages.Clear();

            client1.OnBroadcastRecieved += (sender, e) => recievedBroadcasts.Add(e);
            client1.OnMessageRecieved += (sender, e) => recievedMessages.Add(e);
            client2.OnBroadcastRecieved += (sender, e) => recievedBroadcasts.Add(e);
            client2.OnMessageRecieved += (sender, e) => recievedMessages.Add(e);
        }

        [TestMethod]
        public void TestSingleBroadcast()
        {
            SetupConnections();

            string testMessage = "BroadcastData 1";
            byte[] txPayload = Encoding.ASCII.GetBytes(testMessage);


            client1.SendBroadcast(txPayload);

            //Stuff is done on another thread, so wait for the broadcasts.
            for (int retry = 0; retry < 100; retry++)
            {
                if (recievedBroadcasts.Count() == 1)
                    break;
                System.Threading.Thread.Sleep(10);
            }


            //No messages should have been recieved.
            Assert.AreEqual(0, recievedMessages.Count(), "Messages where recieved while no messages where send.");

            //The sender should't have recieved the broadcast while the reciever should have.
            Assert.AreEqual(1, recievedBroadcasts.Count(), "There one broadcast send and more or less broadcasts have been recieved.");

            Message rxBroadcast = recievedBroadcasts.FirstOrDefault();
            Assert.IsNotNull(rxBroadcast, "Broadcast not recieved.");
            Assert.AreEqual(1, rxBroadcast.SID, "Wrong SID recieved.");
            Assert.AreEqual(testMessage, Encoding.ASCII.GetString(rxBroadcast.Payload), "Payload was corrupted");
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
