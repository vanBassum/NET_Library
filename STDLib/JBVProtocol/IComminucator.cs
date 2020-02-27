using System;

namespace STDLib.JBVProtocol
{
    public interface ICommunicator
    {
        void SendPackage(Package package);
        event EventHandler<Package> OnPackageRecieved;

    }


}
