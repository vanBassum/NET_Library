using System;

namespace STDLib
{
    public interface ICommunicator
    {
        void SendPackage(Package package);
        event EventHandler<Package> OnPackageRecieved;

    }


}
