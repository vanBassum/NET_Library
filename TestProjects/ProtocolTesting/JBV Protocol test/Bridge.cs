using STDLib.JBVProtocol.Connections;

namespace JBV_Protocol_test
{
    public class Bridge
    {
        public DummyConnection Con1 { get; set; } = new DummyConnection();
        public DummyConnection Con2 { get; set; } = new DummyConnection();

        public Bridge()
        {
            DummyConnection.CoupleConnections(Con1, Con2);
        }
    }

}
