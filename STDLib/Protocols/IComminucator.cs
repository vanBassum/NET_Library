using System;

namespace STDLib
{
    public interface IComminucator
    {
        void SendPackage(Package package);
        event EventHandler<Package> OnPackageRecieved;

    }


}
