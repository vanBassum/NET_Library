using STDLib.JBVProtocol.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STDLib.JBVProtocol.Devices
{
    public abstract class Device
    {
        static Dictionary<SoftwareID, Type> DeviceList = new Dictionary<SoftwareID, Type>();
        public UInt16 ID { get; set; }
        public abstract SoftwareID SoftwareID { get; }
        protected static JBVClient Client { get; set; }

        public static event EventHandler<Device> OnDeviceFound;

        public static void Init(JBVClient client)
        {
            Client = client;
            Client.CommandRecieved += Client_CommandRecieved;

            foreach (Type t in FindSubClassesOf<Device>())
            {
                Device d = (Device)Activator.CreateInstance(t);
                DeviceList[d.SoftwareID] = t;
            }
        }

        private static void Client_CommandRecieved(object sender, Command e)
        {
            switch (e)
            {
                case ReplySID cmd:
                    Type type;
                    if (DeviceList.TryGetValue(cmd.SID, out type))
                    {
                        Device dev = (Device)Activator.CreateInstance(type);
                        dev.ID = cmd.TxID;
                        OnDeviceFound?.Invoke(null, dev);
                    }
                    break;
            }
        }

        public static void SearchDevices(SoftwareID softId = SoftwareID.Unknown)
        {
            RequestSID cmd = new RequestSID();
            cmd.SID = softId;
            Client.SendCMD(cmd);
        }

        static IEnumerable<Type> FindSubClassesOf<TBaseType>()
        {
            var baseType = typeof(TBaseType);
            var assembly = baseType.Assembly;

            return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}
