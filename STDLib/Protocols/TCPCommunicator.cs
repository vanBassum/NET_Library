using System;

namespace STDLib
{
    public class TCPCommunicator : ICommunicator
    {
        public event EventHandler<Package> OnPackageRecieved;

        readonly Framing framing = new Framing();
        public TcpSocketClient Socket { get; private set; }

        public TCPCommunicator()
        {
            Socket = new TcpSocketClient();
            Socket.OnDataRecieved += TCPCommunicator_OnDataRecieved;
            framing.OnFrameCollected += Framing_OnFrameCollected;
        }

        public TCPCommunicator(TcpSocketClient tcpSocketClient)
        {
            Socket = tcpSocketClient;
            Socket.OnDataRecieved += TCPCommunicator_OnDataRecieved;
            framing.OnFrameCollected += Framing_OnFrameCollected;
        }

        private void Framing_OnFrameCollected(object sender, byte[] e)
        {
            Package package = new Package();
            package.Populate(e);
            OnPackageRecieved?.Invoke(this, package);
        }

        private void TCPCommunicator_OnDataRecieved(object sender, byte[] e)
        {
            framing.Unstuff(e);
        }

        public void SendPackage(Package package)
        {
            Socket.SendDataSync(framing.Stuff(package.GetBytes()));
        }
    }

}
