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
        IDServer idServ;
        Router router;
        Client client1;
        Client client2;


        void SetupConnections()
        {
            
            DummyConnection conA_1 = new DummyConnection();
            DummyConnection conA_2 = new DummyConnection();
            DummyConnection conB_1 = new DummyConnection();
            DummyConnection conB_2 = new DummyConnection();
            DummyConnection conC_1 = new DummyConnection();
            DummyConnection conC_2 = new DummyConnection();
            DummyConnection.CoupleConnections(conA_1, conA_2);
            DummyConnection.CoupleConnections(conB_1, conB_2);
            DummyConnection.CoupleConnections(conC_1, conC_2);

            router = new Router(3);
            router.AddConnection(conA_1);
            router.AddConnection(conB_1);
            router.AddConnection(conC_1);

            client1 = new Client(0);
            client1.SetConnection(conA_2);

            client2 = new Client(2);
            client2.SetConnection(conB_2);

            idServ = new IDServer(conC_2);

        }

        [TestMethod]
        public void RequestLease_NewID()
        {
            SetupConnections();
            client1.RequestLease();
            client2.RequestLease();


            System.Threading.Thread.Sleep(100);



        }

    }
}
